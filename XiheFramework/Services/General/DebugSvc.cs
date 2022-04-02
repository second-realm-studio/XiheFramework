namespace XiheFramework {
    public static class DebugSvc {
        public static void Log(string message) => Game.Log.LogInfo(message);

        public static void Warning(string message) => Game.Log.LogWarning(message);

        public static void Error(string message) => Game.Log.LogError(message);
    }
}