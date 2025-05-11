using UnityEngine;
using XiheFramework.Runtime;

namespace XiheFramework.Core.Console.ConsoleCommands {
    public class HelpCommand : IDevConsoleCommand {
        public string CommandName => "help";

        public CommandExecutionResult Execute(string[] args, out string message) {
            var availableCommandNames = Game.Console.GetAvailableCommandNames();
            message = string.Join("\n", availableCommandNames);
            return CommandExecutionResult.Success;
        }
    }
}