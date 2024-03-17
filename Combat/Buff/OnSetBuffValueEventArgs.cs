namespace XiheFramework.Combat.Buff {
    public class OnSetBuffValueEventArgs {
        public string buffName;
        public string valueName;
        public object value;

        public OnSetBuffValueEventArgs(string buffName, string valueName, object value) {
            this.buffName = buffName;
            this.valueName = valueName;
            this.value = value;
        }
    }
}