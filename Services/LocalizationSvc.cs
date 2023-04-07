using XiheFramework.Modules.Base;

namespace XiheFramework.Services {
    public class LocalizationSvc {
        public static string GetValue(string key) {
            return Game.Localization.GetValue(key);
        }
    }
}