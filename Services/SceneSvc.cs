using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace XiheFramework {
    public class SceneSvc {
        public static void LoadScene(string loadSceneName) {
            SceneManager.LoadScene(loadSceneName);
        }
    }
}