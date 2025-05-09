using XiheFramework.Runtime;

namespace XiheFramework.Core.UI.UIEntity {
    public class UIPopEntityBase : UILayoutEntityBase {
        public void Close() {
            Game.UI.ClosePop(EntityId);
        }
    }
}