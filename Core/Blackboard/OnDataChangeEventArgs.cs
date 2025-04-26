namespace XiheFramework.Core.Blackboard {
    public struct OnDataChangeEventArgs {
        public string dataName;
        public object dataValue;
        public OnDataChangeEventArgs(string dataName, object dataValue) {
            this.dataName = dataName;
            this.dataValue = dataValue;
        }
    }
}