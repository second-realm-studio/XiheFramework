using System.Collections.Generic;
using UnityEngine;

namespace XiheFramework.Core.Config {
    public class ConfigData {
        [SerializeField]
        public List<ConfigEntry> entries = new List<ConfigEntry>();
    }
}