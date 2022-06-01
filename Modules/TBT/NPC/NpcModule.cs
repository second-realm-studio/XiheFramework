using System.Collections.Generic;
using FlowCanvas;
using NodeCanvas.Framework;
using UnityEngine;
using XiheFramework.Util;

namespace XiheFramework {
    public class NpcModule : GameModule {
        public FlowScript eventsActivator;

        private Dictionary<string, NpcBase> m_Npcs = new Dictionary<string, NpcBase>();

        public MultiDictionary<string, string> m_NpcFlowEvents = new MultiDictionary<string, string>();

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

        public string[] GetNpcInvokableEvents(string npcName) {
            if (m_NpcFlowEvents.ContainsKey(npcName)) {
                return m_NpcFlowEvents[npcName].ToArray();
            }

            return null;
        }

        public Transform GetNpcTransform(string internalName) {
            return m_Npcs[internalName].transform;
        }

        public void ActivateNpcEvent(string internalName, string eventName) {
            if (!m_Npcs.ContainsKey(internalName)) {
                return;
            }

            m_NpcFlowEvents.Add(internalName, eventName);
            //m_Npcs[internalName].AddInvokableEvent(eventName);
        }

        public void DeactivateEvent(string internalName, string eventName) {
            if (!m_Npcs.ContainsKey(internalName)) {
                return;
            }

            m_NpcFlowEvents.Remove(internalName, eventName);
            // m_Npcs[internalName].RemoveInvokableEvent(eventName);
        }

        public override void Update() {
        }

        public override void ShutDown(ShutDownType shutDownType) {
        }
    }
}