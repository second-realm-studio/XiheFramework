using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;
using XiheFramework.Util;

namespace XiheFramework {
    /// <summary>
    /// TODO: separate logic and data from hit entity, store data here
    /// </summary>
    public class HitModule : GameModule {
        private Dictionary<HitBox, float> m_IFrames = new Dictionary<HitBox, float>(); //hitbox to be invincible,iframe left(in second)
        // private Dictionary<,int>

        private MultiDictionary<string, int> m_HitRecords = new MultiDictionary<string, int>(); //hit guid, owner id

        public string GetNewGuid() {
            return Guid.NewGuid().ToString();
        }

        //design to be set by hitbox owner(e.g. player when being hit
        public void SetIFrame(HitBox hitBox, float duration) {
            if (m_IFrames.ContainsKey(hitBox)) {
                m_IFrames[hitBox] = duration;
            }
            else if (duration > 0) {
                m_IFrames.Add(hitBox, duration);
            }
        }

        public void RemoveHitRecord(string guid, int ownerId) {
            m_HitRecords.Remove(guid, ownerId);
        }

        public void ApplyHit(HitEntity entity, Collider other) {
            var hitbox = other.GetComponent<HitBox>();
            if (other.GetComponent<HitBox>() == null) {
                return;
            }

            ApplyHit(entity, hitbox);
        }

        public void ApplyHit(HitEntity entity, HitBox target) {
            //if the collider have been applied hit by hit module once ignore it
            if (!IsHittable(target, entity.guid)) {
                return;
            }

            m_HitRecords.Add(entity.guid, target.ownerId);
            StartCoroutine(RemoveHitRecordWithDelayCo(entity.guid, entity.coolDown, target.ownerId));

            Game.Event.Invoke("Hit.OnHit", target, entity);
        }

        public override void Update() {
            UpdateIFrames();
        }

        public override void ShutDown(ShutDownType shutDownType) {
            m_HitRecords.Clear();
            m_IFrames.Clear();
        }

        bool IsHittable(HitBox hitbox, string guid) {
            //hitbox in i-frame
            if (m_IFrames.ContainsKey(hitbox)) {
                if (m_IFrames[hitbox] >= 0) {
                    return false;
                }
            }

            //hit never hit anything
            if (!m_HitRecords.ContainsKey(guid)) {
                return false;
            }

            //hitbox has been hit by this hitEntity(guid)
            if (m_HitRecords.ContainsValue(guid, hitbox.ownerId)) {
                return false;
            }

            return true;
        }

        void UpdateIFrames() {
            List<HitBox> expiredCols = new List<HitBox>();
            foreach (var col in m_IFrames.Keys) {
                if (m_IFrames[col] <= 0) {
                    expiredCols.Add(col);
                }
                else {
                    m_IFrames[col] -= m_IFrames[col] - Time.deltaTime;
                }
            }

            foreach (var col in expiredCols) {
                m_IFrames.Remove(col);
            }
        }

        private IEnumerator RemoveHitRecordWithDelayCo(string guid, float cd, int ownerId) {
            yield return new WaitForSeconds(cd);

            //remove owner id from hit record
            RemoveHitRecord(guid, ownerId);
        }
    }
}