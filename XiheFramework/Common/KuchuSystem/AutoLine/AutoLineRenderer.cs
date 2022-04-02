using System;
using System.Collections.Generic;
using FlowCanvas.Nodes;
using UnityEngine;

[Serializable]
public class AutoLineRenderer : MonoBehaviour {
    LineRenderer lineRenderer;

    // public int segmentsCount;
    public float step = 0.2f; //0-1
    public Vector3[] targetPositions;

    private void Start() {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    private void Update() {
        UpdateVertexPositions(step);
    }

    public void SetTargetPositions(Vector3[] positions) {
        if (!lineRenderer) {
            lineRenderer = gameObject.GetComponent<LineRenderer>();
        }

        if (lineRenderer == null) {
            Debug.Log("null");
        }

        lineRenderer.positionCount = positions.Length;
        targetPositions = positions;
    }

    public void SetTargetPositionsInstant(Vector3[] positions) {
        if (!lineRenderer) {
            lineRenderer = gameObject.GetComponent<LineRenderer>();
        }

        lineRenderer.positionCount = positions.Length;
        targetPositions = positions;
        UpdateVertexPositions(1f);
    }

    void UpdateVertexPositions(float interpolation) {
        for (int i = 0; i < targetPositions.Length; i++) {
            var current = lineRenderer.GetPosition(i);
            var target = targetPositions[i];
            
            //check zero
            if (lineRenderer.GetPosition(i) == Vector3.zero) {
                lineRenderer.SetPosition(i, targetPositions[i]);
                continue;
            }
            
            var desired = Vector3.Lerp(current, target, interpolation);
            // m_DesiredPositions.Add(desired);
            lineRenderer.SetPosition(i, desired);
        }
    }
}