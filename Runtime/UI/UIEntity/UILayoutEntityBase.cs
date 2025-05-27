using UnityEngine;
using XiheFramework.Runtime.Entity;

namespace XiheFramework.Runtime.UI.UIEntity {
    [DisallowMultipleComponent]
    public abstract class UILayoutEntityBase : GameEntityBase {
        public override string GroupName => "UILayoutEntity";
    }
}