using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlimeRope : MonoBehaviour {
    public SlimeRopeJoint prefab;
    public int resolution;

    public Transform start;
    public Transform end;

    public float downLimit;
    public float randomness = 0.1f;

    private List<SlimeRopeJoint> joints = new List<SlimeRopeJoint>();

    private void Start() {
        InstantiateRope();
    }

    void InstantiateRope() {
        for (int i = 0; i < resolution; i++) {
            var j = Instantiate(prefab, transform, true);
            j.transform.position = start.position;
            joints.Add(j);
        }
    }

    private void Update() {
        //if (Input.GetKeyDown(KeyCode.R)) {
        UpdateTargetPositions();
        //}
    }

    private void UpdateTargetPositions() {
        var startPos = start.position;
        var endPos = end.position;

        //how much downward the handle should go based on delta of start pos and end pos
        var delta = endPos - startPos;
        var down = 1 / delta.magnitude;
        down = Mathf.Clamp(down, 0f, downLimit);

        //calculate handle pos
        var handlePos = (startPos + endPos) / 2f + new Vector3(0, -down, 0);

        //for debug
        handleDisplay = handlePos;

        for (int i = 0; i < joints.Count; i++) {
            Random.InitState(i);
            var offset = Random.Range(-randomness, randomness);
            var t = (i + 1f) / (joints.Count + 1f) + offset;
            var target = BezierHelper.GetQuadraticPoint(startPos, endPos, handlePos, t);

            joints[i].SetTargetPos(target);
            // joints[i].transform.position = target;
        }
    }

    private Vector3 handleDisplay;

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(handleDisplay, 0.1f);
    }
}