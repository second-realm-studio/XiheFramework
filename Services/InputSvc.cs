using UnityEngine;
using XiheFramework.Modules.Base;

namespace XiheFramework.Services {
    public static class InputSvc {
        public static void AssignKeyMap(string action, KeyCode key) {
            Game.Input.SetKeycode(action, key);
        }

        //0:down 1:hold 2:up
        // private static bool GetKey(int type, KeyActionTypes keyAction) {
        //     if (!Game.Input.ContainsKeyAction(keyAction)) {
        //         Debug.LogError("Action key " + keyAction.ToString() + " doesn't exist");
        //         return false;
        //     }
        //
        //     var k = Game.Input.GetKeyCode(keyAction);
        //
        //     switch (type) {
        //         case 0:
        //             return UnityEngine.Input.GetKeyDown(k);
        //         case 1:
        //             return UnityEngine.Input.GetKey(k);
        //         case 2:
        //             return UnityEngine.Input.GetKeyUp(k);
        //         default:
        //             return false;
        //     }
        // }

        public static bool GetKeyDown(string keyAction) {
            return Game.Input.GetKeyDown(keyAction);
        }

        public static bool GetKeyHold(string keyAction) {
            return Game.Input.GetKey(keyAction);
        }

        public static bool GetKeyUp(string keyAction) {
            return Game.Input.GetKeyUp(keyAction);
        }

        public static bool GetAnyKeyDown() {
            return Game.Input.AnyKeyDown();
        }

        public static bool GetAnyKey() {
            return Game.Input.AnyKey();
        }

        public static Vector3 GetGlobalMoveAxis(Transform playerTransform, Transform playerCamera) {
            var h = Input.GetAxis("Horizontal"); //-1 ~ 1
            var v = Input.GetAxis("Vertical");

            var camForward = Vector3.Scale(playerCamera.forward, new Vector3(1, 0, 1)).normalized;
            var twoAxis = v * camForward + h * playerCamera.right.normalized;

            return twoAxis;
        }

        public static Vector2 GetMouseAxis() {
            return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }
    }
}