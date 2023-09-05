using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using UnityEngine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Text;

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

	private byte[] m_assemblyData;

	public bool IsChanged { get; private set; }

	public static REConsoleInterface Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		m_consoleCode.text = INLocalization.Instance.GetText("ConsoleInterface_TextArea");
		m_compileButton.onClick.AddListener(Compile);
		m_runButton.onClick.AddListener(Run);
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
				m_consoleOutput.text = INLocalization.Instance.GetText("ConsoleInterface_CompileFailed");
				foreach (var diagnostic in result.Diagnostics)
				{
					m_consoleOutput.text += '\n' + diagnostic.ValueToString();
				}
			}
			else
			{
				ms.Seek(0, SeekOrigin.Begin);
				m_assemblyData = ms.ToArray();
				m_consoleOutput.text = INLocalization.Instance.GetText("ConsoleInterface_CompileSuccess");
			}
		}
	}

	private void Run()
	{
		m_consoleOutput.text = "";
		TextWriter textWriter = Console.Out;
		StringBuilder stringBuilder = new StringBuilder();
		StringWriter stringWriter = new StringWriter(stringBuilder);
		Console.SetOut(stringWriter);

		var assembly = Assembly.Load(m_assemblyData);

		Type dynamicType = assembly.GetType("BPProgram");
		if (dynamicType == null)
		{
			m_consoleOutput.text += INLocalization.Instance.GetText("ConsoleInterface_RuntimeErrorGetType");
			goto end;
		}

		object instance = Activator.CreateInstance(dynamicType);
		if (instance == null)
		{
			m_consoleOutput.text += INLocalization.Instance.GetText("ConsoleInterface_RuntimeErrorInitType");
			goto end;
		}

		MethodInfo methodInfo = dynamicType.GetMethod("BPMain");
		if (methodInfo == null)
		{
			m_consoleOutput.text += INLocalization.Instance.GetText("ConsoleInterface_RuntimeErrorInitType");
			goto end;
		}

		UnityLogRedirector unityLogRedirector = new UnityLogRedirector();
		unityLogRedirector.BeginRedirection();

		object returnValue = methodInfo.Invoke(instance, null);
		m_consoleOutput.text += stringBuilder.ToString();

		unityLogRedirector.EndRedirection();
		m_consoleOutput.text += unityLogRedirector.GetRedirectedLog();

		if (returnValue == null)
		{
			m_consoleOutput.text += INLocalization.Instance.GetText("ConsoleInterface_RuntimeErrorInvokeMethod");
			goto end;
		}

		try {
			m_consoleOutput.text += INLocalization.Instance.GetText("ConsoleInterface_ReturnValue") + (int) returnValue;
		}
		catch (Exception exception)
		{
			m_consoleOutput.text += INLocalization.Instance.GetText("ConsoleInterface_RuntimeErrorReturnConvert") + exception.Message;
		}

	end:
		Console.SetOut(textWriter);
		return;
	}

	private class UnityLogRedirector
	{
		public StringBuilder stringBuilder;

		public UnityLogRedirector()
		{
			stringBuilder = new StringBuilder();
		}

		public void BeginRedirection()
		{
			Application.logMessageReceived += LogMessageReceived;
		}

		private void LogMessageReceived(string logMessage, string stackTrace, LogType logType)
		{
			stringBuilder.AppendLine(logMessage);
		}

		public void EndRedirection()
		{
			Application.logMessageReceived -= LogMessageReceived;
		}

		public string GetRedirectedLog()
		{
			return stringBuilder.ToString();
		}
	}
}
