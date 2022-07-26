using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;
using XiheFramework.Util;

namespace XiheFramework {
    public class HitModule : GameModule {
        public int NextId => m_HitRecords.Count;

        private Dictionary<Collider, float> m_IFrames = new Dictionary<Collider, float>();
        private MultiDictionary<int, Collider> m_HitRecords = new MultiDictionary<int, Collider>();

        public void SetIFrame(Collider target, float duration) {
            if (m_IFrames.ContainsKey(target)) {
                m_IFrames[target] = duration;
            }
            else if (duration > 0) {
                m_IFrames.Add(target, duration);
            }
        }

        public void ApplyHit(HitEntity entity, Collider target) {
            //if the collider have been applied hit by hit module once ignore it
            if (!IsHittable(target, entity.id)) {
                return;
            }

            m_HitRecords.Add(entity.id, target);

            Game.Event.Invoke("Hit.OnHit", target, entity);
        }

        public override void Update() {
            UpdateIFrames();
        }

        public override void ShutDown(ShutDownType shutDownType) {
            m_HitRecords.Clear();
            m_IFrames.Clear();
        }

        bool IsHittable(Collider col, int hitId) {
            if (!m_IFrames.ContainsKey(col)) {
                return false;
            }

            if (m_IFrames[col] <= 0) {
                return false;
            }

            if (!m_HitRecords.ContainsValue(hitId, col)) {
                return false;
            }

            return true;
        }

        void UpdateIFrames() {
            List<Collider> expiredCols = new List<Collider>();
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
    }
}