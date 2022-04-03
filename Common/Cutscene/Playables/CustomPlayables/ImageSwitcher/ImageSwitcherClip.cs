using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class ImageSwitcherClip : PlayableAsset, ITimelineClipAsset {
    public ImageSwitcherBehaviour template = new ImageSwitcherBehaviour();

    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
        var playable = ScriptPlayable<ImageSwitcherBehaviour>.Create(graph, template);
        return playable;
    }
}