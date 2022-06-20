using System;
using System.Collections;
using System.Collections.Generic;
using FlowCanvas;
using UnityEngine;

namespace XiheFramework {
    [CreateAssetMenu(menuName = "Xihe/Achievement")]
    public class Achievement : ScriptableObject {
        public string achievementName;
        public string description;
        public bool isHidden;
        public Sprite lockedIcon;
        public string lockedText;//hint
        public Sprite unlockedIcon;
        public string unlockedText;
    }
}