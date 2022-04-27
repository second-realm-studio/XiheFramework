using UnityEngine;

namespace XiheFramework {
    public class InteractableObject : MonoBehaviour {
        public string itemName;
        //public FlowScript interactBehaviour;

        //private FlowScriptController m_FlowScriptController = null;
        // public UnityEvent onInteracted;

        public Vector3 localOffset;
        public float interactRadius;

        public virtual void Interact() {
            //m_FlowScriptController.SendEvent("OnInteract");
            Game.Event.Invoke("OnInteract", this, itemName);
        }

        protected virtual void Start() {
            var c = gameObject.GetOrAddComponent<SphereCollider>();
            c.enabled = true;
            c.isTrigger = true;
            //c.center 
            c.center = localOffset;
            c.radius = interactRadius;
        }

        private void OnTriggerEnter(Collider other) {
            if (!other.CompareTag("Player")) {
                return;
            }

            GameManager.GetModule<InteractionModule>().RegisterNearPlayerObject(this);
        }

        private void OnTriggerExit(Collider other) {
            if (!other.CompareTag("Player")) {
                return;
            }

            GameManager.GetModule<InteractionModule>().UnRegisterNearPlayerObject(this);
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(transform.TransformPoint(localOffset), interactRadius);
        }

        private void OnDisable() {
            GameManager.GetModule<InteractionModule>().UnRegisterNearPlayerObject(this);
        }
    }
}