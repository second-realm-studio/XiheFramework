using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace XiheFramework.Utility.Playables.ScreenFader {
    public class ScreenFaderMixerBehaviour : PlayableBehaviour {
        private Color m_DefaultColor;
        private bool m_FirstFrameHappened;

        private Image m_TrackBinding;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
            m_TrackBinding = playerData as Image;

            if (m_TrackBinding == null)
                return;

            if (!m_FirstFrameHappened) {
                m_DefaultColor = m_TrackBinding.color;
                m_FirstFrameHappened = true;
            }

            var inputCount = playable.GetInputCount();

            var blendedColor = Color.clear;
            var totalWeight = 0f;
            var greatestWeight = 0f;
            var currentInputs = 0;

            for (var i = 0; i < inputCount; i++) {
                var inputWeight = playable.GetInputWeight(i);
                var inputPlayable = (ScriptPlayable<ScreenFaderBehaviour>)playable.GetInput(i);
                var input = inputPlayable.GetBehaviour();

                blendedColor += input.color * inputWeight;
                totalWeight += inputWeight;

                if (inputWeight > greatestWeight) greatestWeight = inputWeight;

                if (!Mathf.Approximately(inputWeight, 0f))
                    currentInputs++;
            }

            m_TrackBinding.color = blendedColor + m_DefaultColor * (1f - totalWeight);
        }

        public override void OnPlayableDestroy(Playable playable) {
            m_FirstFrameHappened = false;

            if (m_TrackBinding == null)
                return;

            m_TrackBinding.color = m_DefaultColor;
        }
    }
}