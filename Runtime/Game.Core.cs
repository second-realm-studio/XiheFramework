using XiheFramework.Core.Audio;
using XiheFramework.Core.Base;
using XiheFramework.Core.Blackboard;
using XiheFramework.Core.Config;
using XiheFramework.Core.Console;
using XiheFramework.Core.Entity;
using XiheFramework.Core.Event;
using XiheFramework.Core.FSM;
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
        public static AudioModuleBase Audio { get; internal set; }
        public static EventModule Event { get; internal set; }
        public static BlackboardModule Blackboard { get; internal set; }
        public static T GetBlackboard<T>(string blackboardName) where T : IBlackboard => Blackboard.GetBlackboard<T>(blackboardName);
        public static ConsoleModule Console { get; internal set; }
        public static void LogMessage(string message)=> Console.LogMessage(message);
        public static void LogWarning(string message)=> Console.LogWarning(message);
        public static void LogError(string message)=> Console.LogError(message);
        public static ConfigModule Config { get; internal set; }
        public static ResourceModule Resource { get; internal set; }
        public static UIModule UI { get; internal set; }
        public static ISerializationModule Serialization { get; internal set; }
        public static StateMachineModule Fsm { get; internal set; }
        public static SceneModule Scene { get; internal set; }
        public static IEntityModule Entity { get; internal set; }
        public static LogicTimeModule LogicTime { get; internal set; }

#if USE_REWIRED
        public static Rewired.Player Input(int playerInputId) => ReInput.players.GetPlayers()[playerInputId];
        public static Rewired.Player SystemInput => ReInput.players.GetSystemPlayer();
#endif
    }
}