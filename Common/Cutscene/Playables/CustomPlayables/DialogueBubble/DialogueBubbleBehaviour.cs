using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class DialogueBubbleBehaviour : PlayableBehaviour {
    public string text;
    public Color backgroundColor = new Color(1f, 1f, 1f, 1f);
    public bool isLeft;
}