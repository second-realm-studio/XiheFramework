using System;
using UnityEngine;
using UnityEngine.Playables;

namespace XiheFramework.Utility.Playables.MaterialSwitcher {
    [Serializable]
    public class MaterialSwitcherBehaviour : PlayableBehaviour {
        public Material material;
        public Color color;
    }
}