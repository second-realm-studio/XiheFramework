namespace XiheFramework.Core.Console {
    public interface IDevConsoleCommand {
        public string CommandName { get; }
        public CommandExecutionResult Execute(string[] args, out string message);
    }
}