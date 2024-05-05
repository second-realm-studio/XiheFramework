namespace XiheFramework.Combat.Buff {
    public class OnBuffCreateEventArgs {
        public uint buffEntityId;
        public string buffName;

        public OnBuffCreateEventArgs(uint buffEntityId, string buffName) {
            this.buffEntityId = buffEntityId;
            this.buffName = buffName;
        }
    }
}