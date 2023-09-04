using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using UnityEngine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

public class REConsoleInterface : MonoBehaviour
{
	[SerializeField]
	private GameObject m_content;

	[SerializeField]
	private UnityEngine.UI.InputField m_consoleCode;

	[SerializeField]
	private UnityEngine.UI.InputField m_consoleOutput;

	[SerializeField]
	private UnityEngine.UI.Button m_compileButton;

	[SerializeField]
	private UnityEngine.UI.Button m_runButton;

	public bool IsChanged { get; private set; }

	public static REConsoleInterface Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
	}

	private void Compile()
	{
		string code = m_consoleCode.text;
		SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
		string assemblyName = "DynamicAssembly";

		List<MetadataReference> references = new List<MetadataReference>();

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !assembly.IsDynamic))
        {
            try
            {
                references.Add(MetadataReference.CreateFromFile(assembly.Location));
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to add reference for assembly {assembly.FullName}: {ex.Message}");
            }
        }

		CSharpCompilation compilation = CSharpCompilation.Create(
			assemblyName,
			syntaxTrees: new[] { syntaxTree },
			references: references,
			options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

		using (var ms = new MemoryStream())
		{
			EmitResult result = compilation.Emit(ms);

			if (!result.Success)
			{
				Console.WriteLine("Compilation errors:");
				foreach (var diagnostic in result.Diagnostics)
				{
					Console.WriteLine(diagnostic);
				}
			}
			else
			{
				ms.Seek(0, SeekOrigin.Begin);
				byte[] assemblyData = ms.ToArray();
				var assembly = Assembly.Load(assemblyData);
				string typeName = "SystemCommandExecution.Program";
				Type dynamicType = assembly.GetType(typeName);
				if (dynamicType == null)
				{
					Console.WriteLine($"Error getting the type {typeName}");
					return;
				}
				object instance = Activator.CreateInstance(dynamicType);
				if (instance == null)
				{
					Console.WriteLine("Failed to create an instance of dynamicType");
					return;
				}
				MethodInfo methodInfo = dynamicType.GetMethod("DynamicMain");
				if (methodInfo == null)
				{
					Console.WriteLine("Failed to get method info of dynamicType");
					return;
				}
				methodInfo.Invoke(instance, null);
			}
		}
	}
}
