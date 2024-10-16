using XiheFramework.Runtime;

namespace XiheFramework.Core.UI.UIEntity {
    public class UIOverlayEntity : UILayoutEntity {
        public void Close() {
            Game.UI.CloseOverlay(EntityId);
        }
    }
}