namespace XiheFramework.Core.FSM {
    public struct OnStateEnteredEventArgs {
        public string enteredStateName;
        public OnStateEnteredEventArgs(string enteredStateName) {
            this.enteredStateName = enteredStateName;
        }
    }
}