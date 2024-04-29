using UnityEngine;

namespace XiheFramework.Combat.Dashable {
    public class DashableObject : MonoBehaviour {
        public Vector3 DashPosition => transform.position;

        private void Start() {
            GameCombat.Dashable.RegisterDashableObject(this);
        }
    }
}