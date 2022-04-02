using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class MaterialSwitcherClip : PlayableAsset, ITimelineClipAsset {
    public MaterialSwitcherBehaviour template = new MaterialSwitcherBehaviour();

    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
        var playable = ScriptPlayable<MaterialSwitcherBehaviour>.Create(graph, template);
        return playable;
    }
}