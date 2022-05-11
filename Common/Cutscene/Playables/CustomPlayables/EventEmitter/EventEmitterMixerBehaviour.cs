using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using XiheFramework;

public class EventEmitterMixerBehaviour : PlayableBehaviour {
    private Transform m_TrackBinding;

    bool m_FirstFrameHappened;

    private Dictionary<int, string> m_InvokedEvents = new Dictionary<int, string>();

    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        m_TrackBinding = playerData as Transform;

        if (!Application.isPlaying) {
            return;
        }

        if (m_TrackBinding == null)
            return;

        if (!m_FirstFrameHappened) {
            m_FirstFrameHappened = true;
        }

        int inputCount = playable.GetInputCount();

        for (int i = 0; i < inputCount; i++) {
            float inputWeight = playable.GetInputWeight(i);
            if (inputWeight > 0.5f) {
                ScriptPlayable<EventEmitterBehaviour> inputPlayable = (ScriptPlayable<EventEmitterBehaviour>) playable.GetInput(i);
                EventEmitterBehaviour input = inputPlayable.GetBehaviour();

                if (!m_InvokedEvents.ContainsKey(i)) {
                    if (string.IsNullOrEmpty(input.argument)) {
                        Game.Event.Invoke(input.eventName, m_TrackBinding.gameObject.name, null);
                    }
                    else {
                        Game.Event.Invoke(input.eventName, m_TrackBinding.gameObject.name, input.argument);
                    }

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