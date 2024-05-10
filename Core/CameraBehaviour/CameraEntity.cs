using Cinemachine;
using XiheFramework.Core.Entity;
using XiheFramework.Runtime;

namespace XiheFramework.Core.CameraBehaviour {
    public class CameraEntity : GameEntity {
#if USE_CINEMACHINE
        public CinemachineVirtualCameraBase virtualCamera;

        private void OnValidate() {
            if (virtualCamera == null) {
                virtualCamera = GetComponent<CinemachineVirtualCamera>();
            }
        }

        public override void OnInitCallback() {
            virtualCamera = GetComponent<CinemachineVirtualCameraBase>();
            Game.Camera.RegisterCameraEntity(this);
        }
#endif
    }
}