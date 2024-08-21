namespace XiheFramework.Combat.Damage.Interfaces {
    public interface IDamageEventArgs {
        uint senderId { get; set; }
        string senderName { get; set; }
        uint receiverId { get; set; }
        string receiverName { get; set; }
    }
}