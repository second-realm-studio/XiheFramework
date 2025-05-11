using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XiheFramework.Core.Base;
using XiheFramework.Runtime;

namespace XiheFramework.Core.Console {
    public class ConsoleModule : GameModuleBase {
        public override int Priority => 0;
        public List<string> Logs { get; private set; }

        private readonly Dictionary<string, IDevConsoleCommand> m_RegisteredCommands = new();
        private readonly Dictionary<string, string> m_CommandNameIndexingDictionary = new();

        public CommandExecutionResult ExecuteCommand(string commandString, out string message) {
            var parts = commandString.Split(' ');
            var commandName = parts[0];

            if (!m_CommandNameIndexingDictionary.TryGetValue(commandName, out var commandTypeName)) {
                message = $"Command {commandName} not found";
                return CommandExecutionResult.Fail;
            }

            if (!m_RegisteredCommands.TryGetValue(commandTypeName, out var command)) {
                message = $"Command {commandName} found, but instance lost";
                return CommandExecutionResult.Fail;
            }

            var args = new string[parts.Length - 1];
            Array.Copy(parts, 1, args, 0, args.Length);

            var result = command.Execute(args, out message);
            Logs.Add($"{DateTime.Now:h:mm:ss}\n{commandName} {string.Join(" ", args)}\n{message}");
            return result;
        }

        public string[] GetAvailableCommandNames() {
            return m_RegisteredCommands.Keys.ToArray();
        }

        public void LogMessage(string message) {
            Debug.Log(message);
            Logs.Add($"{DateTime.Now:h:mm:ss}\n{message}");
        }

        public void LogWarning(string message) {
            Debug.LogWarning(message);
            Logs.Add($"\n{DateTime.Now:h:mm:ss}\n<color=yellow>{message}</color>");
        }

        public void LogError(string message) {
            Debug.LogError(message);
            Logs.Add($"\n{DateTime.Now:h:mm:ss}\n<color=red>{message}</color>");
        }

        protected override void OnInstantiated() {
            RegisterAllCommands();
            Game.Console = this;
        }

        private void RegisterAllCommands() {
            m_RegisteredCommands.Clear();
            var commandTypes = System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IDevConsoleCommand).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

            foreach (var type in commandTypes) {
                if (!m_RegisteredCommands.ContainsKey(type.Name)) {
                    var commandInstance = (IDevConsoleCommand)Activator.CreateInstance(type);
                    m_RegisteredCommands[type.Name] = commandInstance;
                    var lowerCaseName = commandInstance.CommandName.ToLower();
                    m_CommandNameIndexingDictionary[lowerCaseName] = type.Name;
                }
                else {
                    Debug.LogWarning($"{m_RegisteredCommands[type.Name]} is already registered, skipping {type.FullName}");
                }
            }
        }
    }
}