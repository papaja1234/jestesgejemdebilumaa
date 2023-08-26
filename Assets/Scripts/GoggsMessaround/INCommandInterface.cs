#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class INCommandInterface : MonoBehaviour
{
    private ScriptEngine ScriptEngine;
    public Text ConsoleInput;
    public Text ConsoleOutput;
    [FormerlySerializedAs("Enter")] public UnityEngine.UI.Button EnterButton;
    public UnityEngine.UI.Button ClearButton;

    private int index = 0;
    
    private void Start()
    {
        ScriptEngine.print("STASIS");
        EnterButton.onClick.AddListener(ExecuteCode);
        ClearButton.onClick.AddListener(ClearConsole);
    }

    private void ClearConsole()
    {
        ConsoleOutput.text = "";
    }

    private void ExecuteCode()
    {
        string code = WarpCodeSnippet(ConsoleInput.text);
        Assembly? nullableAssembly= ScriptEngine.Compile(code, index.ToString());
        if (nullableAssembly != null)
        {
            object assemblyInstance = nullableAssembly.CreateInstance("Start")!;
            MethodInfo methodInfo = nullableAssembly.GetType("Start").GetMethod("Main")!;
            try
            {
                object returnValue = methodInfo.Invoke(assemblyInstance, new object[]{});
                ConsoleOutput.text = returnValue.ToString();
            }
            catch (Exception e)
            {
                ConsoleOutput.text += "\n" +e.Message + e.StackTrace;
            }
            //INSettings.SetValue(INFeature.TerrainScale,whatever goes in here);
            
            Debug.Log("WOW HE WROTE A SUCCESSFUL CODE");
        }
        else
        {
            ConsoleOutput.text += ScriptEngine.ExceptionMassager;
        }

        index++;
    }

    private string WarpCodeSnippet(string snippet)
    {
        //may be changed later
        return
            $"using System;\nusing System.Collections.Generic;\nusing System.IO;\nusing System.Linq;\nusing System.Reflection;\nusing System.Text;\nusing UnityEngine;\n" +
            "public class Start : MonoBehaviour\n{public object Main()\n{"+snippet+"\nreturn \"No return statement!\";\n}}";
    }                                                                                         //^ return something just in case

}
