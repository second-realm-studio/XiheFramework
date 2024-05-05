namespace XiheFramework.Combat.Buff {
    public class OnRemoveBuffEventArgs {
        public uint buffEntityId;
        public string buffName;
        public int originalStack;

        /// <summary>
        /// can be negative to keep the stack change info
        /// </summary>
        public int newStack;

        public OnRemoveBuffEventArgs(uint buffEntityId, string buffName, int originalStack, int newStack) {
            this.buffEntityId = buffEntityId;
            this.buffName = buffName;
            this.originalStack = originalStack;
            this.newStack = newStack;
        }
    }
}