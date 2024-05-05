namespace XiheFramework.Combat.Buff {
    public class OnBuffDestroyEventArgs {
        public uint buffEntityId;
        public string buffName;

        public OnBuffDestroyEventArgs(uint buffEntityId, string buffName) {
            this.buffEntityId = buffEntityId;
            this.buffName = buffName;
        }
    }
}