using UnityEngine;
using XiheFramework.Core.Entity;

namespace XiheFramework.Combat.Base {
    public abstract class CombatEntityBase : GameEntity {
        public abstract string entityName { get; }

        protected float timeScale = 1f;
        protected float scaledDeltaTime => Time.deltaTime * timeScale;

        private string m_OnTimeScaleEventId;

        protected override void Start() {
            base.Start();

            timeScale = XiheFramework.Entry.Game.LogicTime.GlobalTimeScale;
            m_OnTimeScaleEventId = XiheFramework.Entry.Game.Event.Subscribe(XiheFramework.Entry.Game.LogicTime.onSetGlobalTimeScaleEventName, OnSetGlobalTimeScale);
        }

        protected virtual void OnSetGlobalTimeScale(object sender, object e) {
            timeScale = (float)e;
        }

        protected virtual void OnDestroy() {
            if (XiheFramework.Entry.Game.Event) {
                XiheFramework.Entry.Game.Event.Unsubscribe(XiheFramework.Entry.Game.LogicTime.onSetGlobalTimeScaleEventName, m_OnTimeScaleEventId);
            }
        }
    }
}