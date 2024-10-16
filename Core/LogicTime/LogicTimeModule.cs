using UnityEngine;
using XiheFramework.Core.Base;
using XiheFramework.Runtime;

namespace XiheFramework.Core.LogicTime {
    public class LogicTimeModule : GameModule {
        public readonly string onSetGlobalTimeScaleEventName = "Time.OnSetGlobalTimeScale";

        public float defaultTimeScale = 1f;

        public float GlobalTimeScale {
            get => m_GlobalTimeScale;
            private set => m_GlobalTimeScale = Mathf.Max(0, value);
        }

        public float ScaledDeltaTime => Time.unscaledDeltaTime * GlobalTimeScale;

        private float m_GlobalTimeScale;
        private int m_Duration;
        private int m_Timer;
        private static readonly int TimeScalePropertyID = Shader.PropertyToID("_GlobalTimeScale");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeScale"></param>
        /// <param name="duration"> duration in frames </param>
        /// <param name="setUnityTimeScale"> if true, will affect Time.timeScale </param>
        public void SetGlobalTimeScaleInFrame(float timeScale, int duration, bool setUnityTimeScale = false) {
            var oldTimeScale = GlobalTimeScale;
            GlobalTimeScale = timeScale;
            m_Duration = duration;
            var args = new OnSetGlobalTimeScaleEventArgs(timeScale, oldTimeScale, duration);
            Game.Event.Invoke(onSetGlobalTimeScaleEventName, null, args);
            if (setUnityTimeScale) {
                Time.timeScale = timeScale;
            }

            Shader.SetGlobalFloat(TimeScalePropertyID, timeScale);
        }

        public void SetGlobalTimeScaleInSecond(float timeScale, float duration, bool setUnityTimeScale = false) {
            SetGlobalTimeScaleInFrame(timeScale, (int)(duration / Time.deltaTime), setUnityTimeScale);
        }

        public void SetGlobalTimeScalePermanent(float timeScale, bool setUnityTimeScale = false) {
            SetGlobalTimeScaleInFrame(timeScale, 0, setUnityTimeScale);
        }

        public override void OnUpdate() {
            if (m_Duration <= 0f) {
                return;
            }

            if (m_Timer < m_Duration) {
                m_Timer += 1;
                return;
            }

            //end of slow down
            GlobalTimeScale = defaultTimeScale;
            Shader.SetGlobalFloat(TimeScalePropertyID, defaultTimeScale);
            Time.timeScale = defaultTimeScale;
            Game.Event.Invoke(onSetGlobalTimeScaleEventName, null, defaultTimeScale);
            m_Duration = 0;
            m_Timer = 0;
        }

        public override void Setup() {
            GlobalTimeScale = defaultTimeScale;
            Time.timeScale = defaultTimeScale;
            Shader.SetGlobalFloat(TimeScalePropertyID, defaultTimeScale);
        }

        public override void OnLateStart() {
            GlobalTimeScale = defaultTimeScale;
        }

        protected override void Awake() {
            base.Awake();

            Game.LogicTime = this;
        }
    }
}