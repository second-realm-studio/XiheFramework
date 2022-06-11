using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XiheFramework {
    /// <summary>
    /// control day time etc.
    /// </summary>
    public static class WeatherSvc {
        public static void SetTime(uint hour, uint minute, uint second) {
            Game.Weather.SetTime(hour, minute, second);
        }
    }
}