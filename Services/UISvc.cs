using XiheFramework.Modules.Base;

namespace XiheFramework.Services {
    public static class UISvc {
        public static void ActivateUI(string uiName) {
            Game.UI.ActivateUI(uiName);
        }

        public static void UnactivateUI(string uiName) {
            Game.UI.UnactivateUI(uiName);
        }
    }
}