#nullable enable
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class ScriptEngine : MonoBehaviour
{
    //list of reference assemblies to use when compiling, library mods are loaded first and added to the end of this list
    //all assemblies currently loaded are added to this list by the constructor
    private static List<PortableExecutableReference> references = new List<PortableExecutableReference>();
    public static string ExceptionMassager = "";
    private static RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
    
    static ScriptEngine()
    {
        
        //add all currently referenced assemblies so mods have access to them
        Assembly[] referencedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (Assembly assembly in referencedAssemblies)
        {
            try
            {
                if (!AddAssemblyFromPath(assembly.Location))
                {
                    Debug.LogError($"Assembly {Path.GetFileName(assembly.Location)} failed to load!");
                }
            }
            catch (NotSupportedException e)
            {
                Debug.Log("Assembly location not supported, probably loaded from memory\n" + e);
            }
        }
        //Goggs: should be for windows platform
        //Debug.LogError(Assembly.GetCallingAssembly().GetName());
    }

    /// <summary>
    /// Returns null on error
    /// </summary>
    /// <param name="code">Literal code to be compiled</param>
    /// <param name="fileName">Name of the code</param>
    /// <returns></returns>
    public static Assembly? Compile(string code, string fileName)
    {
        //actual compilation happens here
        SyntaxTree tree = SyntaxFactory.ParseSyntaxTree(code);
        //tree.Options = new CSharpParseOptions().WithLanguageVersion(LanguageVersion.CSharp10)
        //    .WithDocumentationMode(DocumentationMode.Diagnose);
        CSharpCompilation compilation = CSharpCompilation.Create(fileName)
            .WithOptions(new CSharpCompilationOptions(
                outputKind: OutputKind.DynamicallyLinkedLibrary, //build a dll...?
                optimizationLevel: OptimizationLevel.Debug)//optimize the code, we may change this to debug later if it's too slow
                .WithPlatform(Platform.X64)) 
            .WithReferences(references)
            .AddSyntaxTrees(tree);

        //the compiled dll is outputted to this memory stream
        using MemoryStream ms = new MemoryStream();
        EmitResult? compilationResult = null;
        string? errorMessage = null;

        //woah we actually compile it here
        compilationResult = compilation.Emit(ms);

        //error handling
        if (!compilationResult.Success)
        {
            StringBuilder errMessageBuilder = new StringBuilder($"Error compiling code for mod {fileName}!");

            foreach (Diagnostic diagnostic in compilationResult.Diagnostics)
            {
                errMessageBuilder.AppendLine(diagnostic.ToString());
            }
            errorMessage = errMessageBuilder.ToString();

            Debug.LogError(errorMessage);
            ExceptionMassager = errorMessage;
            return null;
        }

        //and finally, we load the assembly ^-^
        return Assembly.Load(ms.ToArray());
    }

    //returns true on success, false on failure
    private static bool AddAssemblyFromPath(string path)
    {
        //ignore empty paths
        if (string.IsNullOrEmpty(path)) return false;

        string fullPath = Path.GetFullPath(path);

        if (!File.Exists(fullPath)) return false;

        //ignore already referenced libraries
        if (references.Any(r => Path.GetFullPath(r.FilePath) == fullPath)) return true;

        try
        {
            references.Add(MetadataReference.CreateFromFile(fullPath));
        }
        catch
        {
            return false;
        }

        return true;
    }

    public static Func<InputType, OutputType>? CompileAsFunc<InputType, OutputType>(string code, string inputTypeName)
    {
        string randomClassName = RandomString(42, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
        string wrappedCode = $"using System;\nusing System.Collections.Generic;\nusing System.IO;\nusing System.Linq;\nusing System.Reflection;\nusing System.Text;\nusing UnityEngine;\n" +
            "public class "+randomClassName+" : MonoBehaviour\n{public "+typeof(OutputType)+" Main("+typeof(InputType)+" "+inputTypeName+")\n{return "+code+";\n}}";
        Assembly? nullableAssembly = Compile(wrappedCode, $"eval{randomClassName}");
        if (nullableAssembly == null)
        {
            return null;
        }
        else
        {
            object assemblyInstance = nullableAssembly.CreateInstance(randomClassName)!;
            MethodInfo methodInfo = nullableAssembly.GetType(randomClassName).GetMethod("Main")!;
            return delegate(InputType type) 
            { 
                OutputType returnValue = (OutputType)methodInfo.Invoke(assemblyInstance, new object[]{type});
                return returnValue;
            };
        }
    }
    public static string RandomString(int length, string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "length cannot be less than zero.");
        }
			
        if (string.IsNullOrEmpty(allowedChars))
        {
            throw new ArgumentException("allowedChars may not be empty.");
        }
        var allowedCharSet = new HashSet<char>(allowedChars).ToArray();
			
        if (256 < allowedCharSet.Length)
        {
            throw new ArgumentException($"allowedChars may contain no more than 256 characters.");
        }

        
        var result = new StringBuilder();
        var buf = new byte[128];
        while (result.Length < length)
        {
            randomNumberGenerator.GetBytes(buf);
            var i = 0;
            while (i < buf.Length && result.Length < length)
            {
                var outOfRangeStart = 256 - 256 % allowedCharSet.Length;
                if (outOfRangeStart > buf[i])
                {
                    result.Append(allowedCharSet[buf[i] % allowedCharSet.Length]);
                }
                i++;
            }
        }
        var result2 = result.ToString();
        return result2;
    }
    public string WarpCodeSnippet(string snippet, string randomClassName = "Start")
    {
        string[] lines = snippet.Split('\n');
        string usings = "";
        string regularCode = "";
        foreach (string line in lines)
        {
            if (line.Contains("using"))
            {
                usings += line + '\n';
            }
            else
            {
                regularCode += line + '\n';
            }
        }

        return @"
        using System;
        using System.Collections.Generic;
        using System.IO;
        using System.Linq;
        using System.Reflection;
        using System.Text;
        using UnityEngine;
        "
               + usings
               + @"
        
        public class "+randomClassName+@" : MonoBehaviour
        {
            public static object Main()
            {
                "
               + regularCode
               + @"
                return ""No return statement!"";
            }
        }
        ";
    }
}
