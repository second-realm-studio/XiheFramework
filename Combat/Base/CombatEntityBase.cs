using UnityEngine;
using XiheFramework.Core;
using XiheFramework.Core.Entity;

namespace XiheFramework.Combat.Base {
    public abstract class CombatEntityBase : GameEntity {
        public abstract string entityName { get; }

        protected float timeScale = 1f;
        protected float scaledDeltaTime => Time.deltaTime * timeScale;

        private string m_OnTimeScaleEventId;

        protected override void Start() {
            base.Start();

            timeScale = GameCore.LogicTime.GlobalTimeScale;
            m_OnTimeScaleEventId = GameCore.Event.Subscribe(GameCore.LogicTime.onSetGlobalTimeScaleEventName, OnSetGlobalTimeScale);
        }

        protected virtual void OnSetGlobalTimeScale(object sender, object e) {
            timeScale = (float)e;
        }

        protected virtual void OnDestroy() {
            if (GameCore.Event) {
                GameCore.Event.Unsubscribe(GameCore.LogicTime.onSetGlobalTimeScaleEventName, m_OnTimeScaleEventId);
            }
        }
    }
}