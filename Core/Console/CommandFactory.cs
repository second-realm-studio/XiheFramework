using System;
using System.Collections.Generic;
using UnityEngine;

namespace XiheFramework.Core.Console {
    public static class CommandFactory {
        public static bool ExecuteCommand(string commandString) {
            var parts = commandString.Split(' ');
            var commandName = parts[0];
            //search all assemblies for the command
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                var commandType = assembly.GetType($"DevCommands.{commandName}Command");
                if (commandType != null) {
                    var command = (IDevConsoleCommand)Activator.CreateInstance(commandType);

                    var args = new string[parts.Length - 1];
                    Array.Copy(parts, 1, args, 0, args.Length);

                    return command.Execute(args);
                }
            }

            Debug.LogError($"Command {commandName} not found");
            return false;
        }

        public static string[] PrintAllCommands() {
            //search all assemblies for the command
            var result = new List<string>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (var type in assembly.GetTypes()) {
                    if (type.GetInterface("IDevConsoleCommand") != null) {
                        result.Add(type.Name);
                    }
                }
            }

            return result.ToArray();
        }
    }
}