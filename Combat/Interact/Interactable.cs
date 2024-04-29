using System;
using UnityEngine;
using XiheFramework.Combat.Base;

namespace XiheFramework.Combat.Interact {
    public class Interactable : MonoBehaviour {
        private Action<CombatEntity> m_OnInteract;

        private void Start() {
            GameCombat.Interact.RegisterInteractableObject(this);
        }

        public void Interact(CombatEntity entity) {
            m_OnInteract?.Invoke(entity);
        }

        public void AddListener(Action<CombatEntity> callback) {
            m_OnInteract += callback;
        }

        public void RemoveListener(Action<CombatEntity> callback) {
            m_OnInteract -= callback;
        }

        public void ClearListener() {
            m_OnInteract = null;
        }

        private void OnDestroy() {
            if (GameCombat.Interact) {
                GameCombat.Interact.UnregisterInteractableObject(this);
            }
        }
    }
}