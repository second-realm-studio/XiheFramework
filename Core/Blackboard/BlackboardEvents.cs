namespace XiheFramework.Core.Blackboard {
    public class BlackboardEvents {
        public const string OnBlackboardCreated = "Blackboard.OnCreated";
        public const string OnBlackboardDataChange = "Blackboard.OnDataChange";
        public const string OnBlackboardReleased = "Blackboard.OnReleased";

        public struct OnBlackboardCreatedEventArgs {
            public string blackboardName;
        }

        public struct OnBlackboardDataChangeEventArgs {
            public string blackboardName;
            public string dataName;
            public object oldValue;
            public object newValue;
        }

        public struct OnBlackboardReleasedEventArgs {
            public string blackboardName;
        }
    }
}