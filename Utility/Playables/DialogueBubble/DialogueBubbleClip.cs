using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace XiheFramework.Utility.Playables.DialogueBubble {
    public class DialogueBubbleClip : PlayableAsset, ITimelineClipAsset {
        public DialogueBubbleBehaviour template = new();

        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
            var playable = ScriptPlayable<DialogueBubbleBehaviour>.Create(graph, template);
            return playable;
        }
    }
}