using XiheFramework.Runtime;

namespace XiheFramework.Core.UI.UIEntity {
    public class UIPopEntity : UILayoutEntity {
        public void Close() {
            Game.UI.ClosePop(EntityId);
        }
    }
}