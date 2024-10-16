namespace XiheFramework.Combat.Buff {
    public struct OnAddBuffEventArgs {
        public uint buffEntityId;
        public string buffAddress;
        public int deltaStack;

        public OnAddBuffEventArgs(uint buffEntityId, string buffAddress, int deltaStack) {
            this.buffEntityId = buffEntityId;
            this.buffAddress = buffAddress;
            this.deltaStack = deltaStack;
        }
    }
}