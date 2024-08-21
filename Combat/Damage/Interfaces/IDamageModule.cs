namespace XiheFramework.Combat.Damage.Interfaces {
    public interface IDamageModule {
        string onProcessedDamageEventName { get; }
        void RegisterDamage(IDamageData damageData);
        bool Process(IDamageData damageData, out IDamageEventArgs outputData);
        bool Validate(IDamageData damageData, out string message);
        IDamageEventArgs Calculate(IDamageData damageData);
    }
}