using System;
using System.Collections;
using System.Collections.Generic;
using FlowCanvas;
using UnityEngine;

namespace XiheFramework {
    [CreateAssetMenu(menuName = "Xihe/Achievement/Achievement Data")]
    public class AchievementData : ScriptableObject {
        public string achievementName;
        public int id;
        public int progressionLimit;
        public string unlockedDescription;
        public string lockedDescription;
        public bool isHidden;
        public Sprite lockedIcon;
        public Sprite unlockedIcon;
    }
}