using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace XiheFramework.Utility.Playables.LightControl {
    [Serializable]
    public class LightControlClip : PlayableAsset, ITimelineClipAsset {
        public LightControlBehaviour template = new();

        public ClipCaps clipCaps => ClipCaps.Blending;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
            var playable = ScriptPlayable<LightControlBehaviour>.Create(graph, template);
            return playable;
        }
    }
}