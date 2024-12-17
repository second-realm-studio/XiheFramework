using XiheFramework.Core.Base;

namespace XiheFramework.Runtime {
    public partial class Game {
        public static GameManager Manager { get; internal set; }
        
        public static T GetModule<T>() where T : GameModule {
            return GameManager.GetModule<T>();
        }

        public static void ResetFramework() {
            GameManager.ResetFramework();
        }

        public static string GetGameName() {
            return GameManager.GetGameName();
        }
    }
}