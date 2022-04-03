using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLine : MonoBehaviour {
    public List<Transform> particles;
    public Material material;
    public float width = 0.01f;
    public float widthMin = 0.01f;
    public AnimationCurve widthCurve;

    private void Start() {
        for (int i = 1; i < particles.Count; i++) {
            var line = particles[i].gameObject.AddComponent<LineRenderer>();
            //line.SetPositions(new[] {particles[i - 1].position, particles[i].position});
            //line.widthMultiplier = width;
            line.material = material;
        }
    }

    private void Update() {
        for (int i = 1; i < particles.Count; i++) {
            var line = particles[i].gameObject.GetComponent<LineRenderer>();

            var from = particles[i - 1].position;
            var to = particles[i].position;
            var middle = (to + from) / 2f;
            line.positionCount = 3;
            line.SetPositions(new[] {from, middle, to});

            var m = width * particles[i].localScale.x;
            m = Mathf.Clamp(m, widthMin, 1f);
            line.widthMultiplier = m;
            line.widthCurve = widthCurve;
            line.material = material;
        }
    }
}