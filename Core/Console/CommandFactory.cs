using System;
using System.Windows.Input;

namespace DevConsole {
    public static class CommandFactory {
        public static bool ExecuteCommand(string commandString) {
            var parts = commandString.Split(' ');
            var commandName = parts[0];
            var commandType = Type.GetType($"DevConsole.Commands.{commandName}Command");
            if (commandType == null) {
                return false;
            }

            var command = (IDevConsoleCommand)Activator.CreateInstance(commandType);

            var args = new string[parts.Length - 1];
            Array.Copy(parts, 1, args, 0, args.Length);

            return command.Execute(args);
        }
    }
}