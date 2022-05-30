using System.Collections.Generic;
using FlowCanvas;
using NodeCanvas.Framework;
using UnityEngine;

namespace XiheFramework {
    public class NpcModule : GameModule {
        public FlowScript eventsActivator;

        private Dictionary<string, NpcBase> m_Npcs = new Dictionary<string, NpcBase>();

        private void Start() {
            var controller = gameObject.AddComponent<FlowScriptController>();
            controller.behaviour = eventsActivator;
            var blackBoard = gameObject.AddComponent<Blackboard>();
            controller.blackboard = blackBoard;
            controller.StartBehaviour();
        }

        public void RegisterNpc(NpcBase npc) {
            if (npc == null) {
                return;
            }

            if (m_Npcs.ContainsKey(npc.internalName)) {
                m_Npcs[npc.internalName] = npc;
            }
            else {
                m_Npcs.Add(npc.internalName, npc);
            }
        }

        public Transform GetNpcTransform(string internalName) {
            return m_Npcs[internalName].transform;
        }

        public override void Update() {
        }

        public override void ShutDown(ShutDownType shutDownType) {
        }
    }
}