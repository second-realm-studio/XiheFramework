using FlowCanvas;
using NodeCanvas.Framework;
using UnityEngine;

namespace XiheFramework.Utility {
    public static class FlowCanvasUtil {
        public static void StartFlowBehaviour(GameObject gameObject, FlowScript flowScript) {
            var controller = gameObject.AddComponent<FlowScriptController>();
            controller.behaviour = flowScript;
            var blackBoard = gameObject.GetOrAddComponent<Blackboard>();
            controller.blackboard = blackBoard;
            controller.StartBehaviour();
        }
    }
}