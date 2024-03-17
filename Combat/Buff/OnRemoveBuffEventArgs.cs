namespace XiheFramework.Combat.Buff {
    public class OnRemoveBuffEventArgs {
        public string buffName;
        public int stack;

        public OnRemoveBuffEventArgs(string buffName, int stack) {
            this.buffName = buffName;
            this.stack = stack;
        }
    }
}