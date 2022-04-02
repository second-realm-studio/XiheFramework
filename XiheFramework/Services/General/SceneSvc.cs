using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XiheFramework {
    public class SceneSvc {
        public static void LoadScene(string loadSceneName) {
            Game.Scene.LoadScene(loadSceneName);
        }
    }
}