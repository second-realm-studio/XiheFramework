namespace XiheFramework.Core.Blackboard {
    public interface IBlackboard {
        string BlackboardName { get; }
        void OnCreated();
        void OnRelease();
    }
}