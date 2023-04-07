using System;
using UnityEngine;
using UnityEngine.Playables;

namespace XiheFramework.Utility.Playables.ImageSwitcher {
    [Serializable]
    public class ImageSwitcherBehaviour : PlayableBehaviour {
        public Sprite image;
        public Color color = new(1f, 1f, 1f, 1f);
    }
}