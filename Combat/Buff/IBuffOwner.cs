namespace XiheFramework.Combat.Buff {
    public interface IBuffOwner {
        bool HasBuff(string buffName);
        uint GetBuffEntityId(string buffName);
        void ClearBuff();
    }
}