using UnityEngine;
#if USE_CINEMACHINE
using Cinemachine;
#endif

namespace XiheFramework.Combat.CameraBehaviour {
#if USE_CINEMACHINE
    [RequireComponent(typeof(CinemachineVirtualCamera))]
#endif
    public abstract class CameraBehaviourBase : MonoBehaviour {
#if USE_CINEMACHINE
        public CinemachineVirtualCameraBase virtualCamera;

        private void OnValidate() {
            if (virtualCamera == null) {
                virtualCamera = GetComponent<CinemachineVirtualCameraBase>();
            }
        }

        private void Start() {
            virtualCamera = GetComponent<CinemachineVirtualCameraBase>();
            GameCombat.Camera.RegisterCameraBehaviour(this);
            OnCameraBehaviourStart();

            GameCombat.Camera.MainCameraBrain.m_CameraActivatedEvent.AddListener(OnFocusedCameraChanged);
        }

        private void OnFocusedCameraChanged(ICinemachineCamera incoming, ICinemachineCamera outgoing) {
            if ((CinemachineVirtualCameraBase)incoming == virtualCamera) {
                OnCameraBehaviourFocusedEnter();
            }
            else if ((CinemachineVirtualCameraBase)outgoing == virtualCamera) {
                OnCameraBehaviourUnfocusedEnter();
            }
        }

        private void OnDestroy() {
            if (!GameCombat.Camera) return;
            OnCameraBehaviourDestroy();
            GameCombat.Camera.UnregisterCameraBehaviour(this);
        }

        /// <summary>
        /// Called once when the camera is instantiated.
        /// </summary>
        protected abstract void OnCameraBehaviourStart();

        /// <summary>
        /// Called once the camera becomes the focused camera.
        /// </summary>
        protected abstract void OnCameraBehaviourFocusedEnter();

        /// <summary>
        /// Called every render frame then the camera is focused.
        /// </summary>
        internal abstract void OnCameraBehaviourFocusedUpdate();

        /// <summary>
        /// Late-called every render frame at the end of the loop then the camera is focused.
        /// </summary>
        internal abstract void OnCameraBehaviourFocusedLateUpdate();

        /// <summary>
        /// Called once the camera becomes the unfocused camera.
        /// </summary>
        protected abstract void OnCameraBehaviourUnfocusedEnter();

        /// <summary>
        /// Called every render frame then the camera is unfocused.
        /// </summary>
        internal abstract void OnCameraBehaviourUnfocusedUpdate();

        /// <summary>
        /// Called every render frame at the end of the loop then the camera is unfocused.
        /// </summary>
        internal abstract void OnCameraBehaviourUnfocusedLateUpdate();

        protected abstract void OnCameraBehaviourDestroy();
#endif
    }
}