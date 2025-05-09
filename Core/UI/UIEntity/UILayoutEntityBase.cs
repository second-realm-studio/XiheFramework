using UnityEngine;
using XiheFramework.Core.Entity;

namespace XiheFramework.Core.UI.UIEntity {
    [DisallowMultipleComponent]
    public abstract class UILayoutEntityBase : GameEntityBase {
        public override string GroupName => "UILayoutEntity";
    }
}