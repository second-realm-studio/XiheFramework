namespace XiheFramework.Core.FSM {
    public struct OnStateExitedEventArgs {
        public string fsmName;
        public string exitedStateName;

        public OnStateExitedEventArgs(string fsmName, string exitedStateName) {
            this.fsmName = fsmName;
            this.exitedStateName = exitedStateName;
        }
    }
}