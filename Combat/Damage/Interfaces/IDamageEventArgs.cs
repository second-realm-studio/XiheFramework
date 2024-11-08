namespace XiheFramework.Combat.Damage.Interfaces {
    public interface IDamageEventArgs {
        uint SenderId { get; set; }
        string SenderName { get; set; }
        uint ReceiverId { get; set; }
        string ReceiverName { get; set; }
    }
}