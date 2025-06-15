namespace XiheFramework.Runtime.Base {
    public enum CoreModulePriority {
        Default = -1,
        CustomModuleDefault = 0,
        Event = -100,
        Blackboard = -90,
        LogicTime = -80,
        Config = -80,
        Resource = -70,
        Fsm = -70,
        Entity = -60,
        UI = -50,
        Scene = -40,
        Console = -40,
        Audio = -40,
    }
}