using System.Collections.Generic;
using UnityEngine;

namespace XiheFramework {
    public class NpcInteractUIBehaviour : UIBehaviour {
        public NpcInteractUI template;

        public Vector2 offset;

        private Dictionary<Transform, NpcInteractUI> m_NpcUIs = new Dictionary<Transform, NpcInteractUI>();

        public override void Start() {
            base.Start();

            if (template == null) {
                template = Game.Blackboard.GetData<NpcInteractUI>("NpcInteractUI.Template");
            }

            Game.Event.Subscribe("OnNpcInteractUIActivated", OnNpcInteractUIActivated);
            Game.Event.Subscribe("OnNpcInteractUIUnactivated", OnNpcInteractUIUnactivated);

            //close ui when flow event is invoked
            Game.Event.Subscribe("OnFlowEventInvoked", OnFlowEventInvoked);
        }


        private void Update() {
            UpdatePosition();
        }

        private void OnFlowEventInvoked(object sender, object e) {
            
        }

        private void OnNpcInteractUIActivated(object sender, object e) {
            if (!(sender is string npcName)) {
                return;
            }

            var trans = Game.Npc.GetNpcTransform(npcName);

            AddNpcInteractUI(npcName);
        }

        void AddNpcInteractUI(string sender) {
            var trans = Game.Npc.GetNpcTransform(sender);
            if (m_NpcUIs.ContainsKey(trans)) {
                return;
            }

            var ui = Instantiate(template, Camera.main.WorldToScreenPoint(trans.position), Quaternion.identity, transform);
            var events = Game.Npc.GetNpcInvokableEvents(sender);
            ui.UpdateFlowEventItems(events);

            m_NpcUIs.Add(trans, ui);
        }

        private void OnNpcInteractUIUnactivated(object sender, object e) {
            if (!(sender is string npcName)) {
                return;
            }

            // var trans = Game.Npc.GetNpcTransform(npcName);

            RemoveNpcInteractUI(npcName);
        }

        private void RemoveNpcInteractUI(string npcName) {
            var trans = Game.Npc.GetNpcTransform(npcName);

            // m_NpcUIs[trans].closeAction.Invoke();
            Destroy(m_NpcUIs[trans].gameObject, 0.0f);
            m_NpcUIs.Remove(trans);
        }


        void UpdatePosition() {
            foreach (var trans in m_NpcUIs.Keys) {
                if (Camera.main != null) {
                    if (m_NpcUIs[trans].RectTransform != null) {
                        m_NpcUIs[trans].RectTransform.position = Camera.main.WorldToScreenPoint(trans.position) + (Vector3) offset;
                    }
                }
            }
        }
    }
}