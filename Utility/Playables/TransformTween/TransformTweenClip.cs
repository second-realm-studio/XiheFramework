using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace XiheFramework.Utility.Playables.TransformTween {
    [Serializable]
    public class TransformTweenClip : PlayableAsset, ITimelineClipAsset {
        public TransformTweenBehaviour template = new();
        public ExposedReference<Transform> startLocation;
        public ExposedReference<Transform> endLocation;

        public ClipCaps clipCaps => ClipCaps.Blending;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
            var playable = ScriptPlayable<TransformTweenBehaviour>.Create(graph, template);
            var clone = playable.GetBehaviour();
            clone.startLocation = startLocation.Resolve(graph.GetResolver());
            clone.endLocation = endLocation.Resolve(graph.GetResolver());
            return playable;
        }
    }
}