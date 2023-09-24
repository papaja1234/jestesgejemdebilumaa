using System;
using System.Collections.Generic;
using System.Linq;

namespace RECmd
{
    /// <summary>
    /// Handles commands.
    /// </summary>
    public class RECommandHandler
    {
        private Dictionary<string, RECommand> commands = new Dictionary<string, RECommand>(); //command name (lowercase!) -> command class

        public RECommandHandler()
        {
            //we register our help command here since it's special-ish
            RegisterCommand("help", "Provides help for a command.", new RECommandArgDef[] { new RECommandArgDef("Name", "String", "Name of the command to get help for.") }, (args) =>
            {
                if (!commands.TryGetValue(args.GetString(0).ToLower().Trim(), out RECommand command)) //we lower & trim since we ignore case (and trailing spaces etc)
                    throw new RECommandException($"Command {args.GetString(0)} not found!");
                Console.WriteLine(command.GetHelpString());
            });
        }

        /// <summary>
        /// Registers a command to this handler.
        /// </summary>
        /// <param name="name">Name of the command. (lowercase!)</param>
        /// <param name="description">Description of the command.</param>
        /// <param name="args">Arguments for the command (to print when the user asks for help)</param>
        /// <param name="execute">Method to execute the command.</param>
        public void RegisterCommand(string name, string description, RECommandArgDef[] args, Action<RECommandArgs> execute)
        {
            commands.Add(name, new RECommand(name, description, args, execute));
        }

        /// <summary>
        /// Removes a command.
        /// </summary>
        /// <param name="name">Name of the command to remove.</param>
        public void RemoveCommand(string name)
        {
            commands.Remove(name);
        }

        /// <summary>
        /// Runs a command previously registered.
        /// </summary>
        /// <param name="text">Raw text to invoke the command from (e.g. "/gamerule x y")</param>
        /// <exception cref="RECommandException"></exception>
        public void RunCommand(string text)
        {
            if (text.Normalize()[0] != '/')
            {
                Console.Write($"<{GameRules.GetGameRuleString("playername")}>: {text}\n");
                return;
            }
            string[] textParts = text.Normalize().TrimStart('/').Split(" "); //Dart: maybe use "　" (U+3000) instead of " " for chinese? //Goggs: i guess i'll just use string.Normalize(). also we don't use U+3000
            if (!commands.TryGetValue(textParts[0].ToLower().Trim(), out RECommand command)) //we lower & trim since we ignore case (and trailing spaces etc)
            {
                Console.WriteLine($"Command {textParts[0]} not found!");
                throw new RECommandException($"Command {textParts[0]} not found!");
            }

            command.Execute(new RECommandArgs(textParts.Skip(1).ToArray())); //skip first part since that's the command name
        }
    }
}
