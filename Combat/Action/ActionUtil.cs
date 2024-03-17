namespace XiheFramework.Combat.Action {
    public static class ActionUtil {
        public static string GetActionEntityAddress(string actionName) {
            return $"ActionEntity_{actionName}";
        }
    }
}