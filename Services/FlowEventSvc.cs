using XiheFramework.Modules.Base;

namespace XiheFramework.Services {
    public static class FlowEventSvc {
        public static void SetEventUsed(string eventName, bool used) {
            Game.Blackboard.SetData("FlowEvent.Used." + eventName, used);
            Game.Event.Invoke("OnFlowEventUsed", eventName, used);
        }

        public static bool IsEventUsed(string eventName) {
            return Game.Blackboard.GetData<bool>("FlowEvent.Used." + eventName);
        }

        public static void StartFlowEvent(string eventName) {
            Game.FlowEvent.StartEvent(eventName);
        }

        public static void DestroyFlowEvent(string eventName, float delay) {
            Game.FlowEvent.DestroyEvent(eventName, delay);
        }
    }
}