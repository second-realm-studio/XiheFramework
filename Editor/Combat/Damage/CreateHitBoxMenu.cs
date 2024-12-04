using UnityEditor;
using UnityEngine;
using XiheFramework.Combat.Damage.HitBox;

namespace XiheFramework.Editor.Combat.Damage {
    public static class CreateHitBoxMenu {
        [MenuItem("GameObject/Xihe/HitBox/RaycastHitBox", false, 10)]
        static void CreateRaycastHitBox(MenuCommand menuCommand) {
            var go = new GameObject("RaycastHitBox", typeof(RaycastHitBox));
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

        [MenuItem("GameObject/Xihe/HitBox/SphereHitBox", false, 10)]
        static void CreateSphereHitBox(MenuCommand menuCommand) {
            var go = new GameObject("SphereHitBox", typeof(SphereHitBox));
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
    }
}