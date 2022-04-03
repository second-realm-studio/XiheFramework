using FlowCanvas;
using NodeCanvas.Framework;
using UnityEngine;

namespace XiheFramework.RPG {
    public static class InteractionSvc {
        public static void EnableInteraction() {
            //Rpg.Interact.Enable();
        }

        public static void DisableInteraction() {
            //Rpg.Interact.Disable();
        }

        public static void SpawnInteractableObject(string itemName) {
            //if (!Rpg.Interact) {
            //    Debug.LogWarning("Rpg.Interact is null");
            //    return;
            //}
            //Debug.Log("Rpg.Interact.SpawnManualInteractableObject(itemName)");
            //Rpg.Interact.SpawnManualInteractableObject(itemName);
        }

        public static void Interact() {
            //if (!Rpg.Interact) {
            //    return;
            //}

            //Rpg.Interact.InteractFocusedObject();
        }

        public static void UnregisterNearPlayerObject(InteractableObject interactableObject) {
            //Rpg.Interact.UnRegisterNearPlayerObject(interactableObject);
        }

        /// <summary>
        /// 把物体引用添加到Interaction组件
        /// </summary>
        /// <param name="interactableObject"></param>
        public static void RegisterNearPlayerObject(InteractableObject interactableObject) {
            //Rpg.Interact.RegisterNearPlayerObject(interactableObject);
        }
    }
}