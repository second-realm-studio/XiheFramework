using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace XiheFramework.Utility.Playables.NavMeshAgentControl {
    [TrackColor(0.855f, 0.8623f, 0.87f)]
    [TrackClipType(typeof(NavMeshAgentControlClip))]
    [TrackBindingType(typeof(NavMeshAgent))]
    public class NavMeshAgentControlTrack : TrackAsset {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
            return ScriptPlayable<NavMeshAgentControlMixerBehaviour>.Create(graph, inputCount);
        }
    }
}