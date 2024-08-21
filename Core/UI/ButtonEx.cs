using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonEx : Button {
    public Sprite NormalImage;
    public Sprite PressedImage;

    public override void OnPointerDown(PointerEventData eventData) {
        base.OnPointerDown(eventData);
        if (PressedImage != null) {
            image.sprite = PressedImage;
        }
    }

    public override void OnPointerUp(PointerEventData eventData) {
        base.OnPointerUp(eventData);
        if (NormalImage != null) {
            image.sprite = NormalImage;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ButtonEx))]
public class MyButtonEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        ButtonEx button = (ButtonEx)target;
        button.NormalImage = (Sprite)EditorGUILayout.ObjectField("NormalImage", button.NormalImage, typeof(Sprite), true);
        button.PressedImage = (Sprite)EditorGUILayout.ObjectField("PressedImage", button.PressedImage, typeof(Sprite), true);
    }
}
#endif