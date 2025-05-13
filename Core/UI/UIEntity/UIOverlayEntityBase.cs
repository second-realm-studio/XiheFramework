using XiheFramework.Runtime;

namespace XiheFramework.Core.UI.UIEntity {
    public class UIOverlayEntityBase : UILayoutEntityBase {
        public override string GroupName => "UIOverlayEntity";

        public void Close() {
            Game.UI.CloseOverlay(EntityId);
        }
    }
}