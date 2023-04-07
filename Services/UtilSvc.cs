using NodeCanvas.Framework;
using UnityEngine;

namespace XiheFramework.Services {
    public static class UtilSvc {
        public static void SetFlowCanvasBlackBoardValue<T>(Blackboard blackboard, string name, T value) {
            blackboard.SetVariableValue(name, value);
        }

        public static T GetFlowCanvasBlackBoardValue<T>(Blackboard blackboard, string name) {
            return blackboard.GetVariableValue<T>(name);
        }

        public static string JoinString(string a, string b) {
            return a + b;
        }

        public static void AddDeltaTime(float input, out float output) {
            output = input + Time.deltaTime;
        }
    }
}