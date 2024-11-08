using UnityEngine.SceneManagement;
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
        public static AudioModule Audio { get; internal set; }
        public static EventModule Event { get; internal set; }
        public static BlackboardModule Blackboard { get; internal set; }
        public static ConfigModule Config { get; internal set; }
        public static ResourceModule Resource { get; internal set; }
        public static UIModule UI { get; internal set; }
        public static StateMachineModule Fsm { get; internal set; }
        public static SceneModule Scene { get; internal set; }
        public static EntityModule Entity { get; internal set; }
        public static LogicTimeModule LogicTime { get; internal set; }

#if USE_REWIRED
        public static Rewired.Player Input(int playerInputId) => ReInput.players.GetPlayers()[playerInputId];
        public static Rewired.Player SystemInput => ReInput.players.GetSystemPlayer();
#endif

        public static T GetModule<T>() where T : GameModule {
            return GameManager.GetModule<T>();
        }

        public static void ResetFramework() {
            GameManager.ResetFramework();
        }
    }
}