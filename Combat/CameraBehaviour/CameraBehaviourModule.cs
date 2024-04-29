using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Core.Base;
#if USE_CINEMACHINE
using Cinemachine;
#endif

namespace XiheFramework.Combat.CameraBehaviour {
    public class CameraBehaviourModule : GameModule {
        public int focusedPriority = 100;
        public int unfocusedPriority = 0;

#if USE_CINEMACHINE
        public CinemachineBrain MainCameraBrain { get; private set; }
#endif
        private uint m_FocusedCameraId = 1001;
        private readonly Dictionary<uint, CameraEntity> m_CamerasInScene = new Dictionary<uint, CameraEntity>();

#if USE_CINEMACHINE
        private readonly MultiDictionary<ICinemachineCamera, CameraBehaviourBase> m_CameraBehaviours =
            new MultiDictionary<ICinemachineCamera, CombatSystem.CameraBehaviour.CameraBehaviourBase>();
#endif
        private readonly Queue<uint> m_UnregisterQueue = new();

        public override void Setup() {
#if USE_CINEMACHINE
            if (MainCameraBrain == null) {
                if (Camera.main != null) {
                    MainCameraBrain = Camera.main.GetComponent<CinemachineBrain>();
                }
                else {
                    Debug.LogError($"[CAMERA] Main camera not found");
                }

                if (MainCameraBrain == null) {
                    Debug.LogError($"[CAMERA] CinemachineBrain not found. Add a CinemachineBrain component on main camera");
                }
            }
#else
            Debug.LogError($"[CAMERA] Cinemachine not found. Please define USE_CINEMACHINE in the project settings");
#endif
        }

        public override void OnUpdate() {
#if USE_CINEMACHINE
            while (m_UnregisterQueue.Count > 0) {
                var unregisterId = m_UnregisterQueue.Dequeue();
                var vCam = m_CamerasInScene[unregisterId].virtualCamera;
                m_CameraBehaviours.RemoveList(vCam);
            }

            foreach (var camBehaviours in m_CameraBehaviours) {
                if (camBehaviours.Value == null || camBehaviours.Value.Count == 0) {
                    continue;
                }

                if (m_CamerasInScene[m_FocusedCameraId].virtualCamera == (CinemachineVirtualCameraBase)camBehaviours.Key) {
                    foreach (var behaviour in camBehaviours.Value) {
                        behaviour.OnCameraBehaviourFocusedUpdate();
                    }
                }
                else {
                    foreach (var behaviour in camBehaviours.Value) {
                        behaviour.OnCameraBehaviourUnfocusedUpdate();
                    }
                }
            }
#endif
        }

        public override void OnLateUpdate() {
#if USE_CINEMACHINE
            foreach (var camBehaviours in m_CameraBehaviours) {
                if (camBehaviours.Value == null || camBehaviours.Value.Count == 0) {
                    continue;
                }

                if (m_CamerasInScene[m_FocusedCameraId].virtualCamera == (CinemachineVirtualCameraBase)camBehaviours.Key) {
                    foreach (var behaviour in camBehaviours.Value) {
                        behaviour.OnCameraBehaviourFocusedUpdate();
                    }
                }
                else {
                    foreach (var behaviour in camBehaviours.Value) {
                        behaviour.OnCameraBehaviourUnfocusedUpdate();
                    }
                }
            }
#endif
        }

        public void SetFocusedCamera(uint cameraId) {
#if USE_CINEMACHINE
            foreach (var cam in m_CamerasInScene) {
                if (cameraId == cam.Key) {
                    cam.Value.virtualCamera.Priority = focusedPriority;
                    m_FocusedCameraId = cameraId;
                }
                else {
                    cam.Value.virtualCamera.Priority = unfocusedPriority;
                }
            }

            if (enableDebug) {
                Debug.Log($"[CAMERA] Focused virtual camera changed to {cameraId}");
            }
#endif
        }

        public void RegisterCameraEntity(CameraEntity cameraEntity) {
            m_CamerasInScene.TryAdd(cameraEntity.EntityId, cameraEntity);
        }

        public void RegisterCameraBehaviour(CameraBehaviourBase cameraBehaviour) {
#if USE_CINEMACHINE
            m_CameraBehaviours.Add(cameraBehaviour.virtualCamera, cameraBehaviour);
#endif
        }

        public void UnregisterCameraBehaviour(CameraBehaviourBase cameraBehaviour) {
#if USE_CINEMACHINE
            m_CameraBehaviours.Remove(cameraBehaviour.virtualCamera, cameraBehaviour);
#endif
        }
    }
}