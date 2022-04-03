using System;
using UnityEngine;

public class SlimeRopeJoint : MonoBehaviour {
    public float speed = 1f;

    private Rigidbody m_Rigidbody;

    private Vector3 targetPos;

    private void Start() {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    public void SetTargetPos(Vector3 pos) {
        targetPos = pos;
    }

    // public void SetScale(float radius) {
    //     this.transform.localScale = Vector3.one * radius;
    // }

    private void Update() {
        var delta = targetPos - m_Rigidbody.position;
        m_Rigidbody.velocity = delta * speed;
    }
}