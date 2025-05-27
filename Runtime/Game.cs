using System;
using XiheFramework.Runtime.Base;

namespace XiheFramework.Runtime {
    public partial class Game {
        public static GameManager Manager { get; internal set; }

        public static void InstantiateGameModule<T>(Action onInstantiated = null) where T : GameModuleBase {
            GameManager.InstantiatePresetGameModule<T>(onInstantiated);
        }

        public static void InstantiateGameModuleAsync<T>(Action onInstantiated = null) where T : GameModuleBase {
            GameManager.InstantiatePresetGameModuleAsync<T>(onInstantiated);
        }

        public static T GetModule<T>() where T : GameModuleBase {
            return GameManager.GetModule<T>();
        }

        public static void DestroyFramework() {
            GameManager.DestroyFramework();
        }

        public static string GetGameName() {
            return GameManager.GameName;
        }
    }
}