using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace XiheFramework.Utility.Playables.DialogueBubble {
    [TrackColor(0.1394896f, 0.4411765f, 0.3413077f)]
    [TrackClipType(typeof(DialogueBubbleClip))]
    [TrackBindingType(typeof(RectTransform))]
    public class DialogueBubbleTrack : TrackAsset {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
            return ScriptPlayable<DialogueBubbleMixerBehaviour>.Create(graph, inputCount);
        }

        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver) {
#if UNITY_EDITOR
            var trackBinding = director.GetGenericBinding(this) as RectTransform;
            if (trackBinding == null)
                return;

            var serializedObject = new SerializedObject(trackBinding);
            var iterator = serializedObject.GetIterator();
            while (iterator.NextVisible(true)) {
                if (iterator.hasVisibleChildren)
                    continue;

                driver.AddFromName<RectTransform>(trackBinding.gameObject, iterator.propertyPath);
            }
#endif
            base.GatherProperties(director, driver);
        }
    }
}