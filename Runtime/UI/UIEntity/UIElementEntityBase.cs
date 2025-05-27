using UnityEngine;
using XiheFramework.Runtime.Entity;

namespace XiheFramework.Runtime.UI.UIEntity {
    public abstract class UIElementEntityBase : GameEntityBase {
        public override string GroupName => "UIElementEntity";
        
        public override void OnInitCallback() {
            base.OnInitCallback();

            if (transform is not RectTransform) {
                gameObject.AddComponent<RectTransform>();
            }
        }
    }
}