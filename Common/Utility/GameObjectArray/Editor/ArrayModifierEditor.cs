using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ArrayModifier))]
public class ArrayModifierEditor : Editor {
    private ArrayModifier m_Target;

    private void OnEnable() {
        m_Target = (ArrayModifier) target;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (GUILayout.Button("Update")) {
            m_Target.UpdateModifier();
        }

        if (GUILayout.Button("Clear")) {
            m_Target.ClearModifier();
        }
    }
}