using UnityEngine;
using UnityEngine.UI;

public class DialogueBubble : MonoBehaviour {
    public Image image;
    public Text text;

    public float widthPerLetter = 20f;
    public float heightPerLine = 20f;
    public int maxLetterPerLine = 25;
    public float edgeWidth = 25f;
    public float edgeHeight = 30f;


    private RectTransform m_Transform;

    public void UpdateSize() {
        m_Transform = GetComponent<RectTransform>();

        var length = text.text.Length;
        //define height first
        var height = (length / maxLetterPerLine + 1) * heightPerLine + edgeHeight * 2f;

        var indexs = text.text.AllIndexesOf("  ", true);
        foreach (var i in indexs) {
            Debug.Log(i);
            height += heightPerLine;
        }


        //define width
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