using UnityEngine;
using UnityEngine.UI;
using XiheFramework.Core.Utility;

namespace XiheFramework.Utility.Playables.DialogueBubble {
    public class DialogueBubble : MonoBehaviour {
        public Image image;
        public Text text;

        public int maxLetterPerLine = 25;
        public float edgeWidth = 25f;
        public float edgeHeight = 30f;

        public float totalHeight;

        private RectTransform m_Transform;

        public static DialogueBubble Create() {
            var go = new GameObject("dialogue bubble");
            var goRect = go.AddComponent<RectTransform>();
            goRect.anchorMax = new Vector2(1f, 0f);
            goRect.anchorMin = new Vector2(1f, 0f);
            goRect.pivot = new Vector2(1f, 0f);

            var bubble = go.AddComponent<DialogueBubble>();

            var imgObj = new GameObject("image");
            var imgRect = imgObj.AddComponent<RectTransform>();
            imgRect.SetParent(goRect);
            imgRect.anchorMin = Vector2.zero;
            imgRect.anchorMax = Vector2.one;
            imgRect.pivot = Vector2.one / 2f;
            imgRect.sizeDelta = Vector2.zero;
            imgRect.anchoredPosition = Vector2.zero;

            var img = imgObj.AddComponent<Image>();

            var txtObj = new GameObject("text");
            var txtRect = txtObj.AddComponent<RectTransform>();
            txtRect.SetParent(goRect);
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.pivot = Vector2.one / 2f;
            txtRect.sizeDelta = new Vector2(-60f, -54f);
            txtRect.anchoredPosition = Vector2.zero;

            var txt = txtObj.AddComponent<Text>();

            bubble.image = img;
            bubble.text = txt;

            return bubble;
        }

        public void UpdateSize() {
            m_Transform = GetComponent<RectTransform>();

            var length = text.text.Length;
            //define height first
            var heightPerLine = text.fontSize * text.lineSpacing;
            var height = (length / maxLetterPerLine + 1) * heightPerLine + edgeHeight * 2f;

            var indexs = text.text.AllIndexesOf("  ", true);
            foreach (var i in indexs) {
                Debug.Log(i);
                height += heightPerLine;
            }

            totalHeight = height;

            //define width
            var widthPerLetter = text.fontSize;
            length = length > maxLetterPerLine ? maxLetterPerLine : length;
            var width = widthPerLetter * length + edgeWidth * 2f;
            m_Transform.sizeDelta = new Vector2(width, height);
        }

        public void SetSprite(Sprite src) {
            image.sprite = src;
        }

        public void SetText(string src) {
            text.text = src;
        }
    }
}