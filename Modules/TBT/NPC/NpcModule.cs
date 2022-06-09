using System.Collections.Generic;
using FlowCanvas;
using NodeCanvas.Framework;
using UnityEngine;
using XiheFramework.Util;

namespace XiheFramework {
    public class NpcModule : GameModule {
        public List<NpcBase> npcCandidates;

        public FlowScript eventsActivator;

        // public int maxInteractCount = 1;
        public MultiDictionary<string, string> m_NpcFlowEvents = new MultiDictionary<string, string>();

        private Dictionary<string, NpcBase> m_Npcs = new Dictionary<string, NpcBase>();
        // private int m_CurrentInteractCount = 0;

        private string m_ActiveNpc; //who is currently interacted by player

        private void Start() {
            var controller = gameObject.AddComponent<FlowScriptController>();
            controller.behaviour = eventsActivator;
            var blackBoard = gameObject.AddComponent<Blackboard>();
            controller.blackboard = blackBoard;
            controller.StartBehaviour();
        }

        public void InstantiateNpc(string npcName) {
            InstantiateNpc(npcName, Vector3.zero, Quaternion.identity, null);
        }

        public void InstantiateNpc(string npcName, Vector3 position, Quaternion rotation, Transform parent) {
            if (!NpcExist(npcName)) {
                foreach (var npcBase in npcCandidates) {
                    if (npcBase.internalName.Equals(npcName)) {
                        Instantiate(npcBase, position, rotation, parent);
                    }
                }
            }
        }

        public void DestroyNpc(string npcName,float delay) {
            if (NpcExist(npcName)) {
                Destroy(m_Npcs[npcName].gameObject,delay);
            }
        }

        public bool AllowInteract() {
            return string.IsNullOrEmpty(m_ActiveNpc);
        }

        public void SetInteractingNpc(string npcName) {
            m_ActiveNpc = npcName;
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

        public bool NpcExist(string npcName) {
            return m_Npcs.ContainsKey(npcName);
        }

        public NpcBase GetNpc(string npcName) {
            if (m_Npcs.ContainsKey(npcName)) {
                return m_Npcs[npcName];
            }
            else {
                return null;
            }
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