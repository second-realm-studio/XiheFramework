using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using XiheFramework.Core;

namespace XiheFramework.Utility.Playables.DialogueBubble {
    public class DialogueBubbleMixerBehaviour : PlayableBehaviour {
        private const float SpaceHeight = 40f;

        private readonly Dictionary<int, DialogueBubble> m_DialogueBubbles = new();

        private bool m_FirstFrameHappened;
        private RectTransform m_LeftParent;

        private RectTransform m_RightParent;
        // private Color m_DefaultColor;
        // private int m_DefaultFontSize;
        // private string m_DefaultText;

        private RectTransform m_TrackBinding;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
            m_TrackBinding = playerData as RectTransform;

            if (!Application.isPlaying) return;

            if (m_TrackBinding == null)
                return;

            if (!m_FirstFrameHappened) {
                m_LeftParent = m_TrackBinding.GetChild(0) as RectTransform;
                m_RightParent = m_TrackBinding.GetChild(1) as RectTransform;
                // m_DefaultColor = m_TrackBinding.color;
                // m_DefaultFontSize = m_TrackBinding.fontSize;
                // m_DefaultText = m_TrackBinding.text;
                m_FirstFrameHappened = true;
            }

            var inputCount = playable.GetInputCount();

            // float totalWeight = 0f;

            for (var i = 0; i < inputCount; i++) {
                var inputWeight = playable.GetInputWeight(i);
                if (inputWeight > 0.5f) {
                    var inputPlayable = (ScriptPlayable<DialogueBubbleBehaviour>)playable.GetInput(i);
                    var input = inputPlayable.GetBehaviour();

                    if (!m_DialogueBubbles.ContainsKey(i)) {
                        var template =
                            GameCore.Blackboard.GetData<DialogueBubble>(input.isLeft ? "DialogueBubbleTemplate_Left" : "DialogueBubbleTemplate_Right");

                        if (template == null) {
                            Debug.Log("template null");
                            return;
                        }

                        RectTransform parent = null;
                        if (m_LeftParent == null || m_RightParent == null)
                            parent = m_TrackBinding;
                        else if (input.isLeft)
                            parent = m_LeftParent;
                        else
                            parent = m_RightParent;

                        var db = Object.Instantiate(template, Vector3.zero, Quaternion.identity, parent);

                        db.GetComponent<RectTransform>().localPosition = Vector3.zero;

                        db.text.text = input.text;
                        db.image.color = input.backgroundColor;
                        db.UpdateSize();

                        m_DialogueBubbles.Add(i, db);
                    }

                    break;
                }
            }

            UpdateBubblePositions();
        }

        private void UpdateBubblePositions() {
            foreach (var key in m_DialogueBubbles.Keys) {
                var pos = GetProperPosition(key);
                m_DialogueBubbles[key].GetComponent<RectTransform>().anchoredPosition = pos;
                Debug.Log(key + pos.ToString());
            }
        }

        private Vector2 GetProperPosition(int i) {
            var totalHeight = 0f;


            for (var j = 0; j < m_DialogueBubbles.Count - 1 - i; j++) {
                totalHeight += m_DialogueBubbles[m_DialogueBubbles.Count - j - 1].totalHeight;
                totalHeight += SpaceHeight;
            }

            return new Vector2(0f, totalHeight);
        }

        /// <summary>
        ///     cut text
        /// </summary>
        /// <param name="original"> full text </param>
        /// <param name="progression"> 0-1 </param>
        /// <param name="speed"> 1 refers to full duration, 2 refer to half duration </param>
        /// <returns></returns>
        private string GetText(string original, float progression, float speed) {
            var length = Mathf.FloorToInt(original.Length * progression * speed);
            length = Mathf.Clamp(length, 0, original.Length);
            var result = original.Substring(0, length);
            return result;
        }

        public override void OnPlayableDestroy(Playable playable) {
            m_FirstFrameHappened = false;

            if (m_TrackBinding == null)
                return;

            foreach (var value in m_DialogueBubbles.Values) Object.Destroy(value.gameObject);

            m_DialogueBubbles.Clear();
        }
    }
}