using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace XiheFramework.Utility.Playables.MaterialSwitcher {
    [Serializable]
    public class MaterialSwitcherClip : PlayableAsset, ITimelineClipAsset {
        public MaterialSwitcherBehaviour template = new();

        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
            var playable = ScriptPlayable<MaterialSwitcherBehaviour>.Create(graph, template);
            return playable;
        }
    }
}