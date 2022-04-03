using JetBrains.Annotations;

namespace XiheFramework {
    public class DebugModule : GameModule {
        public bool showLog = true;

        public void LogInfo(string message) {
            if (showLog) {
                UnityEngine.Debug.Log(message);
            }
        }

        [StringFormatMethod("format")]
        public void LogFormat(string message, params object[] args) {
            if (showLog) {
                UnityEngine.Debug.LogFormat(message, args);
            }
        }

        public void LogWarning(string message) {
            if (showLog) {
                UnityEngine.Debug.LogWarning(message);
            }
        }

        [StringFormatMethod("format")]
        public void LogWarningFormat(string message, params object[] args) {
            if (showLog) {
                UnityEngine.Debug.LogWarningFormat(message, args);
            }
        }

        public void LogError(string message) {
            if (showLog) {
                UnityEngine.Debug.LogError(message);
            }
        }

        [StringFormatMethod("format")]
        public void LogErrorFormat(string message, params object[] args) {
            if (showLog) {
                UnityEngine.Debug.LogErrorFormat(message, args);
            }
        }

        public override void Update() { }

        public override void ShutDown(ShutDownType shutDownType) { }
    }
}