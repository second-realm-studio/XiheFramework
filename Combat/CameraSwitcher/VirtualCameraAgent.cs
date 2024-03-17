using Cinemachine;
using UnityEngine;

namespace XiheFramework.Combat.CameraSwitcher {
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class VirtualCameraAgent : MonoBehaviour {
        public CinemachineVirtualCamera virtualCamera;
        public string cameraId;

        private void OnValidate() {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
        }
    }
}