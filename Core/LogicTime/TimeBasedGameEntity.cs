using UnityEngine;
using XiheFramework.Core.Entity;
using XiheFramework.Runtime;

namespace XiheFramework.Core.LogicTime {
    public abstract class TimeBasedGameEntity : GameEntity {

        protected float timeScale = 1f;
        protected float ScaledDeltaTime => Time.deltaTime * timeScale;

        private string m_OnTimeScaleEventId;

        public override void OnInitCallback() {
            timeScale = Game.LogicTime.GlobalTimeScale;
            m_OnTimeScaleEventId = Game.Event.Subscribe(Game.LogicTime.onSetGlobalTimeScaleEventName, OnSetGlobalTimeScale);
        }

        public override void OnDestroyCallback() {
            Game.Event.Unsubscribe(Game.LogicTime.onSetGlobalTimeScaleEventName, m_OnTimeScaleEventId);
        }

        protected virtual void OnSetGlobalTimeScale(object sender, object e) {
            timeScale = (float)e;
        }
    }
}