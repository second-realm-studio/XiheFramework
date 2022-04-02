using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLineJoint : MonoBehaviour {
    public int Id { get; set; } = -1;

    private void Start() {
        Id = -1;
    }

    public List<AutoLineJoint> stableConnections = new List<AutoLineJoint>();
}