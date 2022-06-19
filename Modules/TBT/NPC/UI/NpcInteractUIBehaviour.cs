using UnityEngine;

namespace XiheFramework {
    public class NpcInteractUIBehaviour : UIBehaviour {
        public NpcInteractUI template;

        public Vector2 offset;

        // private Dictionary<Transform, NpcInteractUI> m_NpcUIs = new Dictionary<Transform, NpcInteractUI>();
        private string m_CurrentOwner;
        private NpcInteractUI m_CurrentNpcUi;

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
            RemoveNpcInteractUI();
        }

        private void OnNpcInteractUIActivated(object sender, object e) {
            if (!(sender is string npcName)) {
                return;
            }

            // var trans = Game.Npc.GetNpcTransform(sender);

            AddNpcInteractUI(npcName);
        }

        void AddNpcInteractUI(string sender) {
            // var trans = Game.Npc.GetNpcTransform(sender);
            if (m_CurrentNpcUi != null) {
                RemoveNpcInteractUI();
            }

            m_CurrentOwner = sender;

            if (Camera.main != null) {
                var ui = Instantiate(template, Camera.main.WorldToScreenPoint(Game.Npc.GetNpcPosition(sender)), Quaternion.identity, transform);
                var events = Game.Npc.GetNpcInvokableEvents(sender);
                ui.UpdateFlowEventItems(events);

                m_CurrentOwner = sender;
                m_CurrentNpcUi = ui;
            }

            // m_NpcUIs.Add(trans, ui);
        }

        private void OnNpcInteractUIUnactivated(object sender, object e) {
            if (!(sender is string npcName)) {
                return;
            }

            // var trans = Game.Npc.GetNpcTransform(npcName);

            RemoveNpcInteractUI();
        }

        private void RemoveNpcInteractUI() {
            if (!string.IsNullOrEmpty(m_CurrentOwner) || m_CurrentNpcUi != null) {
                Destroy(m_CurrentNpcUi.gameObject, 0.5f);
                m_CurrentNpcUi = null;
            }

            // m_NpcUIs[trans].closeAction.Invoke();
        }


        void UpdatePosition() {
            if (Camera.main != null && m_CurrentNpcUi != null) {
                m_CurrentNpcUi.RectTransform.position = Camera.main.WorldToScreenPoint(Game.Npc.GetNpcPosition(m_CurrentOwner)) + (Vector3) offset;
            }

            // foreach (var trans in Keys) {
            //     if (Camera.main != null) {
            //         if (m_NpcUIs[trans].RectTransform != null) {
            //             m_NpcUIs[trans].RectTransform.position = Camera.main.WorldToScreenPoint(trans.position) + (Vector3) offset;
            //         }
            //     }
            // }
        }
    }
}