using System;
using System.Collections.Generic;
using System.Linq;
using XiheFramework.Core.Base;
using XiheFramework.Runtime;

namespace XiheFramework.Core.Console {
    public class ConsoleModule : GameModuleBase {
        public override int Priority => 0;

        private readonly Dictionary<string, IDevConsoleCommand> m_RegisteredCommands = new();

        public bool ExecuteCommand(string commandString, out string message) {
            var parts = commandString.Split(' ');
            var commandName = parts[0];
            if (!m_RegisteredCommands.TryGetValue(commandName, out var command)) {
                message = $"Command {commandName} not found";
                return false;
            }

            var args = new string[parts.Length - 1];
            Array.Copy(parts, 1, args, 0, args.Length);

            return command.Execute(args, out message);
        }

        public string[] GetAvailableCommandNames() {
            return m_RegisteredCommands.Keys.ToArray();
        }

        protected override void OnInstantiated() {
            RegisterAllCommands();
            Game.Command = this;
        }

        private void RegisterAllCommands() {
            m_RegisteredCommands.Clear();
            var commandTypes = System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IDevConsoleCommand).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

            foreach (var type in commandTypes) {
                if (!m_RegisteredCommands.ContainsKey(type.Name)) {
                    m_RegisteredCommands[type.Name] = (IDevConsoleCommand)Activator.CreateInstance(type);
                }
            }
        }
    }
}