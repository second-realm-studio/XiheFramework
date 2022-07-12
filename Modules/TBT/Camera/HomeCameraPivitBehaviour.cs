using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XiheFramework;

public class HomeCameraPivitBehaviour : MonoBehaviour {
    public bool invertDrag;
    public bool clampBound;
    public Vector2 boundMin;
    public Vector2 boundMax;

    private Vector3 m_Destination;
    private Vector4 m_Area;

    public void SetDestination(Vector3 destination) {
        m_Destination = destination;
    }
    
    // Start is called before the first frame update
    void Start() {
        m_Area = new Vector4(boundMin.x, boundMax.x, boundMin.y, boundMax.y);
        m_Destination = transform.position;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKey(KeyCode.Mouse1)) {
            DragByMouse();
        }

        if (Input.GetKey(KeyCode.Space)) {
            LockToPlayer();
        }

        if (clampBound) {
            ClampBound();
        }

        transform.position = Vector3.Lerp(transform.position, m_Destination, 0.3f);
    }

    private void DragByMouse() {
        var delta = Game.Input.GetMouseDeltaPosition();
        if (Camera.main != null) {
            var cam = Camera.main.transform;
            var worldDrag = cam.right * delta.x + cam.forward * delta.y;
            if (invertDrag) {
                worldDrag = cam.right * -delta.x + cam.forward * -delta.y;
            }

            m_Destination += worldDrag * Time.deltaTime;
        }
    }

    private void ClampBound() {
        if (!m_Area.Contain(m_Destination)) {
            var nearest = m_Destination.ToVector2().GetNearestPointFromOutside(m_Area);
            m_Destination = new Vector3(nearest.x, transform.position.y, nearest.y);
        }
    }

    private void LockToPlayer() {
        m_Destination = Game.Blackboard.GetData<Vector3>("Player.CurrentPosition");
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(
            new Vector3((boundMin.x + boundMax.x) / 2f, transform.position.y, (boundMin.y + boundMax.y) / 2f),
            new Vector3(boundMax.x - boundMin.x, 0.5f, boundMax.y - boundMin.y));
    }
}