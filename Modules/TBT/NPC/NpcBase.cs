using System.Collections.Generic;
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
        
        [Header("Outline")]
        public Renderer outlineRenderer;

        public int outlineMatIndex;
        public float outlineWidth;
        public Color outlineColor;
        public float highlightWidth;
        public Color highlightColor;

        private Material m_OutlineMat;
        private bool m_MouseHovering;
        
        public List<string> invokableEvents = new List<string>();

        private static readonly int OutlineColorProp = Shader.PropertyToID("_Color");
        private static readonly int OutlineWidthProp = Shader.PropertyToID("_Width");

        protected virtual void Start() {
            Game.Npc.RegisterNpc(this);
            Game.Event.Subscribe("OnPlayerInvokeNpc", OnInvokeCharacter);

            if (outlineRenderer != null) {
                m_OutlineMat = outlineRenderer.materials[outlineMatIndex];
                m_OutlineMat.SetColor(OutlineColorProp, outlineColor);
                m_OutlineMat.SetFloat(OutlineWidthProp, outlineWidth);
            }

            Game.Grid.SetWalkable(transform.position, false);
        }

        protected virtual void OnInvokeCharacter(object sender, object arg) {
            var ns = sender as Transform;
            var ne = (string) arg;
            if (!string.Equals(ne, internalName)) {
                return;
            }

            if (ns != null && Vector3.Distance(transform.position, ns.position) < invokeRadius) {
                OnCharacterInvokeSystem();
            }
        }

        protected abstract void OnCharacterInvokeSystem();

        private void OnMouseEnter() {
            if (m_OutlineMat != null) {
                m_MouseHovering = true;
                m_OutlineMat.SetFloat(OutlineWidthProp, highlightWidth);
                m_OutlineMat.SetColor(OutlineColorProp, highlightColor);
            }
        }

        private void OnMouseExit() {
            if (m_OutlineMat != null) {
                m_MouseHovering = false;
                m_OutlineMat.SetFloat(OutlineWidthProp, outlineWidth);
                m_OutlineMat.SetColor(OutlineColorProp, outlineColor);
            }
        }

        protected void OnDrawGizmosSelected() {
            Gizmos.color = new Color(1f, 1f, 1f, 0.3f);
            Gizmos.DrawSphere(transform.position, invokeRadius);
        }
    }
}