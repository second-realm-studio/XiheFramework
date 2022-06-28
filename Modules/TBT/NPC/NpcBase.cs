using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XiheFramework {
    /// <summary>
    /// ai:
    /// loop animation
    /// 
    /// player:
    /// if player is close to this && click on hit-box,  show ui
    /// if player is far from this, hide ui
    /// sub to various events
    /// </summary>
    public abstract class NpcBase : MonoBehaviour {
        public string displayName;
        public string internalName;
        public float invokeRadius;
        public float releaseRadius;
        
        public bool allowUpdateCurrentBlock = true;


        [Header("Outline")]
        public Material outlineMaterial;

        // public int outlineMatIndex;
        public float outlineWidth;
        public Color outlineColor;
        public float highlightWidth;
        public Color highlightColor;

        private Material m_OutlineMat;

        private GridBlock m_CurrentBlock;

        protected Transform interactorTransform;
        // private bool m_MouseHovering;

        protected List<Vector3> receivedPath = new List<Vector3>();

        private static readonly int OutlineWidthProp = Shader.PropertyToID("_OutlineWidth");

        public void RequestPath(Vector3 destination, bool includeDiagonal) {
            //invoke path
            Game.Grid.RequestPath(this, transform.position, destination, includeDiagonal);
        }

        public void RequestPath(int x, int y, bool includeDiagonal) {
            //invoke path
            Game.Grid.RequestPath(this, transform.position, Game.Grid.GetGridPosition(x, y), includeDiagonal);
        }

        public void FaceAt(Vector3 target) {
            target.y = transform.position.y;
            transform.LookAt(target);
        }

        protected virtual void Start() {
            Game.Npc.RegisterNpc(this);
            Game.Event.Subscribe("OnPlayerInvokeNpc", OnInvokeCharacter);
            Game.Event.Subscribe("OnPlayerReleaseNpc", OnReleaseCharacter);

            m_OutlineMat = outlineMaterial;
            m_OutlineMat.color = outlineColor;
            m_OutlineMat.SetFloat(OutlineWidthProp, outlineWidth);

            Game.Event.Subscribe("OnReceivePathToBlock", OnReceivePathToBlock);
        }

        protected virtual void OnReceivePathToBlock(object sender, object e) {
            if (!(sender is NpcBase ns)) {
                return;
            }

            if (ns != this) {
                return;
            }

            var ne = e as Vector3[];
            if (ne == null) {
                return;
            }

            receivedPath = ne.ToList();
        }


        protected virtual void OnInvokeCharacter(object sender, object e) {
            var ns = sender as Transform;
            var ne = (string) e;
            if (!string.Equals(ne, internalName)) {
                return;
            }

            if (!Game.Npc.AllowInteract()) {
                return;
            }

            if (ns != null && Vector3.Distance(transform.position, ns.position) < invokeRadius) {
                interactorTransform = ns;
                OnCharacterInvokeSystem();
                Game.Npc.SetInteractingNpc(internalName);
            }
        }

        protected virtual void OnReleaseCharacter(object sender, object e) {
            var ns = sender as Transform;
            var ne = (string) e;
            if (!string.Equals(ne, internalName)) {
                return;
            }

            Game.Npc.SetInteractingNpc(null);
        }

        protected abstract void OnCharacterInvokeSystem();

        protected virtual void Update() {
            if (interactorTransform != null) {
                // Debug.LogWarning(m_InteractorTransform);
                if (Vector3.Distance(interactorTransform.position, transform.position) > releaseRadius) {
                    Game.Event.Invoke("OnPlayerReleaseNpc", interactorTransform, internalName);
                    interactorTransform = null;
                }
            }

            if (allowUpdateCurrentBlock) {
                UpdateCurrentBlock();
            }
        }

        private void UpdateCurrentBlock() {
            //update block
            var block = Game.Grid.GetNearestBlock(transform.position);
            Debug.LogWarning(block.gameObject.name);
            if (block == m_CurrentBlock) {
                return;
            }

            // Debug.LogWarning(Game.Blackboard.GetData<float>("AStar.NodeSize"));
            if (Vector3.Distance(block.aStarNode.worldPosition, transform.position) < Game.Blackboard.GetData<float>("AStar.NodeSize") * 1.414f) {
                Game.Grid.SetWalkable(block.aStarNode.gridX, block.aStarNode.gridY, false);
                if (m_CurrentBlock != null) {
                    Game.Grid.SetWalkable(m_CurrentBlock.aStarNode.gridX, m_CurrentBlock.aStarNode.gridY, true);
                }

                m_CurrentBlock = block;
            }
        }

        private void OnMouseEnter() {
            if (m_OutlineMat != null) {
                Debug.Log("Mouse enter");
                // m_MouseHovering = true;
                m_OutlineMat.SetFloat(OutlineWidthProp, highlightWidth);
                m_OutlineMat.color = highlightColor;
            }
        }

        private void OnMouseExit() {
            if (m_OutlineMat != null) {
                // m_MouseHovering = false;
                m_OutlineMat.SetFloat(OutlineWidthProp, outlineWidth);
                m_OutlineMat.color = outlineColor;
            }
        }

        protected void OnDrawGizmosSelected() {
            Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
            Gizmos.DrawSphere(transform.position, invokeRadius);
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawSphere(transform.position, releaseRadius);
        }
    }
}