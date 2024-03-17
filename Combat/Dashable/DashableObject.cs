using UnityEngine;
using XiheFramework.Entry;

namespace XiheFramework.Combat.Dashable {
    public class DashableObject : MonoBehaviour {
        public Vector3 DashPosition => transform.position;

        private void Start() {
            Game.Dashable.RegisterDashableObject(this);
        }
    }
}