using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DialogueBubbleClip : PlayableAsset, ITimelineClipAsset {
    
    public DialogueBubbleBehaviour template = new DialogueBubbleBehaviour();

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
        var playable = ScriptPlayable<DialogueBubbleBehaviour>.Create(graph, template);
        return playable;
    }

    public ClipCaps clipCaps => ClipCaps.None;
}