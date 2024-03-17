using System.Collections.Generic;
using XiheFramework.Core.Base;

namespace XiheFramework.Combat.CameraSwitcher {
    public class CameraModule : GameModule {
        public Dictionary<string, VirtualCameraAgent> m_CamerasInTheScene = new Dictionary<string, VirtualCameraAgent>();

        public void RegisterCamera(VirtualCameraAgent cameraAgent) {
            if (m_CamerasInTheScene.ContainsKey(cameraAgent.cameraId)) {
                
            }
        }
    }
}