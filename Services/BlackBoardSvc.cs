using XiheFramework.Modules.Base;
using XiheFramework.Modules.Blackboard;

namespace XiheFramework.Services {
    public static class BlackBoardSvc {
        public static void SetValue<T>(string key, T value, BlackBoardDataType targetType) {
            Game.Blackboard.SetData(key, value, targetType);
        }

        public static T GetValue<T>(string key) {
            return Game.Blackboard.GetData<T>(key);
        }
    }
}