using UnityEngine;

namespace XiheFramework {
    public class WeatherModule : GameModule {
        public Light sun;

        public void SetTime(uint hour, uint minute, uint second) {
            float t = (hour * 3600f + minute * 60f + second) / 86400f;
            var r = sun.transform.localRotation;
            r = Quaternion.Euler((t / 360f + 90f) % 360f, r.y, r.z);
            sun.transform.localRotation = r;
        }

        public override void Update() {
        }

        public override void ShutDown(ShutDownType shutDownType) {
        }
    }
}