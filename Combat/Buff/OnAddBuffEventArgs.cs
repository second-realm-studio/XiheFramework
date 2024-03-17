namespace XiheFramework.Combat.Buff {
    public class OnAddBuffEventArgs {
        public string buffName;
        public int deltaStack;
        public BuffEntity buffEntity;

        public OnAddBuffEventArgs(string buffName, int deltaStack, BuffEntity buffEntity) {
            this.buffName = buffName;
            this.deltaStack = deltaStack;
            this.buffEntity = buffEntity;
        }
    }
}