using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XiheFramework;

public class AStarNode : MonoBehaviour {
    [Header("Setting")]
    public bool walkable;

    [Header("Init Info")]
    public Vector3 worldPosition;

    public int gridX;
    public int gridY;

    [Header("Dynamic Info")]
    public int fromDst;

    public int toDst;

    public AStarNode parent;

    private Transform m_CachedTransform;
    private float m_Size = 1f;

    private void Start() {
        InitNode();
    }

    public void InitNode() {
        var size = Game.Blackboard.GetData<float>("AStar.NodeSize");
        m_Size = Math.Abs(size) <= float.Epsilon ? 1f : size;

        m_CachedTransform = transform;
        worldPosition = m_CachedTransform.position;
        gridX = Mathf.RoundToInt(m_CachedTransform.localPosition.x / size);
        gridY = Mathf.RoundToInt(m_CachedTransform.localPosition.z / size);
        // gridX=Mathf.RoundToInt(worldPosition.x / size);
        // gridX=Mathf.RoundToInt(worldPosition.z / size);
    }

    private void OnDrawGizmos() {
        if (Selection.Contains(gameObject))
            Gizmos.color = new Color(1f, 0.5f, 0f);
        else if (walkable)
            Gizmos.color = Color.white;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawWireCube(transform.position, new Vector3(m_Size, 0.2f, m_Size));
    }

    public int TotalDst => fromDst + toDst;
}