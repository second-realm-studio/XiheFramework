using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XiheFramework.Core.Base;
using XiheFramework.Runtime;

namespace XiheFramework.Core.Command {
    public class CommandModule : GameModule {
        private readonly Dictionary<string, IExecutable> m_RegisteredCommands = new();

        public void ExecuteCommand(string commandTypeName) {
            if (!m_RegisteredCommands.TryGetValue(commandTypeName, out var command)) {
                Debug.LogError($"Command {commandTypeName} not found");
                return;
            }

            command.Execute();
        }

        protected override void Awake() {
            base.Awake();
            Game.Command = this;
        }

        public override void Setup() {
            base.Setup();

            RegisterAllCommands();
        }

        private void RegisterAllCommands() {
            m_RegisteredCommands.Clear();
            var commandTypes = System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IExecutable).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

            foreach (var type in commandTypes) {
                if (!m_RegisteredCommands.ContainsKey(type.Name)) {
                    m_RegisteredCommands[type.Name] = (IExecutable)Activator.CreateInstance(type);
                }
            }
        }
    }
}