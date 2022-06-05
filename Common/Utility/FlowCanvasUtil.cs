using FlowCanvas;
using NodeCanvas.Framework;
using UnityEngine;
using XiheFramework;

public static class FlowCanvasUtil {
    public static void StartFlowBehaviour(GameObject gameObject, FlowScript flowScript) {
        var controller = gameObject.AddComponent<FlowScriptController>();
        controller.behaviour = flowScript;
        var blackBoard = gameObject.GetOrAddComponent<Blackboard>();
        controller.blackboard = blackBoard;
        controller.StartBehaviour();
    }
}