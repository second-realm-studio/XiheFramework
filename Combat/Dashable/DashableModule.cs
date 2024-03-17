using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Core.Base;

namespace XiheFramework.Combat.Dashable {
    public class DashableModule : GameModule {
        private Dictionary<int, DashableObject> m_DashableObjects = new Dictionary<int, DashableObject>();

        public void RegisterDashableObject(DashableObject dashableObject) {
            m_DashableObjects.Add(dashableObject.GetInstanceID(), dashableObject);
        }

        public DashableObject[] GetDashableObjectsWithinRange(Vector3 originPos, float range) {
            var dashableObjects = new List<DashableObject>();
            foreach (var dashableObject in m_DashableObjects.Values) {
                if (dashableObject == null) {
                    continue;
                }

                if (Vector3.Distance(dashableObject.DashPosition, originPos) <= range) {
                    dashableObjects.Add(dashableObject);
                }
            }

            return dashableObjects.ToArray();
        }
    }
}