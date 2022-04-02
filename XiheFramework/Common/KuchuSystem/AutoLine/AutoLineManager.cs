using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using XiheFramework;
using Random = UnityEngine.Random;

/// <summary>
/// add stable(mandatory) connections and auto-connect joints in the distance range
/// </summary>
public class AutoLineManager : MonoBehaviour {
    public Vector2 distanceRange;
    public float unitSegmentDst = 0.2f; //unit length for each segment for dynamic segments

    [Header("LineRenderer Setting")]
    public Material lineMaterial;

    public AnimationCurve lineCurve;
    public float lineWidth = 0.7f;

    [Range(0f, 1f)]
    public float vertexInterpolation = 0.5f;


    [Header("Additional Setting")]
    public bool debug = false;

    public Vector3 gravityDirection = Vector3.down;
    public float gravityChange = 3f;


    [Range(0, 1)]
    public float randomness;

    [Range(1f, 60f)]
    public float refreshRate = 30f;

    public bool update = false;


    private Dictionary<int, AutoLineRenderer> m_AutoLineRenderers = new Dictionary<int, AutoLineRenderer>();
    private AutoLineJoint[] m_CachedJoints;

    // private Dictionary<int, int> m_InstanceIds = new Dictionary<int, int>();
    private List<bool> m_InstanceIds = new List<bool>();

    private float m_Timer;

    private void Update() {
        if (!update) {
            return;
        }

        var refreshTime = 1 / refreshRate;
        if (m_Timer >= refreshTime) {
            UpdateConnections();
            m_Timer -= refreshTime;
        }

        m_Timer += Time.deltaTime;
    }

    private void UpdateJointId() {
        foreach (var joint in m_CachedJoints) {
            if (joint.Id != -1) {
                continue;
            }

            var changed = false;
            for (int i = 0; i < m_InstanceIds.Count; i++) {
                if (m_InstanceIds[i] == false) {
                    joint.Id = i;
                    m_InstanceIds[i] = true;
                    changed = true;
                    break;
                }
            }

            if (!changed) {
                joint.Id = m_InstanceIds.Count;
                m_InstanceIds.Add(true);
            }
        }
    }

    void DeleteInvalidConnections() {
        List<int> remove = new List<int>();
        foreach (var connection in m_AutoLineRenderers.Keys) {
            CantorPairUtility.SignedReverseCantorPair(connection, out int startId, out int endId);
            var start = GetJointById(startId);
            var end = GetJointById(endId);
            //joint not exist anymore
            var startExist = m_CachedJoints.Contains(start);
            var endExist = m_CachedJoints.Contains(end);

            if (!startExist) {
                m_InstanceIds[startId] = false;
                remove.Add(connection);
            }
            else if (!endExist) {
                m_InstanceIds[endId] = false;
                remove.Add(connection);
            }
            else {
                //too far or too close
                var distance = Vector3.Distance(start.transform.position, end.transform.position);
                var far = distance > distanceRange.y;
                var close = distance < distanceRange.x;
                if (far || close) {
                    if (m_AutoLineRenderers.ContainsKey(connection)) {
                        remove.Add(connection);
                    }
                }
            }
        }

        foreach (var key in remove) {
            Destroy(m_AutoLineRenderers[key].gameObject);
            m_AutoLineRenderers.Remove(key);
        }
    }

    private int[] GetNewConnections() {
        List<int> result = new List<int>();

        for (int i = 0; i < m_CachedJoints.Length; i++) {
            var start = m_CachedJoints[i];
            foreach (var end in m_CachedJoints) {
                //if self
                if (end == start) {
                    continue;
                }

                //if about to exist (already added into result)
                var key = CantorPairUtility.SignedCantorPair(start.Id, end.Id);
                var keyAlt = CantorPairUtility.SignedCantorPair(end.Id, start.Id);
                if (result.Contains(key) || result.Contains(keyAlt)) {
                    continue;
                }

                //if exist
                if (IsConnectionExist(start, end)) {
                    continue;
                }

                //if mandatory
                var m = start.stableConnections.Contains(end);
                //if near
                var delta = end.transform.position - start.transform.position;
                var n = delta.magnitude <= distanceRange.y && delta.magnitude >= distanceRange.x;

                //if mandatory or near
                if (m || n) {
                    result.Add(key);
                }
            }
        }

        return result.ToArray();
    }

    private void UpdateAutoLineRenderers() {
        foreach (var c in m_AutoLineRenderers.Keys) {
            CantorPairUtility.SignedReverseCantorPair(c, out int start, out int end);
            var startPos = GetJointById(start).transform.position;
            var endPos = GetJointById(end).transform.position;
            var positions = GetVertexGlobalPositions(startPos, endPos, c);
            m_AutoLineRenderers[c].SetTargetPositions(positions);
            m_AutoLineRenderers[c].step = vertexInterpolation;
        }
    }

    private void UpdateConnections() {
        Debug.Log("update");
        m_CachedJoints = FindObjectsOfType<AutoLineJoint>();

        UpdateJointId();

        //delete
        DeleteInvalidConnections();

        //add
        var newConnections = GetNewConnections();
        foreach (var id in newConnections) {
            CantorPairUtility.SignedReverseCantorPair(id, out var x, out var y);
            AddAutoLineRenderer(GetJointById(x), GetJointById(y));
        }

        //update
        UpdateAutoLineRenderers();
    }

    private Vector3[] GetVertexGlobalPositions(Vector3 start, Vector3 end, int randomSeed) {
        var result = new List<Vector3>();

        var distance = Vector3.Distance(start, end);
        var vertexCount = Mathf.FloorToInt(distance / unitSegmentDst) + 2; //2 stands for start and end

        Random.InitState(randomSeed);
        var ran = Random.Range(gravityChange, Mathf.Max(gravityChange, randomness * 10f));
        for (int i = 0; i < vertexCount; i++) {
            // var desired = Vector3.Lerp(start, start, (float) i / (vertexCount - 1));

            //gravity
            var handle = 1f / ran / distance * gravityDirection.normalized + Vector3.Lerp(start, end, 0.5f);
            var t = (float) i / (vertexCount - 1);
            var desired = BezierHelper.GetQuadraticPoint(start, end, handle, t);

            result.Add(desired);
        }

        return result.ToArray();
    }

    private void AddAutoLineRenderer(AutoLineJoint start, AutoLineJoint end) {
        if (start == null || end == null) {
            Debug.Log("null");
            return;
        }

        //key
        var key = CantorPairUtility.SignedCantorPair(start.Id, end.Id);

        //go
        var go = new GameObject(start.gameObject.name + " to " + end.gameObject.name);
        go.transform.SetParent(gameObject.transform);
        go.gameObject.layer = start.gameObject.layer;

        //lr
        var lr = go.gameObject.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.widthCurve = lineCurve;
        lr.material = lineMaterial;
        var scale = start.transform.localScale;
        lr.widthMultiplier = lineWidth * (scale.x + scale.y + scale.z) / 3f;

        //alr
        var alr = go.gameObject.AddComponent<AutoLineRenderer>();

        var startPos = start.transform.position;
        var endPos = end.transform.position;
        // var startPos = RandomPosition(start.transform.position);
        // var endPos = RandomPosition(end.transform.position);
        var positions = GetVertexGlobalPositions(startPos, endPos, key);
        alr.SetTargetPositions(positions.ToArray());
        alr.step = vertexInterpolation;

        if (m_AutoLineRenderers.ContainsKey(key)) {
            m_AutoLineRenderers[key] = alr;
            Debug.LogWarning("key already exist which shouldn't happen");
        }
        else {
            m_AutoLineRenderers.Add(key, alr);
        }
    }

    AutoLineJoint GetJointById(int id) {
        foreach (var joint in m_CachedJoints) {
            if (joint.Id == id) {
                return joint;
            }
        }

        return null;
    }

    private bool IsIsolated(AutoLineJoint joint) {
        foreach (var connection in m_AutoLineRenderers.Keys) {
            CantorPairUtility.SignedReverseCantorPair(connection, out var start, out var end);
            if (start == joint.Id) return false;
            if (end == joint.Id) return false;
        }

        return true;
    }

    private Vector3 RandomPosition(Vector3 origin) {
        var ranX = Random.Range(-randomness, randomness);
        var ranY = Random.Range(-randomness, randomness);
        var ranZ = Random.Range(-randomness, randomness);
        var rand = new Vector3(ranX, ranY, ranZ);
        return origin + rand;
    }

    private bool IsConnectionExist(AutoLineJoint start, AutoLineJoint end) {
        var key = CantorPairUtility.SignedCantorPair(start.Id, end.Id);
        if (m_AutoLineRenderers.ContainsKey(key)) {
            return true;
        }

        key = CantorPairUtility.SignedCantorPair(end.Id, start.Id);
        if (m_AutoLineRenderers.ContainsKey(key)) {
            return true;
        }

        return false;
    }

    private void OnDrawGizmos() {
        if (!debug) {
            return;
        }

        Gizmos.color = Color.red;
        foreach (var c in m_AutoLineRenderers.Keys) {
            CantorPairUtility.SignedReverseCantorPair(c, out int x, out int y);
            var start = GetJointById(x);
            var end = GetJointById(y);
            if (!start || !end) {
                continue;
            }

            var originPos = start.transform.position;
            var targetPos = end.transform.position;
            Gizmos.DrawLine(originPos, targetPos);
            // var delta = targetPos - originPos;
            // var radius = 0.025f;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanceRange.x / 2f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanceRange.y / 2f);
    }
}