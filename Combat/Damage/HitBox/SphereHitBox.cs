using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using XiheFramework.Runtime;

namespace XiheFramework.Combat.Damage.HitBox {
    public class SphereHitBox : HitBoxBase {
        /// <summary>
        /// Called when raycast starts hitting something
        /// @param senderId
        /// @param receiverId
        /// @param hitPoint
        /// </summary>
        public Action<uint, uint, Vector3, Collider> OnDamageEnter { get; set; }

        /// <summary>
        /// Called when raycast stays hitting something that it did last frame
        /// @param senderId
        /// @param receiverId
        /// @param hitPoint
        /// </summary>
        public Action<uint, uint, Vector3, Collider> OnDamageStay { get; set; }

        /// <summary>
        /// Called when overlapSphere not hitting anything
        /// @param lastReceiverId
        /// </summary>
        public System.Action<uint> OnDamageExit { get; set; }

        public Vector3 sphereCenter;
        public float sphereRadius = 1;
        public int maxHitCount = 10;
        public int stayCoolDownFrame = 10;

        [Tooltip("If true, the hit point will always be on the surface of the collider; otherwise, the hit point will be on the center of the sphere")]
        public bool hitOnColliderSurface = true;

        private bool m_Activated;
        private readonly List<uint> m_LastFrameHitEntityIds = new List<uint>();
        private readonly List<uint> m_CurrentFrameHitEntityIds = new List<uint>();
        private Collider[] m_Results;
        private int m_StayCoolDownTimer;

        private void Awake() {
            m_Results = new Collider[maxHitCount];
        }

        private void FixedUpdate() {
            m_CurrentFrameHitEntityIds.Clear();

            if (!m_Activated) {
                return;
            }

            var centerWS = transform.TransformPoint(sphereCenter);
            var size = Physics.OverlapSphereNonAlloc(centerWS, sphereRadius, m_Results, hitLayerMask);
            if (size == 0) {
                foreach (var lastFrameHitEntityId in m_LastFrameHitEntityIds) {
                    OnHitExitCallback(lastFrameHitEntityId);
                }

                m_LastFrameHitEntityIds.Clear();
                return;
            }

            for (var i = 0; i < m_Results.Length; i++) {
                var currentCol = m_Results[i];
                if (currentCol == null) {
                    continue;
                }

                var hurtBox = currentCol.GetComponentInParent<HurtBox>();
                if (hurtBox == null) {
                    m_Results[i] = null;
                    continue;
                }

                m_CurrentFrameHitEntityIds.Add(hurtBox.owner.EntityId);
            }

            foreach (var lastFrameHitId in m_LastFrameHitEntityIds.ToList()) {
                if (!m_CurrentFrameHitEntityIds.Contains(lastFrameHitId)) {
                    m_LastFrameHitEntityIds.Remove(lastFrameHitId);
                    OnHitExitCallback(lastFrameHitId);
                }
            }

            foreach (var hitCol in m_Results) {
                if (hitCol != null) {
                    OnHitCallback(hitCol);
                }
            }
        }


        protected override void OnEnableHitBox() {
            m_Activated = true;
        }

        protected override void OnDisableHitBox() {
            m_Activated = false;
        }

        private void OnHitCallback(Collider hitCol) {
            var hurtBox = hitCol.gameObject.GetComponentInParent<HurtBox>();
            if (hurtBox == null) {
                return;
            }

            Vector3 hitPoint;
            var sphereCenterWS = transform.TransformPoint(sphereCenter);
            if (hitOnColliderSurface) {
                hitPoint = hitCol.ClosestPoint(sphereCenterWS);
            }
            else {
                hitPoint = sphereCenterWS;
            }

            if (!m_LastFrameHitEntityIds.Contains(hurtBox.owner.EntityId)) {
                OnHitEnterCallback(hurtBox, hitPoint, hitCol);
                m_LastFrameHitEntityIds.Add(hurtBox.owner.EntityId);
            }
            else {
                OnHitStayCallback(hurtBox, hitPoint, hitCol);
            }
        }

        private void OnHitEnterCallback(HurtBox hurtBox, Vector3 hitPoint, Collider other) {
            OnDamageEnter?.Invoke(OwnerId, hurtBox.owner.EntityId, hitPoint, other);
        }

        private void OnHitStayCallback(HurtBox hurtBox, Vector3 hitPoint, Collider other) {
            if (m_StayCoolDownTimer >= stayCoolDownFrame) {
                m_StayCoolDownTimer -= stayCoolDownFrame;

                OnDamageStay?.Invoke(OwnerId, hurtBox.owner.EntityId, hitPoint, other);
                return;
            }

            m_StayCoolDownTimer++;
        }

        private void OnHitExitCallback(uint lastReceiverId) {
            OnDamageExit?.Invoke(lastReceiverId);
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.TransformPoint(sphereCenter), sphereRadius);
        }
    }
}