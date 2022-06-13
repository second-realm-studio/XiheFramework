using UnityEngine;

namespace XiheFramework {
    public class WeatherModule : GameModule {
        public Light sun;

        [Range(1, 12)]
        public int month = 0;

        [Range(1, 30)]
        public int day = 0;

        [Range(1, 24)]
        public int hour = 0;

        [Range(1, 60)]
        public int minute = 0;

        [Range(1, 60)]
        public int second = 0;

        private Vector3 m_TargetEuler;
        private Transform m_CachedTransform;

        public void SetDate(int m, int d) {
            this.month = m;
            this.day = d;
        }

        public void SetDate(float t) {
            t %= 1f;

            var total = t * 360;
            month = (int) (total / 30f);
            day = (int) ((total - month * 30f) / 60f);
        }

        public void SetTime(int h, int m, int s) {
            this.hour = h;
            this.minute = m;
            this.second = s;
        }

        public void SetTime(float t) {
            t %= 1f;
            var total = t * 86400;

            hour = (int) (total / 24f);
            minute = (int) ((total - hour * 3600f) / 60f);
            second = (int) ((total - hour * 3600f - minute * 60f) / 60f);
        }

        public override void Setup() {
            base.Setup();

            m_CachedTransform = sun.transform;
        }

        public override void Update() {
            UpdateTargetEuler();
            // m_CachedTransform.localRotation = Quaternion.Euler(m_TargetEuler.x, 0f, 0f);
            // m_CachedTransform.rotation = Quaternion.Euler(m_CachedTransform.rotation.x, m_TargetEuler.y, 0f);
            m_CachedTransform.rotation = Quaternion.Euler(m_TargetEuler);
        }

        public override void ShutDown(ShutDownType shutDownType) {
        }

        void UpdateTargetEuler() {
            var yt = (month * 30 + day) / 360f;
            var dt = (hour * 3600 + minute * 60 + second) / 86400f;

            m_TargetEuler.x = (dt * 360f - 90f) % 360f;
            m_TargetEuler.y = (yt * 360f) % 360f;
        }
    }
}