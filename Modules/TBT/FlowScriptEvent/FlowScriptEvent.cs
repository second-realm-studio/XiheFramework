using System;
using System.Collections;
using System.Collections.Generic;
using FlowCanvas;
using NodeCanvas.Framework;
using UnityEngine;

public class FlowScriptEvent : MonoBehaviour {
    public int priority;
    public string eventName;
    public FlowScript canvas;

    [TextArea]
    public string description;

    private void Start() {
        Play();
    }

    public void Play() {
        var controller = gameObject.AddComponent<FlowScriptController>();
        controller.behaviour = canvas;
        var blackBoard = gameObject.AddComponent<Blackboard>();
        controller.blackboard = blackBoard;
        controller.StartBehaviour();
    }
}