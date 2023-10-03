using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using System.Linq;
using UnityEngine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Text;
using RECmd;
using UnityEngine.Serialization;

public class RECommandInterface : MonoBehaviour
{

    [SerializeField] private GameObject m_content;

    [SerializeField] private UnityEngine.UI.Text m_commandOutput;

    [SerializeField] private UnityEngine.UI.InputField m_commandInput;

    [SerializeField] private UnityEngine.UI.Button m_executeButton;

    [FormerlySerializedAs("m_runButton")] [SerializeField] private UnityEngine.UI.Button m_helpButton;

    public RECommandHandler ReCommandHandler;
    public bool IsChanged { get; private set; }
    public static RECommandInterface Instance { get; private set; }

    private TextWriter m_commandOut;

    private StringBuilder m_logBuilder;


    private void Awake()
    {
        Instance = this;
        m_executeButton.onClick.AddListener(Execute);
        m_helpButton.onClick.AddListener(Help);
        m_logBuilder = new StringBuilder();
        ReCommandHandler = new RECommandHandler();
        ReCommandHandler.RegisterCommand("gamerule", "Gets or Sets a gamerule", new []
            {
                new RECommandArgDef("Mode", "String", "Select mode (Get or Set)"),
                new RECommandArgDef("Gamerule", "String", "Select a gamerule"),
                new RECommandArgDef("Value","String","(only for set mode)The value to be set")
            },
           (args) =>
            {
               //we're starting from 0 since we skip the first part in execution
               string mode = args.GetLowNormString(0);
               string gamerule = args.GetLowNormString(1);
               if (mode == "get")
               {
                   Console.WriteLine($"Value of gamerule {args.GetString(1)} is {GameRules.GetGameRuleString(gamerule)}.");
               }
               else if (mode == "set")
               {
                   Console.WriteLine($"Set gamerule {args.GetString(1)} to {args.GetString(2)}.");
                   GameRules.SetGameRule(gamerule, args.GetLowNormString(2));
               }
            }
        );
        ReCommandHandler.RegisterCommand("shift", "Shifts your contraption on grid", new []
            {
                new RECommandArgDef("X-Shift", "Integer", "Shift amount on X-Axis"),
                new RECommandArgDef("Y-Shift", "Integer", "Shift amount on Y-Axis")
            },
            (args) =>
            {
                int dx = args.GetInt(0);
                int dy = args.GetInt(1);
                if (BPManual.Contraption)
                {
                    BPManual.Contraption.MoveOnGrid(dx,dy);
                    Console.WriteLine(INLocalization.Instance.GetText("CommandInterface_Shift"), dx, dy);
                }
                else
                {
                    Console.WriteLine(INLocalization.Instance.GetText("CommandInterface_ContraptionNotFound"));
                }
            }
        );
        ReCommandHandler.RegisterCommand("setpart", "sets a new part at certain location", new []
            {
                new RECommandArgDef("x", "Integer", "x Coordinate of new part"),
                new RECommandArgDef("y", "Integer", "y Coordinate of new part"),
                new RECommandArgDef("type", "Integer", "part type of new part"),
                new RECommandArgDef("custom index", "Integer", "skin value of new part"),
                new RECommandArgDef("grid rotation", "Integer[0..8]", "grid rotation value of new part"),
                new RECommandArgDef("is it flipped", "Boolean as Integer", "determine if new part is flipped")
            },
            (args) =>
            {
                if (BPManual.Contraption)
                {
                    BPManual.Contraption.DataSet.AddPart(args.GetInt(0),args.GetInt(1),args.GetInt(2),args.GetInt(3),args.GetInt(4),args.GetBool(5));
                    Console.WriteLine(INLocalization.Instance.GetText("CommandInterface_SetPart"), args.GetInt(0),args.GetInt(1),args.GetInt(2),args.GetInt(3),args.GetInt(4),args.GetBool(5));
                }
                else
                {
                    Console.WriteLine(INLocalization.Instance.GetText("CommandInterface_ContraptionNotFound"));
                }
            }
        );
    }

    private void Execute()
    {
        m_commandOut = Console.Out;
        m_logBuilder.Clear();
        Console.SetOut(new StringWriter(m_logBuilder));
        
        ReCommandHandler.RunCommand(m_commandInput.text);
        
        m_commandOutput.text += m_logBuilder.ToString();
        Console.SetOut(m_commandOut);
    }

    private void Help()
    {
        m_commandOut = Console.Out;
        m_logBuilder.Clear();
        Console.SetOut(new StringWriter(m_logBuilder));
        
        Console.WriteLine(BPManual.GetHelpPage("0"));
        
        m_commandOutput.text += m_logBuilder.ToString();    
        Console.SetOut(m_commandOut);
        
    }

    private void DebugLogReader(string logMessage, string stackTrace, LogType logType)
    {
        m_logBuilder.AppendLine(logMessage);
    }
}