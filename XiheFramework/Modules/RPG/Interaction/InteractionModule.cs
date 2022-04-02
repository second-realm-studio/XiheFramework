using System;
using System.Collections.Generic;
using UnityEngine;

namespace XiheFramework {
    public class InteractionModule : GameModule {
        private readonly List<InteractableObject> m_NearPlayerObjects = new List<InteractableObject>();

        private List<InteractableObject> m_ObjectInTheScene = new List<InteractableObject>();
        
        private InteractableObject m_FocusObject;

        private bool m_Interactable;

        private void Start() {
            m_Interactable = true;

            //Game.Blackboard.SetData(Keywords.VAR_NearPlayerObjects, new List<InteractableObject>(), BlackBoardDataType.Runtime);
        }

        public override void Update() {
            if (m_Interactable) {
                if (m_NearPlayerObjects.Count != 0) {
                    m_FocusObject = FindFocusInteractableObject();
                }
                else {
                    m_FocusObject = null;
                }
            }

            var screenPos = Vector3.one * -20f;
            if (m_FocusObject != null) {
                var position = m_FocusObject.transform.position;
                screenPos = Camera.main.WorldToScreenPoint(position + new Vector3(0, 1f, 0));
                Game.Blackboard.SetData("FocusObjectPosition", position, BlackBoardDataType.Runtime);
            }

            Game.Blackboard.SetData("FocusObjectScreenPosition", screenPos, BlackBoardDataType.Runtime);
        }

        public override void ShutDown(ShutDownType shutDownType) {
            m_FocusObject = null;
            m_NearPlayerObjects.Clear();
        }

        private InteractableObject FindFocusInteractableObject() {
            int index = 0;
            //float smallest = Vector3.Angle(m_NearPlayerObjects[0].transform.position - transform.position, transform.forward);
            float smallest = float.MaxValue;
            for (int i = 0; i < m_NearPlayerObjects.Count; i++) {
                if (!m_NearPlayerObjects[i].gameObject.activeSelf)
                    continue;
                float angle = Vector3.Angle(m_NearPlayerObjects[i].transform.position - transform.position, transform.forward);
                if (!(angle < smallest)) continue;
                smallest = angle;
                index = i;
            }

            return m_NearPlayerObjects[index];
        }

        public bool IsItemExisted(string itemName) {
            if (Game.Blackboard.ContainsKey(itemName)) {
                return Game.Blackboard.GetData<bool>(itemName);
            }

            return false;
        }

        public void RegisterNearPlayerObject(InteractableObject obj) {
            if (!m_NearPlayerObjects.Contains(obj)) m_NearPlayerObjects.Add(obj);
        }

        public void UnRegisterNearPlayerObject(InteractableObject obj) {
            if (m_NearPlayerObjects.Contains(obj)) m_NearPlayerObjects.Remove(obj);
        }

        public void InteractFocusedObject() {
            if (!m_Interactable) {
                return;
            }

            if (m_FocusObject == null) {
                return;
            }

            m_FocusObject.Interact();
        }

        public void Enable() {
            m_Interactable = true;
        }

        public void Disable() {
            m_Interactable = false;
            m_FocusObject = null;
        }

        public void ClearFocusObject() {
            m_FocusObject = null;
        }

        private void OnDrawGizmos() {
            if (m_FocusObject == null) {
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(m_FocusObject.transform.position + Vector3.up, 0.1f);
        }
    }
}