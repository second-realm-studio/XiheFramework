using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace XiheFramework.Utility.Playables.TextSwitcher {
    [Serializable]
    public class TextSwitcherClip : PlayableAsset, ITimelineClipAsset {
        public TextSwitcherBehaviour template = new();

        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
            var playable = ScriptPlayable<TextSwitcherBehaviour>.Create(graph, template);
            return playable;
        }
    }
}