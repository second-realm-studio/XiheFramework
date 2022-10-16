using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class ShapeKeyDriver : MonoBehaviour {
    public enum Axis {
        X = 0,
        Y,
        Z,
    }

    public int blendShapeIndex;
    public Transform targetBone;
    public Axis axis;
    public float minRotation;
    public float maxRotation;

    private SkinnedMeshRenderer m_TargetRenderer;
    private Quaternion m_OriginalRotation;

    private void Start() {
        m_TargetRenderer = GetComponent<SkinnedMeshRenderer>();
        m_OriginalRotation = targetBone.localRotation;
    }

    private void Update() {
        var currentAngle = targetBone.localRotation.eulerAngles;
        float driverValue = axis switch {
            Axis.X => currentAngle.x - m_OriginalRotation.eulerAngles.x,
            Axis.Y => currentAngle.y - m_OriginalRotation.eulerAngles.y,
            Axis.Z => currentAngle.z - m_OriginalRotation.eulerAngles.z,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (driverValue < -180f) {
            driverValue += 360f;
        }
        else if (driverValue > 180f) {
            driverValue -= 360f;
        }

        var value = Mathf.InverseLerp(minRotation, maxRotation, driverValue);
        value = Mathf.Clamp01(value);

        m_TargetRenderer.SetBlendShapeWeight(blendShapeIndex, value * 100f);
    }
}