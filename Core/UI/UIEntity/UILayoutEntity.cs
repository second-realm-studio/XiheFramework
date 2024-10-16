using UnityEngine;
using XiheFramework.Core.Entity;

namespace XiheFramework.Core.UI.UIEntity {
    [DisallowMultipleComponent]
    public abstract class UILayoutEntity : GameEntity {
        public override string GroupName => "UILayoutEntity";
    }
}