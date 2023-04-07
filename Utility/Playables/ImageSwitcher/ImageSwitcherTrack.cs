using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace XiheFramework.Utility.Playables.ImageSwitcher {
    [TrackColor(0.1394896f, 0.4411765f, 0.3413077f)]
    [TrackClipType(typeof(ImageSwitcherClip))]
    [TrackBindingType(typeof(Image))]
    public class ImageSwitcherTrack : TrackAsset {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
            return ScriptPlayable<ImageSwitcherMixerBehaviour>.Create(graph, inputCount);
        }

        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver) {
#if UNITY_EDITOR
            var trackBinding = director.GetGenericBinding(this) as Image;
            if (trackBinding == null)
                return;

            var serializedObject = new SerializedObject(trackBinding);
            var iterator = serializedObject.GetIterator();
            while (iterator.NextVisible(true)) {
                if (iterator.hasVisibleChildren)
                    continue;

                driver.AddFromName<Image>(trackBinding.gameObject, iterator.propertyPath);
            }
#endif
            base.GatherProperties(director, driver);
        }
    }
}