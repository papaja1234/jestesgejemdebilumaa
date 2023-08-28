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
    public string randomClassName = ScriptEngine.RandomString(13, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");

    private int index = 0;
    
    private void Start()
    {
        ScriptEngine = gameObject.AddComponent<ScriptEngine>();
        ScriptEngine.print("STASIS");
        EnterButton.onClick.AddListener(ExecuteCode);
        ClearButton.onClick.AddListener(ClearConsole);
        /*//testing
        var func = ScriptEngine.CompileAsFunc<double,double>("1d/Math.Sqrt(x)","x");
        string outut = "";
        int i = 1;
        while(i < 25)
        {
            outut+=func(i).ToString()+"\n";
            i++;
        }

        ConsoleOutput.text = outut;
        ConsoleOutput.text += func(func(func(func(func(func(func(func(func(func(func(func(func(145)))))))))))));
        */
    }

    private void ClearConsole()
    {
        ConsoleOutput.text = "";
    }

    private void ExecuteCode()
    {
        randomClassName = ScriptEngine.RandomString(13, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
        string code = ScriptEngine.WarpCodeSnippet(ConsoleInput.text, randomClassName);
        Assembly? nullableAssembly= ScriptEngine.Compile(code, index.ToString());
        if (nullableAssembly != null)
        {
            object assemblyInstance = nullableAssembly.CreateInstance(randomClassName)!;
            MethodInfo methodInfo = nullableAssembly.GetType(randomClassName).GetMethod("Main")!;
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
}
