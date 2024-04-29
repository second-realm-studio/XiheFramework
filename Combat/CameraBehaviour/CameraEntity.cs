using XiheFramework.Core.Entity;
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

        protected override void Start() {
            base.Start();

            virtualCamera = GetComponent<CinemachineVirtualCameraBase>();
            Game.Camera.RegisterCameraEntity(this);
        }
#endif
    }
}