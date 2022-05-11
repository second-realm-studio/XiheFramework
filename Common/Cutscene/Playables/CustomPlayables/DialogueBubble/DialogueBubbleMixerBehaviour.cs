using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using XiheFramework;
using Object = UnityEngine.Object;

public class DialogueBubbleMixerBehaviour : PlayableBehaviour {
    // private Color m_DefaultColor;
    // private int m_DefaultFontSize;
    // private string m_DefaultText;

    private RectTransform m_TrackBinding;
    private bool m_FirstFrameHappened;

    private Dictionary<int, DialogueBubble> m_DialogueBubbles =
        new Dictionary<int, DialogueBubble>();

    private const float SpaceHeight = 40f;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        m_TrackBinding = playerData as RectTransform;

        if (!Application.isPlaying) {
            return;
        }

        if (m_TrackBinding == null)
            return;

        if (!m_FirstFrameHappened) {
            // m_DefaultColor = m_TrackBinding.color;
            // m_DefaultFontSize = m_TrackBinding.fontSize;
            // m_DefaultText = m_TrackBinding.text;
            m_FirstFrameHappened = true;
        }

        int inputCount = playable.GetInputCount();

        // float totalWeight = 0f;

        for (int i = 0; i < inputCount; i++) {
            float inputWeight = playable.GetInputWeight(i);
            if (inputWeight > 0.5f) {
                ScriptPlayable<DialogueBubbleBehaviour> inputPlayable = (ScriptPlayable<DialogueBubbleBehaviour>) playable.GetInput(i);
                DialogueBubbleBehaviour input = inputPlayable.GetBehaviour();

                if (!m_DialogueBubbles.ContainsKey(i)) {
                    var template = Game.Blackboard.GetData<DialogueBubble>("Template DialogueBubble");
                    if (template == null) {
                        Debug.Log("template null");
                        return;
                    }

                    var go = Object.Instantiate(template, Vector3.zero, Quaternion.identity, m_TrackBinding);
                    // var go = DialogueBubble.Create();
                    // go.transform.SetParent(m_TrackBinding);

                    go.GetComponent<RectTransform>().localPosition = Vector3.zero;

                    go.text.text = input.text;
                    go.UpdateSize();

                    m_DialogueBubbles.Add(i, go);
                }

                break;
            }
        }

        UpdateBubblePositions();
    }

    private void UpdateBubblePositions() {
        foreach (var key in m_DialogueBubbles.Keys) {
            m_DialogueBubbles[key].transform.localPosition = GetProperPosition(key);
        }
    }

    private Vector3 GetProperPosition(int i) {
        var totalHeight = 0f;

        for (int j = 0; j < m_DialogueBubbles.Count - 1 - i; j++) {
            totalHeight += m_DialogueBubbles[j].GetComponent<RectTransform>().sizeDelta.y;
            totalHeight += SpaceHeight;
        }

        return new Vector3(0f, totalHeight, 0f);
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

        foreach (var value in m_DialogueBubbles.Values) {
            Object.Destroy(value.gameObject);
        }

        m_DialogueBubbles.Clear();
    }
}