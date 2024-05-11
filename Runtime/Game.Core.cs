using XiheFramework.Core.Audio;
using XiheFramework.Core.Base;
using XiheFramework.Core.Blackboard;
using XiheFramework.Core.Config;
using XiheFramework.Core.Entity;
using XiheFramework.Core.Event;
using XiheFramework.Core.FSM;
using XiheFramework.Core.Localization;
using XiheFramework.Core.LogicTime;
using XiheFramework.Core.Resource;
using XiheFramework.Core.Scene;
using XiheFramework.Core.Serialization;
using XiheFramework.Core.UI;
#if USE_REWIRED
using Rewired;
#endif

namespace XiheFramework.Runtime {
    /// <summary>
    /// shortcut to get all component
    /// </summary>
    public static partial class Game {
        public static AudioModule Audio => GameManager.GetModule<AudioModule>();
        public static EventModule Event => GameManager.GetModule<EventModule>();

        public static BlackboardModule Blackboard => GameManager.GetModule<BlackboardModule>();
        
        public static ConfigModule Config => GameManager.GetModule<ConfigModule>();

        public static SerializationModule Serialization => GameManager.GetModule<SerializationModule>();

        public static ResourceModule Resource => GameManager.GetModule<ResourceModule>();

        public static UIModule UI => GameManager.GetModule<UIModule>();

        public static LocalizationModule Localization => GameManager.GetModule<LocalizationModule>();

        public static StateMachineModule Fsm => GameManager.GetModule<StateMachineModule>();

        public static SceneModule Scene => GameManager.GetModule<SceneModule>();

        public static EntityModule Entity => GameManager.GetModule<EntityModule>();

        public static LogicTimeModule LogicTime => GameManager.GetModule<LogicTimeModule>();

#if USE_REWIRED
        public static Player Input(int playerInputId) => ReInput.players.GetPlayers()[playerInputId];
        public static Player SystemInput => ReInput.players.GetSystemPlayer();
#endif

        public static T GetModule<T>() where T : GameModule {
            return GameManager.GetModule<T>();
        }

        public static void ResetFramework() {
            GameManager.ResetFramework();
        }
    }
}