using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[Serializable]
public class EventEmitterBehaviour : PlayableBehaviour {
    public string eventName;
    public string argument;
}