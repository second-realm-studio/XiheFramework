﻿namespace XiheFramework {
    /// <summary>
    /// shortcut to get all component
    /// </summary>
    public static partial class Game {
        public static AudioModule Audio => GameManager.GetModule<AudioModule>();

        public static EventModule Event => GameManager.GetModule<EventModule>();
        
        public static InputModule Input => GameManager.GetModule<InputModule>();

        public static BlackboardModule Blackboard => GameManager.GetModule<BlackboardModule>();

        public static SerializationModule Serialization => GameManager.GetModule<SerializationModule>();

        public static UIModule UI => GameManager.GetModule<UIModule>();

        public static LocalizationModule Localization => GameManager.GetModule<LocalizationModule>();

        public static StateMachineModule Fsm => GameManager.GetModule<StateMachineModule>();

        public static FlowScriptEventModule FlowEvent => GameManager.GetModule<FlowScriptEventModule>();

        public static EntityModule Entity => GameManager.GetModule<EntityModule>();

        public static T GetModule<T>() where T : GameModule {
            return GameManager.GetModule<T>();
        }
    }
}