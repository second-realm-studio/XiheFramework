using System;
using FlowCanvas.Nodes;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using XiheFramework;

public class TextSwitcherMixerBehaviour : PlayableBehaviour {
    private Color m_DefaultColor;
    private int m_DefaultFontSize;
    private string m_DefaultText;

    private Text m_TrackBinding;
    private bool m_FirstFrameHappened;
    
    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        m_TrackBinding = playerData as Text;

        if (m_TrackBinding == null)
            return;

        if (!m_FirstFrameHappened) {
            m_DefaultColor = m_TrackBinding.color;
            m_DefaultFontSize = m_TrackBinding.fontSize;
            m_DefaultText = m_TrackBinding.text;
            m_FirstFrameHappened = true;
        }

        int inputCount = playable.GetInputCount();

        float totalWeight = 0f;

        for (int i = 0; i < inputCount; i++) {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<TextSwitcherBehaviour> inputPlayable = (ScriptPlayable<TextSwitcherBehaviour>) playable.GetInput(i);
            TextSwitcherBehaviour input = inputPlayable.GetBehaviour();

            totalWeight += inputWeight;

            if (!Mathf.Approximately(inputWeight, 1f)) continue;

            m_TrackBinding.color = input.color;
            m_TrackBinding.fontSize = input.fontSize;

            var translated=Game.Localization.GetValue(input.text);
            m_TrackBinding.text = GetText(translated, input.progression, input.speed);
        }

        if (!Mathf.Approximately(totalWeight, 1f)) {
            m_TrackBinding.text = m_DefaultText;
        }
    }

    /// <summary>
    /// cut text 
    /// </summary>
    /// <param name="original"> full text </param>
    /// <param name="progression"> 0-1 </param>
    /// <param name="speed"> 1 refers to full duration, 2 refer to half duration </param>
    /// <returns></returns>
    private string GetText(string original, float progression, float speed) {
        
        int length = Mathf.FloorToInt(original.Length * progression * speed);
        length = Mathf.Clamp(length, 0, original.Length);
        var result = original.Substring(0, length);
        return result;
    }

    public override void OnPlayableDestroy(Playable playable) {
        m_FirstFrameHappened = false;

        if (m_TrackBinding == null)
            return;

        m_TrackBinding.color = m_DefaultColor;
        m_TrackBinding.fontSize = m_DefaultFontSize;
        m_TrackBinding.text = m_DefaultText;
    }
}