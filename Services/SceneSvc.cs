using UnityEngine.SceneManagement;

namespace XiheFramework.Services {
    public class SceneSvc {
        public static void LoadScene(string loadSceneName) {
            SceneManager.LoadScene(loadSceneName);
        }
    }
}