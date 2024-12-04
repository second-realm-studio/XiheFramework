using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XiheFramework.Utility.Extension;

namespace XiheFramework.Combat.Damage.HitBox {
    public class RaycastHitBox : HitBoxBase {
        /// <summary>
        /// Event triggered when hit is detected upon entering a collision.
        /// </summary>
        /// <remarks>
        /// The delegate takes three parameters:
        /// <list type="number">
        ///     <item><description><c>uint</c>: The unique identifier of the damage source.</description></item>
        ///     <item><description><c>uint</c>: The unique identifier of the damage target.</description></item>
        ///     <item><description><see cref="RaycastHit"/>: Details about the collision, such as hit point and normal.</description></item>
        /// </list>
        /// </remarks>
        public Action<uint, uint, RaycastHit> OnDamageEnter { get; set; }

        /// <summary>
        /// Event triggered when hit is detected upon staying in a collision.
        /// </summary>
        /// <remarks>
        /// The delegate takes three parameters:
        /// <list type="number">
        ///     <item><description><c>uint</c>: The unique identifier of the damage source.</description></item>
        ///     <item><description><c>uint</c>: The unique identifier of the damage target.</description></item>
        ///     <item><description><see cref="RaycastHit"/>: Details about the collision, such as hit point and normal.</description></item>
        /// </list>
        /// </remarks>
        public Action<uint, uint, RaycastHit> OnDamageStay { get; set; }

        /// <summary>
        /// Called when raycast stops hitting anything
        /// @param lastReceiverId
        /// </summary>
        public Action<uint> OnDamageExit { get; set; }

        public Vector3 rayOrigin; //object space
        public int rayArraySize = 1; //can not be lower than 1
        public float spread = 0.1f;
        public Vector3 rayDirection; //in object space
        public float rayDistance = 10;

        public int maxHitCount = 10; //0 means unlimited
        public int stayCoolDownFrame = 10;

        private bool m_Activated;
        private int m_StayCoolDownTimer;
        private readonly List<uint> m_LastFrameHitEntityIds = new List<uint>();
        private readonly List<uint> m_CurrentFrameHitEntityIds = new List<uint>();
        private List<RaycastHit> m_HitList;

        private void Awake() {
            m_HitList = new List<RaycastHit>();
            rayArraySize = Mathf.Max(rayArraySize, 1);
        }

        private void FixedUpdate() {
            if (!m_Activated) {
                return;
            }

            UpdateRaycastHitList();

            if (m_HitList.Count == 0) {
                foreach (var lastFrameHitEntityId in m_LastFrameHitEntityIds) {
                    OnHitExitCallback(lastFrameHitEntityId);
                }

                m_LastFrameHitEntityIds.Clear();
                return;
            }

            foreach (var lastFrameHitEntityId in m_LastFrameHitEntityIds.ToList()) {
                if (!m_CurrentFrameHitEntityIds.Contains(lastFrameHitEntityId)) {
                    m_LastFrameHitEntityIds.Remove(lastFrameHitEntityId);
                    OnHitExitCallback(lastFrameHitEntityId);
                }
            }

            foreach (var hit in m_HitList) {
                OnHitCallback(hit);
            }
        }

        private void UpdateRaycastHitList() {
            m_HitList.Clear();
            m_CurrentFrameHitEntityIds.Clear();

            var originWS = transform.TransformPoint(rayOrigin);
            var directionWS = transform.TransformDirection(rayDirection);

            for (int i = 0; i < rayArraySize; i++) {
                var startPos = GetOffsetRayOrigin(originWS, directionWS, i);
                if (Physics.Raycast(startPos, directionWS, out var hit, rayDistance, hitLayerMask, QueryTriggerInteraction.Collide)) {
                    var hurtBox = hit.collider.GetComponent<HurtBox>();
                    if (hurtBox == null) {
                        continue;
                    }

                    var receiverId = hurtBox.owner.EntityId;
                    if (m_CurrentFrameHitEntityIds.Contains(receiverId)) {
                        continue;
                    }

                    m_HitList.Add(hit);
                    m_CurrentFrameHitEntityIds.Add(receiverId);
                    if (m_HitList.Count >= maxHitCount) {
                        return;
                    }
                }
            }
        }

        protected override void OnEnableHitBox() {
            m_Activated = true;
        }

        protected override void OnDisableHitBox() {
            m_Activated = false;
        }

        protected void OnHitCallback(RaycastHit hit) {
            var hurtBox = hit.collider.gameObject.GetComponentInParent<HurtBox>();
            if (hurtBox == null) {
                return;
            }

            if (!m_LastFrameHitEntityIds.Contains(hurtBox.owner.EntityId)) {
                OnHitEnterCallback(hurtBox, hit);
                m_LastFrameHitEntityIds.Add(hurtBox.owner.EntityId);
            }
            else {
                OnHitStayCallback(hurtBox, hit);
            }
        }

        private void OnHitEnterCallback(HurtBox hurtBox, RaycastHit hit) {
            OnDamageEnter?.Invoke(OwnerId, hurtBox.owner.EntityId, hit);
        }

        private void OnHitStayCallback(HurtBox hurtBox, RaycastHit hit) {
            if (m_StayCoolDownTimer >= stayCoolDownFrame) {
                m_StayCoolDownTimer -= stayCoolDownFrame;

                OnDamageStay?.Invoke(OwnerId, hurtBox.owner.EntityId, hit);
                return;
            }

            m_StayCoolDownTimer++;
        }

        private void OnHitExitCallback(uint lastReceiverId) {
            OnDamageExit?.Invoke(lastReceiverId);
        }

        private Vector3 GetOffsetRayOrigin(Vector3 originWS, Vector3 directionWS, int index) {
            Vector3 offset;
            if (rayArraySize > 1) {
                var offsetDirection = Vector3.Cross(directionWS, transform.forward).normalized;
                offset = spread / 2f * -offsetDirection + index * spread / (rayArraySize - 1) * offsetDirection;
            }
            else {
                offset = Vector3.zero;
            }

            var startPos = originWS + offset;
            return startPos;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            var originWS = transform.TransformPoint(rayOrigin);
            var directionWS = transform.TransformDirection(rayDirection).normalized;
            for (int i = 0; i < rayArraySize; i++) {
                var startPos = GetOffsetRayOrigin(originWS, directionWS, i);
                Gizmos.DrawLine(startPos, startPos + directionWS * rayDistance);
            }
        }
    }
}