using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[Serializable]
public class ImageSwitcherBehaviour : PlayableBehaviour {
    public Sprite image;
    public Color color = new Color(1f, 1f, 1f, 1f);
}