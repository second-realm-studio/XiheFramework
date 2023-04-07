using System;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace XiheFramework.Utility.Playables.Video {
    public sealed class VideoSchedulerPlayableBehaviour : PlayableBehaviour {
        internal PlayableDirector director { get; set; }

        internal IEnumerable<TimelineClip> clips { get; set; }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
            if (clips == null)
                return;

            var inputPort = 0;
            foreach (var clip in clips) {
                var scriptPlayable =
                    (ScriptPlayable<VideoPlayableBehaviour>)playable.GetInput(inputPort);

                var videoPlayableBehaviour = scriptPlayable.GetBehaviour();

                if (videoPlayableBehaviour != null) {
                    var preloadTime = Math.Max(0.0, videoPlayableBehaviour.preloadTime);
                    if (director.time >= clip.start + clip.duration ||
                        director.time <= clip.start - preloadTime)
                        videoPlayableBehaviour.StopVideo();
                    else if (director.time > clip.start - preloadTime)
                        videoPlayableBehaviour.PrepareVideo();
                }

                ++inputPort;
            }
        }
    }
}