namespace XiheFramework.Core.FSM {
    public struct OnStateExitedEventArgs {
        public string exitedStateName;

        public OnStateExitedEventArgs(string exitedStateName) {
            this.exitedStateName = exitedStateName;
        }
    }
}