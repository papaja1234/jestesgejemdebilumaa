using System;
using System.IO;
using UnityEngine;
using System.Text;
using Unity.Mathematics;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

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
        DontDestroyOnLoad(this);
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
                new RECommandArgDef("grid rotation", "Integer[..7]", "grid rotation value of new part"),
                new RECommandArgDef("is it flipped", "Boolean as Integer", "determine if new part is flipped")
            },
            (args) =>
            {
                Debug.Log(WPFMonoBehaviour.levelManager.gameState);
                Debug.Log(nameof(WPFMonoBehaviour.levelManager.gameState));
                if (Contraption.Instance && WPFMonoBehaviour.levelManager && WPFMonoBehaviour.levelManager.ConstructionUI)
                {
                    if (WPFMonoBehaviour.levelManager.gameState is LevelManager.GameState.Running or LevelManager.GameState.PausedWhileRunning or LevelManager.GameState.PreviewWhileRunning)
                    {/*
                        ContraptionDataset.ContraptionDatasetUnit unit = new ContraptionDataset.ContraptionDatasetUnit
                        {
                            x = args.GetInt(0),
                            y = args.GetInt(1),
                            partType = args.GetInt(2),
                            customPartIndex = args.GetInt(3),
                            rot = args.GetInt(4),
                            flipped = args.GetBool(5)
                        };
                        BasePart customPart = WPFMonoBehaviour.gameData.GetCustomPart(((SortedPartType)args.GetInt(2)).ToPartType(),
                            unit.customPartIndex);
                    
                        if (customPart != null)
                        {
                            Contraption.Instance.Parts.Add(customPart);
                            customPart.contraption = Contraption.Instance;
                            customPart.enabled = true;
                            customPart.Initialize();
                            customPart.PostInitialize();
                            GameObject obj = Object.Instantiate(customPart.gameObject);
                            obj.SetActive(value: true);
                            customPart.EnsureRigidbody();
                        }
                        Console.WriteLine(INLocalization.Instance.GetText("CommandInterface_SetPart"), args.GetInt(0),
                            args.GetInt(1), args.GetInt(2), args.GetInt(3), args.GetInt(4), args.GetBool(5));*/
                        return;
                    }else if (WPFMonoBehaviour.levelManager.gameState is LevelManager.GameState.Building
                              or LevelManager.GameState.PausedWhileBuilding
                              or LevelManager.GameState.PreviewWhileBuilding)
                    {
                        int x = args.GetInt(0);
                        int y = args.GetInt(1);
                        int intPartType = args.GetInt(2);
                        int customPartIndex = args.GetInt(3);
                        int rotation = args.GetInt(4);
                        bool isFlipped = args.GetBool(5);
                        BasePart.PartType partType = ((SortedPartType)intPartType).ToPartType();
                        Contraption.Instance.DataSet.AddPart
                        (
                            x,
                            y,
                            intPartType,
                            customPartIndex,
                            rotation, isFlipped, 
                            out ContraptionDataset.ContraptionDatasetUnit unit
                        );
                        ConstructionUI.PartDesc partDesc = 
                        WPFMonoBehaviour.
                            levelManager.
                                ConstructionUI.
                                    FindPartDesc
                                    (
                                        partType
                                    );
                        BasePart customPart = 
                            WPFMonoBehaviour.
                                gameData.
                                    GetCustomPart
                                    (
                                        partType,
                                        unit.customPartIndex
                                    );

                        if (customPart != null)
                        {
                            WPFMonoBehaviour.levelManager.BuildPart(unit, customPart);
                            partDesc.useCount++;
                        }

                        Console.WriteLine(INLocalization.Instance.GetText("CommandInterface_SetPart"), x,
                            y, intPartType, customPartIndex, rotation, isFlipped);
                    }
                }
                else
                {
                    Console.WriteLine(INLocalization.Instance.GetText("CommandInterface_ContraptionNotFound"));
                }
            }
        );
        ReCommandHandler.RegisterCommand("#multiline", "execute following lines of code line-by-line", new []
            {
                new RECommandArgDef("commands", "Command[]", "Multiple Lines of Command")
            },
            (args) =>
            {
                ReCommandHandler.HandleMultiLineCommand(args.GetRawString());
            }
        );
        ReCommandHandler.RegisterCommand("delay", "delay for a Time Span (seconds), only used with #multiline", new []
            {
                new RECommandArgDef("delay amount", "Float", "How long to delay in seconds")
            },
            (args) =>
            {
                
            }
        );
        ReCommandHandler.RegisterCommand("fill", "fills an rectangular area with a new part", new []
            {
                new RECommandArgDef("x1", "Integer", "x Coordinate of rectangle vertex1"),
                new RECommandArgDef("y1", "Integer", "y Coordinate of rectangle vertex1"),
                new RECommandArgDef("x2", "Integer", "x Coordinate of rectangle vertex2"),
                new RECommandArgDef("y2", "Integer", "y Coordinate of rectangle vertex2"),
                new RECommandArgDef("type", "Integer", "part type of new part"),
                new RECommandArgDef("custom index", "Integer", "skin value of new part"),
                new RECommandArgDef("grid rotation", "Integer[..7]", "grid rotation value of new part"),
                new RECommandArgDef("is it flipped", "Boolean as Integer", "determine if new part is flipped")
            },
            (args) =>
            {
                if (Contraption.Instance && WPFMonoBehaviour.levelManager && WPFMonoBehaviour.levelManager.ConstructionUI)
                {
                    int x, y;
                    ConstructionUI.PartDesc partDesc;

                    if (!(WPFMonoBehaviour.levelManager.gameState is LevelManager.GameState.Building
                            or LevelManager.GameState.PausedWhileBuilding
                            or LevelManager.GameState.PreviewWhileBuilding)) return;

                    partDesc =
                        WPFMonoBehaviour.levelManager.ConstructionUI.FindPartDesc(((SortedPartType)args.GetInt(4))
                            .ToPartType());
                    partDesc.useCount += (Math.Max(args.GetInt(0), args.GetInt(2)) -
                                          Math.Min(args.GetInt(0), args.GetInt(2))) *
                                         (Math.Max(args.GetInt(1), args.GetInt(3)) -
                                          Math.Min(args.GetInt(1), args.GetInt(3)));
                    
                    

                    ContraptionDataset.ContraptionDatasetUnit unit = new ContraptionDataset.ContraptionDatasetUnit
                    {
                        partType = args.GetInt(4),
                        customPartIndex = args.GetInt(5),
                        rot = args.GetInt(6),
                        flipped = args.GetBool(7)
                    };
                    for (x = Math.Min(args.GetInt(0),args.GetInt(2)); x <= Math.Max(args.GetInt(0),args.GetInt(2)); x++)
                    {
                        for (y = Math.Min(args.GetInt(1),args.GetInt(3)); y <= Math.Max(args.GetInt(1),args.GetInt(3)); y++)
                        {
                            unit.x = x;
                            unit.y = y;
                            BPManual.Contraption.DataSet.AddPart(x, y, args.GetInt(2), args.GetInt(3),
                                args.GetInt(4), args.GetBool(5), out ContraptionDataset.ContraptionDatasetUnit _);
                            BasePart customPart = WPFMonoBehaviour.gameData.GetCustomPart(((SortedPartType)unit.partType).ToPartType(),unit.customPartIndex);
                            if (customPart != null)
                            {
                                WPFMonoBehaviour.levelManager.BuildPart(unit, customPart);
                            }

                            if (WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.Running)
                            {
                                customPart.EnsureRigidbody();
                            }
                        }
                    }
                    Console.WriteLine(INLocalization.Instance.GetText("CommandInterface_FillPart"), args.GetInt(0),
                        args.GetInt(1), args.GetInt(2), args.GetInt(3), args.GetInt(4), args.GetInt(5),args.GetInt(6),args.GetBool(7));
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
        if (GameRules.ShowCommandLog)
        {
            m_commandOut = Console.Out;
            m_logBuilder.Clear();
            Console.SetOut(new StringWriter(m_logBuilder));
        }
        ReCommandHandler.RunCommand(m_commandInput.text);
        if (!GameRules.ShowCommandLog) return;
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