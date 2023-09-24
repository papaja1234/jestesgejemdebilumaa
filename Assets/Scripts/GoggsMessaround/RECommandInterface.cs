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
#if LEGACY_COMMAND_SYSTEM
    /// <summary>
    /// String tuple of Name and Description
    /// </summary>
    public struct NDString
    {
        public string Name;
        public string Description;

        /// <summary>
        /// Create a new Name-Description string tuple
        /// </summary>
        /// <param name="name">Name of the tuple</param>
        /// <param name="description">Description of the tuple</param>
        public NDString(string name, string description)
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Special ToString() Method
        /// </summary>
        /// <returns>Formatted string in form of "x (y)"</returns>
        public string GetFormatted()
        {
            return Name + " (" + Description + ")";
        }
    }

    public class Command
    {
        private NDString m_value;
        private List<NDString> m_arguments;
        private Action<List<string>> m_action;
        
        /// <summary>
        /// Name of this is used for finding commands (ignore caps)
        /// Description of this is used for documentations 
        /// </summary>
        public NDString Value => m_value;
        public List<NDString> Arguments => m_arguments;
        public Action<List<string>> Action => m_action;

        public Command(NDString value, List<NDString> arguments, Action<List<string>> action)
        {
            m_value = value;
            m_arguments = arguments;
            m_action = action;
        }

        public void Execute(List<string> arguments)
        {
            if (arguments.Count == m_arguments.Count)
                m_action.Invoke(arguments);
            else WriteInvalidArguments();
        }

        /// <summary>
        /// Throw a exception but imaginary
        /// </summary>
        public void WriteInvalidArguments()
        {
            Console.Write("Invalid arguments, required: ");
            bool once = true;
            foreach (NDString argument in m_arguments)
            {
                if (once) once = false;
                else Console.Write(", ");
                Console.Write(argument.GetFormatted());
            }

            Console.WriteLine();
        }
    }

    public class CommandManager
    {
        private List<Command> m_commands;
        public List<Command> Commands => m_commands;

        public CommandManager(List<Command> commands)
        {
            m_commands = commands;
        }

        public void Execute(string command)
        {
            List<string> split = command.Split(' ').ToList();
            Execute(split[0], split.GetRange(1, split.Count - 1));
        }

        public void Execute(string name, List<string> arguments)
        {
            foreach (Command command in m_commands)
            {
                if (string.Equals(name.Normalize(), command.Value.Name.Normalize(), StringComparison.CurrentCultureIgnoreCase))
                {
                    command.Execute(arguments);
                    return;
                }
            }

            WriteInvalidCommand();
        }


        public void WriteInvalidCommand()
        {
            Console.Write("Invalid command, valid choices: ");
            bool once = true;
            foreach (Command command in m_commands)
            {
                if (once) once = false;
                else Console.Write(", ");
                Console.Write(command.Value.GetFormatted());
            }

            Console.WriteLine();
        }
    }
#endif
    [SerializeField] private GameObject m_content;

    [SerializeField] private UnityEngine.UI.Text m_commandOutput;

    [SerializeField] private UnityEngine.UI.InputField m_commandInput;

    [SerializeField] private UnityEngine.UI.Button m_executeButton;

    [FormerlySerializedAs("m_runButton")] [SerializeField] private UnityEngine.UI.Button m_helpButton;

#if LEGACY_COMMAND_SYSTEM
    public CommandManager commandManager { get; set; } = commandManager = 
            new CommandManager(new List<Command>
                {
                    //Help Command
                    new Command(
                        new NDString("help", "Display help pages"),
                        new List<NDString>
                        {
                            new NDString("page",
                                "The page of help text")
                        },
                        delegate(List<string> args)
                        {
                            Console.WriteLine(BPManual.GetHelpPage(args[0]));
                        }
                        ),
                    new Command(
                        new NDString("gamerule", "Set and See gamerules"),
                        new List<NDString>
                        {
                            new NDString("rulename",
                                "Rule name to be changed/seeked"),
                            new NDString("value",
                                "Value to be settled")
                        },
                        delegate(List<string> args)
                        {
                            Console.WriteLine(BPManual.GetHelpPage(args[0]));
                        }
                        )
                }
        );//yeah you can add commands using the c# command Interface
#endif

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
        ReCommandHandler.RegisterCommand("gamerule", "Gets or Sets a gamerule", new []{new RECommandArgDef("Mode", "String", "Select mode (Get or Set)")},
           (args) =>
           {
               //we're starting from 0 since we skip the first part in execution
               Console.SetOut(m_commandOut);
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
           });
    }

    private void Execute()
    {
        m_commandOut = Console.Out;
        m_logBuilder.Clear();
        StringWriter stringWriter = new StringWriter(m_logBuilder);
        Console.SetOut(stringWriter);
        ReCommandHandler.RunCommand(m_commandInput.text);
        m_commandOutput.text += stringWriter.ToString();
        Console.SetOut(m_commandOut);
    }

    private void Help()
    {
        m_commandOutput.text += BPManual.GetHelpPage("0");
    }

    private void DebugLogReader(string logMessage, string stackTrace, LogType logType)
    {
        m_logBuilder.AppendLine(logMessage);
    }
}