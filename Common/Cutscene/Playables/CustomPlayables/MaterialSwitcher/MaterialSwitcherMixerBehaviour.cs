using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class MaterialSwitcherMixerBehaviour : PlayableBehaviour {
    Material m_DefaultMaterial;
    private Color m_DefaultColor;

    MeshRenderer m_TrackBinding;

    bool m_FirstFrameHappened;

    // private Texture2D tex1 = null;
    // private Texture2D tex2 = null;
    // private Texture2D mixed = null;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        m_TrackBinding = playerData as MeshRenderer;

        if (m_TrackBinding == null)
            return;

        if (!m_FirstFrameHappened) {
            var sharedMaterial = m_TrackBinding.sharedMaterial;
            m_DefaultMaterial = sharedMaterial;
            m_DefaultColor = sharedMaterial.color;
            m_FirstFrameHappened = true;
        }

        int inputCount = playable.GetInputCount();

        float totalWeight = 0f;
        float greatestWeight = 0f;
        // float secondGreatestWeight = 0f;
        int currentInputs = 0;

        for (int i = 0; i < inputCount; i++) {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<MaterialSwitcherBehaviour> inputPlayable = (ScriptPlayable<MaterialSwitcherBehaviour>) playable.GetInput(i);
            MaterialSwitcherBehaviour input = inputPlayable.GetBehaviour();

            totalWeight += inputWeight;

            if (inputWeight > greatestWeight) {
                if (Application.isPlaying) {
                    if (input.material != null) {
                        m_TrackBinding.material = input.material;
                    }

                    m_TrackBinding.material.color = input.color;
                }
                else {
                    if (input.material != null) {
                        m_TrackBinding.sharedMaterial = input.material;
                    }

                    m_TrackBinding.sharedMaterial.color = input.color;
                }

                greatestWeight = inputWeight;
            }

            if (!Mathf.Approximately(inputWeight, 0f))
                currentInputs++;
        }

        if (currentInputs != 1 && 1f - totalWeight > greatestWeight) {
            if (Application.isPlaying) {
                m_TrackBinding.material = m_DefaultMaterial;
                m_TrackBinding.material.color = m_DefaultColor;
            }
            else {
                m_TrackBinding.sharedMaterial = m_DefaultMaterial;
                m_TrackBinding.sharedMaterial.color = m_DefaultColor;
            }
        }
    }

    public override void OnPlayableDestroy(Playable playable) {
        m_FirstFrameHappened = false;

        if (m_TrackBinding == null)
            return;

        if (Application.isPlaying) {
            m_TrackBinding.material = m_DefaultMaterial;
            m_TrackBinding.material.color = m_DefaultColor;
        }
        else {
            m_TrackBinding.sharedMaterial = m_DefaultMaterial;
            m_TrackBinding.sharedMaterial.color = m_DefaultColor;
        }
    }
}