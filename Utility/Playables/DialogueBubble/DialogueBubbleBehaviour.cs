using System;
using UnityEngine;
using UnityEngine.Playables;

namespace XiheFramework.Utility.Playables.DialogueBubble {
    [Serializable]
    public class DialogueBubbleBehaviour : PlayableBehaviour {
        public string text;
        public Color backgroundColor = new(1f, 1f, 1f, 1f);
        public bool isLeft;
    }
}