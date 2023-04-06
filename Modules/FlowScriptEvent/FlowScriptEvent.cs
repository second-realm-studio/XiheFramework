using System;
using System.Collections;
using System.Collections.Generic;
using FlowCanvas;
using NodeCanvas.Framework;
using UnityEngine;

namespace XiheFramework {
    public class FlowScriptEvent : MonoBehaviour {
        public int priority;
        public string eventName;
        public FlowScript canvas;
        public bool playOnAwake;

        [TextArea]
        public string description;

        private void Start() {
            if (playOnAwake) {
                Play();
            }
        }


        public void Play() {
            var controller = gameObject.AddComponent<FlowScriptController>();
            controller.behaviour = canvas;
            if (gameObject.TryGetComponent<Blackboard>(out var blackBoard)) {
                controller.blackboard = blackBoard;
            }
            else {
                controller.blackboard = gameObject.AddComponent<Blackboard>();
            }

            controller.StartBehaviour();
        }
    }
}