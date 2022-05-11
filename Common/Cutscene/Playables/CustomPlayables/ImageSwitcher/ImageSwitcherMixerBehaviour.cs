using System;
using AmplifyShaderEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class ImageSwitcherMixerBehaviour : PlayableBehaviour {
    Sprite m_DefaultSprite;
    Color m_DefaultColor;

    Image m_TrackBinding;

    bool m_FirstFrameHappened;

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

        int inputCount = playable.GetInputCount();

        float totalWeight = 0f;
        float greatestWeight = 0f;
        // float secondGreatestWeight = 0f;
        int currentInputs = 0;

        for (int i = 0; i < inputCount; i++) {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<ImageSwitcherBehaviour> inputPlayable = (ScriptPlayable<ImageSwitcherBehaviour>) playable.GetInput(i);
            ImageSwitcherBehaviour input = inputPlayable.GetBehaviour();

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