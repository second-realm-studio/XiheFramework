using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XiheFramework {
    [Serializable]
    public class AchievementProgress {
        public int id;
        public int progress;

        public AchievementProgress(int id, int progress) {
            this.id = id;
            this.progress = progress;
        }
    }
}