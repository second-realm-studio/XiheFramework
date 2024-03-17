using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Core.Base;

namespace XiheFramework.Combat.Interact {
    public class InteractModule : GameModule {
        private List<Interactable> m_CachedInteractableObjects = new List<Interactable>();

        public void RegisterInteractableObject(Interactable interactable) {
            m_CachedInteractableObjects.Add(interactable);
        }

        public void UnregisterInteractableObject(Interactable interactable) {
            m_CachedInteractableObjects.Remove(interactable);
        }

        public Interactable GetClosestInteractableObject(Vector3 position, float maxDistance) {
            var closestDistance = float.MaxValue;
            Interactable closestInteractable = null;
            foreach (var interactable in m_CachedInteractableObjects) {
                var distance = Vector3.Distance(position, interactable.transform.position);
                if (distance < closestDistance && distance <= maxDistance) {
                    closestDistance = distance;
                    closestInteractable = interactable;
                }
            }

            return closestInteractable;
        }
    }
}