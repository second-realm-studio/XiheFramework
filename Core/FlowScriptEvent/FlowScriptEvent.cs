using FlowCanvas;
using UnityEngine;

namespace XiheFramework.Modules.FlowScriptEvent {
    public class FlowScriptEvent : MonoBehaviour {
        public int priority;
        public string eventName;
        public FlowScript canvas;
        public bool playOnAwake;

        [TextArea] public string description;

        private void Start() {
            if (playOnAwake) Play();
        }


        public void Play() {
            var controller = gameObject.AddComponent<FlowScriptController>();
            controller.behaviour = canvas;
            if (gameObject.TryGetComponent<NodeCanvas.Framework.Blackboard>(out var blackBoard))
                controller.blackboard = blackBoard;
            else
                controller.blackboard = gameObject.AddComponent<NodeCanvas.Framework.Blackboard>();

            controller.StartBehaviour();
        }
    }
}