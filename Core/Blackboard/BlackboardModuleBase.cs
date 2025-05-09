using XiheFramework.Core.Base;

namespace XiheFramework.Core.Blackboard {
    public abstract class BlackboardModuleBase : GameModuleBase, IBlackboardModule {
        public abstract void CreateBlackboard<T>(string blackboardName) where T : IBlackboard, new();
        public abstract T GetBlackboard<T>(string blackboardName) where T : IBlackboard;
        public abstract IBlackboard GetBlackboard(string blackboardName);
        public abstract void ReleaseBlackboard(string blackboardName);
    }
}