namespace XiheFramework.Combat.Buff {
    public class OnAddBuffEventArgs {
        public uint buffEntityId;
        public string buffName;
        public int deltaStack;

        public OnAddBuffEventArgs(uint buffEntityId, string buffName, int deltaStack) {
            this.buffEntityId = buffEntityId;
            this.buffName = buffName;
            this.deltaStack = deltaStack;
        }
    }
}