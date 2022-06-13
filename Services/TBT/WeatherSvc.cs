using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XiheFramework {
    /// <summary>
    /// control day time etc.
    /// </summary>
    public static class WeatherSvc {
        public static void SetDate(int month, int day) {
            Game.Weather.SetDate(month, day);
        }

        public static void SetDate(float t) {
            Game.Weather.SetDate(t);
        }

        public static void SetTime(int hour, int minute, int second) {
            Game.Weather.SetTime(hour, minute, second);
        }

        public static void SetTime(float t) {
            Game.Weather.SetTime(t);
        }
    }
}