using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;

public class ArrayModifier : MonoBehaviour {
    public GameObject source;

    private Renderer m_Renderer;

    [Range(1, 20)]
    public int countX = 1;

    [Range(1, 20)]
    public int countY = 1;

    [Range(1, 20)]
    public int countZ = 1;

    //public bool useRelativeOffset;
    // public Vector3 relativeOffset;

    //public bool useConstantOffset;
    // public Vector3 constantOffset;

    public bool alignRendererBound;
    public Vector3 offset;

    // private Transform[,,] cachedInstances;
    //public bool merge;

    private Transform m_CachedTransform;

    public void UpdateModifier() {
        if (source == null) {
            return;
        }

        m_CachedTransform = transform;

        if (alignRendererBound) {
            m_Renderer = source.GetComponent<Renderer>();
        }

        foreach (Transform t in m_CachedTransform) {
            //DestroyImmediate(t.gameObject);
            UnityEditor.EditorApplication.delayCall += () => {
                Undo.DestroyObjectImmediate(t.gameObject);
                // DestroyImmediate(t.gameObject);
            };
        }


        for (int i = 0; i < countX; i++) {
            for (int j = 0; j < countY; j++) {
                for (int k = 0; k < countZ; k++) {
                    Vector3 pos = new Vector3(i * offset.x, j * offset.y, k * offset.z) + source.transform.position;

                    if (alignRendererBound) {
                        var _bounds = m_Renderer.bounds;
                        pos = new Vector3(i * _bounds.size.x, j * _bounds.size.y, k * _bounds.size.z) + m_Renderer.transform.position -
                              _bounds.min;
                    }

                    Vector3 globalPos = m_CachedTransform.TransformPoint(pos);
                    GameObject go = Instantiate(source, globalPos, m_CachedTransform.rotation, m_CachedTransform);
                    go.name = source.name + "_" + i + "_" + j + "_" + k;
                    // if (relativeOffset != Vector3.zero) {
                    //     
                    // }
                }
            }
        }
    }

    public void ClearModifier() {
        foreach (Transform t in m_CachedTransform) {
            //DestroyImmediate(t.gameObject);
            UnityEditor.EditorApplication.delayCall += () => {
                Undo.DestroyObjectImmediate(t.gameObject);
                // DestroyImmediate(t.gameObject);
            };
        }
    }
}