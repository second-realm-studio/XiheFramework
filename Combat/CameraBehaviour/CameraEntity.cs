using XiheFramework.Core.Entity;
using XiheFramework.Runtime;
#if USE_CINEMACHINE
using Cinemachine;
#endif

namespace XiheFramework.Combat.CameraBehaviour {
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