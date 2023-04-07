using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace XiheFramework.Utility.Playables.ImageSwitcher {
    public class ImageSwitcherMixerBehaviour : PlayableBehaviour {
        private Color m_DefaultColor;
        private Sprite m_DefaultSprite;

        private bool m_FirstFrameHappened;

        private Image m_TrackBinding;

        // private Texture2D tex1 = null;
        // private Texture2D tex2 = null;
        // private Texture2D mixed = null;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
            m_TrackBinding = playerData as Image;

            if (m_TrackBinding == null)
                return;

            if (!m_FirstFrameHappened) {
                m_DefaultSprite = m_TrackBinding.sprite;
                m_DefaultColor = m_TrackBinding.color;
                m_FirstFrameHappened = true;
            }

            var inputCount = playable.GetInputCount();

            var totalWeight = 0f;
            var greatestWeight = 0f;
            // float secondGreatestWeight = 0f;
            var currentInputs = 0;

            for (var i = 0; i < inputCount; i++) {
                var inputWeight = playable.GetInputWeight(i);
                var inputPlayable = (ScriptPlayable<ImageSwitcherBehaviour>)playable.GetInput(i);
                var input = inputPlayable.GetBehaviour();

                totalWeight += inputWeight;

                if (inputWeight > greatestWeight) {
                    m_TrackBinding.sprite = input.image;
                    m_TrackBinding.color = input.color;
                    greatestWeight = inputWeight;
                }

                // if (inputWeight > secondGreatestWeight) {
                //     if (inputWeight > greatestWeight) {
                //         tex1 = input.image.texture;
                //         greatestWeight = inputWeight;
                //     }
                //     else {
                //         tex2 = input.image.texture;
                //         secondGreatestWeight = inputWeight;
                //     }
                //
                //     if (tex2 == null) {
                //         m_TrackBinding.sprite = input.image;
                //     }
                //     else {
                //         m_TrackBinding.sprite = MergeTextures(tex1, tex2);
                //     }
                // }


                if (!Mathf.Approximately(inputWeight, 0f))
                    currentInputs++;
            }

            if (currentInputs != 1 && 1f - totalWeight > greatestWeight) {
                m_TrackBinding.sprite = m_DefaultSprite;
                m_TrackBinding.color = m_DefaultColor;
            }
        }

        public override void OnPlayableDestroy(Playable playable) {
            m_FirstFrameHappened = false;

            if (m_TrackBinding == null)
                return;

            m_TrackBinding.sprite = m_DefaultSprite;
            m_TrackBinding.color = m_DefaultColor;
        }
    }
}