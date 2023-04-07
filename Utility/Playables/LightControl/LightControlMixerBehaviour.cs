using UnityEngine;
using UnityEngine.Playables;

namespace XiheFramework.Utility.Playables.LightControl {
    public class LightControlMixerBehaviour : PlayableBehaviour {
        private float m_DefaultBounceIntensity;
        private Color m_DefaultColor;
        private float m_DefaultIntensity;
        private float m_DefaultRange;
        private bool m_FirstFrameHappened;

        private Light m_TrackBinding;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
            m_TrackBinding = playerData as Light;

            if (m_TrackBinding == null)
                return;

            if (!m_FirstFrameHappened) {
                m_DefaultColor = m_TrackBinding.color;
                m_DefaultIntensity = m_TrackBinding.intensity;
                m_DefaultBounceIntensity = m_TrackBinding.bounceIntensity;
                m_DefaultRange = m_TrackBinding.range;
                m_FirstFrameHappened = true;
            }

            var inputCount = playable.GetInputCount();

            var blendedColor = Color.clear;
            var blendedIntensity = 0f;
            var blendedBounceIntensity = 0f;
            var blendedRange = 0f;
            var totalWeight = 0f;
            var greatestWeight = 0f;
            var currentInputs = 0;

            for (var i = 0; i < inputCount; i++) {
                var inputWeight = playable.GetInputWeight(i);
                var inputPlayable = (ScriptPlayable<LightControlBehaviour>)playable.GetInput(i);
                var input = inputPlayable.GetBehaviour();

                blendedColor += input.color * inputWeight;
                blendedIntensity += input.intensity * inputWeight;
                blendedBounceIntensity += input.bounceIntensity * inputWeight;
                blendedRange += input.range * inputWeight;
                totalWeight += inputWeight;

                if (inputWeight > greatestWeight) greatestWeight = inputWeight;

                if (!Mathf.Approximately(inputWeight, 0f))
                    currentInputs++;
            }

            m_TrackBinding.color = blendedColor + m_DefaultColor * (1f - totalWeight);
            m_TrackBinding.intensity = blendedIntensity + m_DefaultIntensity * (1f - totalWeight);
            m_TrackBinding.bounceIntensity = blendedBounceIntensity + m_DefaultBounceIntensity * (1f - totalWeight);
            m_TrackBinding.range = blendedRange + m_DefaultRange * (1f - totalWeight);
        }

        public override void OnPlayableDestroy(Playable playable) {
            m_FirstFrameHappened = false;

            if (m_TrackBinding == null)
                return;

            m_TrackBinding.color = m_DefaultColor;
            m_TrackBinding.intensity = m_DefaultIntensity;
            m_TrackBinding.bounceIntensity = m_DefaultBounceIntensity;
            m_TrackBinding.range = m_DefaultRange;
        }
    }
}