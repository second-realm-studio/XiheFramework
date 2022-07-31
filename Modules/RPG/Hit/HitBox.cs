using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour {
    public Collider hitboxCollider;
    public string ownerId;

    private void Start() {
        if (hitboxCollider == null) {
            this.hitboxCollider = GetComponent<Collider>();
        }
    }

    public void Init(string guid) {
        ownerId = guid;
    }
}