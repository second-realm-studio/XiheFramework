namespace XiheFramework.Combat.Buff {
    public static class BuffUtil {
        public static string GetBuffEntityAddress(string buffName) {
            return $"BuffEntity_{buffName}";
        }
    }
}