#nullable enable
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public class ScriptEngine : MonoBehaviour
{
    //list of reference assemblies to use when compiling, library mods are loaded first and added to the end of this list
    //all assemblies currently loaded are added to this list by the constructor
    private static List<PortableExecutableReference> references = new List<PortableExecutableReference>();
    public static string ExceptionMassager = "";
    
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
        CSharpCompilation compilation = CSharpCompilation.Create(fileName)
            .WithOptions(new CSharpCompilationOptions(
                outputKind: OutputKind.DynamicallyLinkedLibrary, //build a dll
                optimizationLevel: OptimizationLevel.Release)) //optimize the code, we may change this to debug later if it's too slow
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
}
