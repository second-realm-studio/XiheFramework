using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using XiheFramework.Modules.Base;

namespace XiheFramework.Utility.Playables.EventEmitter {
    public class EventEmitterMixerBehaviour : PlayableBehaviour {
        private bool m_FirstFrameHappened;

        private readonly Dictionary<int, string> m_InvokedEvents = new();
        private Transform m_TrackBinding;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
            m_TrackBinding = playerData as Transform;

            if (!Application.isPlaying) return;

            if (m_TrackBinding == null)
                return;

            if (!m_FirstFrameHappened) m_FirstFrameHappened = true;

            var inputCount = playable.GetInputCount();

            for (var i = 0; i < inputCount; i++) {
                var inputWeight = playable.GetInputWeight(i);
                if (inputWeight > 0.5f) {
                    var inputPlayable = (ScriptPlayable<EventEmitterBehaviour>)playable.GetInput(i);
                    var input = inputPlayable.GetBehaviour();

                    if (!m_InvokedEvents.ContainsKey(i)) {
                        if (string.IsNullOrEmpty(input.argument))
                            Game.Event.Invoke(input.eventName, m_TrackBinding.gameObject.name);
                        else
                            Game.Event.Invoke(input.eventName, m_TrackBinding.gameObject.name, input.argument);

                        Debug.Log("Event " + input.eventName + "sent by timeline binding: " + m_TrackBinding.gameObject.name);

                        m_InvokedEvents.Add(i, input.eventName);
                    }

                    break;
                }
            }
        }

        public override void OnPlayableDestroy(Playable playable) {
            m_FirstFrameHappened = false;

            if (m_TrackBinding == null)
                return;

            m_InvokedEvents.Clear();
        }
    }
}