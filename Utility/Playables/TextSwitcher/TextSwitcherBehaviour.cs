using System;
using UnityEngine;
using UnityEngine.Playables;

namespace XiheFramework.Utility.Playables.TextSwitcher {
    [Serializable]
    public class TextSwitcherBehaviour : PlayableBehaviour {
        public Color color = Color.white;
        public int fontSize = 14;
        [Range(0.1f, 50f)] public float speed = 20f;
        public string text;

        public bool localized;

        public float progression = 1f;
    }
}