namespace XiheFramework.Core.Console {
    public interface IDevConsoleCommand {
        public bool Execute(string[] args, out string message);
    }
}