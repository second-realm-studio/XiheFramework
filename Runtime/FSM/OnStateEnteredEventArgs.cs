namespace XiheFramework.Runtime.FSM {
    public struct OnStateEnteredEventArgs {
        public string fsmName;
        public string enteredStateName;

        public OnStateEnteredEventArgs(string fsmName, string enteredStateName) {
            this.fsmName = fsmName;
            this.enteredStateName = enteredStateName;
        }
    }
}