namespace XiheFramework {
    public static class FlowEventSvc {
        public static void SetEventUsed(string eventName, bool used) {
            Game.Blackboard.SetData("FlowEvent.Used." + eventName, used);
        }
    
        public static bool IsEventUsed(string eventName) {
            return Game.Blackboard.GetData<bool>("FlowEvent.Used." + eventName);
        }
    }
}
