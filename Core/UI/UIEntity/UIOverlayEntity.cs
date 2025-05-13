using XiheFramework.Runtime;

namespace XiheFramework.Core.UI.UIEntity {
    public class UIOverlayEntity : UILayoutEntityBase {
        public override string GroupName => "UIOverlayEntity";

        public void Close() {
            Game.UI.CloseOverlay(EntityId);
        }
    }
}