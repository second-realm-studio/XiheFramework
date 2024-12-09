using System;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Core.Entity;

namespace XiheFramework.Combat.HitBox.BuiltIn {
    public class BoxHitBox : HitBoxBase {
        /// <summary>
        /// Called when raycast starts hitting something
        /// @param senderId
        /// @param receiverId
        /// @param hitPoint
        /// </summary>
        public Action<uint, uint, Vector3> OnDamageEnter { get; set; }

        /// <summary>
        /// Called when raycast stays hitting something that it did last frame
        /// @param senderId
        /// @param receiverId
        /// @param hitPoint
        /// </summary>
        public Action<uint, uint, Vector3> OnDamageStay { get; set; }

        /// <summary>
        /// Called when overlapSphere not hitting anything
        /// @param lastReceiverId
        /// </summary>
        public Action<uint> OnDamageExit { get; set; }

        public Vector3 sphereCenter;
        public float sphereRadius;
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
                var currentId = m_Results[i];
                var receiverEntity = currentId.GetComponentInParent<GameEntity>();
                if (receiverEntity == null) {
                    m_Results[i] = null;
                    continue;
                }

                m_CurrentFrameHitEntityIds.Add(receiverEntity.EntityId);
            }

            foreach (var lastFrameHitId in m_LastFrameHitEntityIds) {
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
            var receiverEntity = hitCol.gameObject.GetComponentInParent<GameEntity>();
            if (receiverEntity == null) {
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

            if (!m_LastFrameHitEntityIds.Contains(receiverEntity.EntityId)) {
                OnHitEnterCallback(receiverEntity, hitPoint);
                m_LastFrameHitEntityIds.Add(receiverEntity.EntityId);
            }
            else {
                OnHitStayCallback(receiverEntity, hitPoint);
            }
        }

        private void OnHitEnterCallback(GameEntity receiverEntity, Vector3 hitPoint) {
            OnDamageEnter?.Invoke(OwnerId, receiverEntity.EntityId, hitPoint);
        }

        private void OnHitStayCallback(GameEntity receiverEntity, Vector3 hitPoint) {
            if (m_StayCoolDownTimer >= stayCoolDownFrame) {
                m_StayCoolDownTimer -= stayCoolDownFrame;

                OnDamageStay?.Invoke(OwnerId, receiverEntity.EntityId, hitPoint);
                return;
            }

            m_StayCoolDownTimer++;
        }

        private void OnHitExitCallback(uint lastReceiverId) {
            OnDamageExit.Invoke(lastReceiverId);
        }

        private void OnDrawGizmos() {
            Gizmos.color=Color.red;
            Gizmos.DrawWireSphere(transform.TransformPoint(sphereCenter), sphereRadius);
        }
    }
}