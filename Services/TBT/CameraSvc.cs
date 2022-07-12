using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;
using XiheFramework;

public static class CameraSvc {
    public static void SetHomeCameraBoundaryActive(bool clampBound) {
        var cam = GameObject.FindObjectOfType<HomeCameraPivitBehaviour>();
        if (cam == null) {
            return;
        }

        if (clampBound) {
            cam.clampBound = true;
        }
        else {
            cam.clampBound = false;
        }
    }

    public static void SetHomeCameraBoundary(Vector2 boundMin, Vector2 boundMax) {
        var cam = GameObject.FindObjectOfType<HomeCameraPivitBehaviour>();
        if (cam == null) {
            return;
        }

        cam.boundMin = boundMin;
        cam.boundMax = boundMax;
    }

    public static void MoveMainCamera(Vector3 position) {
        var cam = GameObject.FindObjectOfType<HomeCameraPivitBehaviour>();
        if (cam == null) {
            return;
        }

        cam.clampBound = false;
        cam.SetDestination(position);
    }
}