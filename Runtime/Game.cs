using System;
using XiheFramework.Core.Base;

namespace XiheFramework.Runtime {
    public partial class Game {
        public static GameManager Manager { get; internal set; }

        public static void InstantiateGameModule(Type moduleType, Action onInstantiated) {
            GameManager.InstantiatePresetGameModule(moduleType, onInstantiated);
        }

        public static void InstantiateGameModuleAsync(Type moduleType, Action onInstantiated) {
            GameManager.InstantiatePresetGameModuleAsync(moduleType, onInstantiated);
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