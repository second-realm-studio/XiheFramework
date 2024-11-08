using System;
using UnityEngine;

namespace XiheFramework.Combat.Damage.HitBox {
    public class RaycastHitBox : HitBoxBase {
        /// <summary>
        /// Called when raycast starts hitting something
        /// @param senderId
        /// @param receiverId
        /// @param hitInfo
        /// </summary>
        public Action<uint, uint, RaycastHit> OnDamageEnter { get; set; }

        /// <summary>
        /// Called when raycast stays hitting something that it did last frame
        /// @param senderId
        /// @param receiverId
        /// @param hitInfo
        /// </summary>
        public Action<uint, uint, RaycastHit> OnDamageStay { get; set; }

        /// <summary>
        /// Called when raycast stops hitting anything
        /// @param lastReceiverId
        /// </summary>
        public Action<uint> OnDamageExit { get; set; }

        public Transform rayOrigin;
        public float rayDistance = 10;
        public Vector3 rayDirection; //in object space

        public int stayCoolDownFrame = 10;

        private bool m_Activated;
        private int m_StayCoolDownTimer;
        private Collider m_LastHitCollider; //determine if hit is HitEnter or HitStay or HitExit

        private void FixedUpdate() {
            if (!m_Activated) {
                return;
            }

            var originWS = rayOrigin ? rayOrigin.position : transform.position;
            var directionWS = transform.TransformDirection(rayDirection);

            if (Physics.Raycast(originWS, directionWS, out var hit, rayDistance, hitLayerMask)) {
                OnHitCallback(hit);
            }
            else {
                OnHitExitCallback();
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

            if (m_LastHitCollider != hit.collider) {
                OnHitEnterCallback(hurtBox, hit);
                m_LastHitCollider = hit.collider;
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

        private void OnHitExitCallback() {
            if (m_LastHitCollider == null) {
                return;
            }

            var hurtBox = m_LastHitCollider.gameObject.GetComponentInParent<HurtBox>();
            if (hurtBox == null) {
                return;
            }

            OnDamageExit?.Invoke(hurtBox.owner.EntityId);
            m_LastHitCollider = null;
        }
    }
}