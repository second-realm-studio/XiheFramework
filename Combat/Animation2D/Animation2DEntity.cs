using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Combat.Base;
using XiheFramework.Core;
using XiheFramework.Core.LogicTime;
using XiheFramework.Runtime;

namespace XiheFramework.Combat.Animation2D {
    public sealed class Animation2DEntity : TimeBasedGameEntity {
        public string animationName;

        // public SpriteAnimation currentAnimation;
        public UvAnimation uvAnimation;
        public MeshRenderer mainMeshRenderer;
        public Vector3 offset;

        public override string EntityGroupName => "Animation2DEntity";
        public override string EntityAddressName => animationName;

        //properties
        public Material MainMaterial { get; private set; }

        public int CurrentFrame => m_CurrentFrame;
        public int TotalFrameCount => uvAnimation.totalFrames;
        public int LastFrame => uvAnimation.totalFrames - 1;
        private float AnimationFrameTime => m_FrameInterval * Time.deltaTime;
        public EndBehaviour EndBehaviour { get; private set; }

        private readonly Dictionary<int, System.Action> m_EventMap = new Dictionary<int, System.Action>();
        private int m_FrameInterval;
        private int m_CurrentFrame;
        private bool m_Paused;

        private static readonly int SheetTexturePropertyId = Shader.PropertyToID("_MainTex");
        private static readonly int LightMaskTexturePropertyId = Shader.PropertyToID("_LightMaskTex");
        private static readonly int ColorRampTexturePropertyId = Shader.PropertyToID("_LightRampTex");
        private static readonly int CurrentFramePropertyId = Shader.PropertyToID("_Frame");
        private static readonly int ColumnsPropertyId = Shader.PropertyToID("_Columns");
        private static readonly int RowsPropertyId = Shader.PropertyToID("_Rows");
        private static readonly int ShadowThicknessPropertyId = Shader.PropertyToID("_ShadowThickness");
        private static readonly int LightOffsetRangePropertyId = Shader.PropertyToID("_LightOffsetRange");
        private static readonly int ShadowColorPropertyId = Shader.PropertyToID("_ShadowColor");

        private const string FlipKeywordId = "_FLIP_ON";

#if UNITY_EDITOR
        private void OnValidate() {
            if (mainMeshRenderer == null) {
                mainMeshRenderer = GetComponentInChildren<MeshRenderer>();
            }
        }
#endif
        public override void OnDestroyCallback() {
            ClearEvent();
            StopAllCoroutines();
        }

        public void Setup(bool playFirstFrameAtStart = false) {
            MainMaterial = mainMeshRenderer.material;
            if (uvAnimation == null) {
                Debug.LogError($"{this.animationName} uvAnimation is null");
                return;
            }

            if (uvAnimation.texture != null) MainMaterial.SetTexture(SheetTexturePropertyId, uvAnimation.texture);
            if (uvAnimation.shadowTexture != null) MainMaterial.SetTexture(LightMaskTexturePropertyId, uvAnimation.shadowTexture);
            if (uvAnimation.colorRampTexture != null) MainMaterial.SetTexture(ColorRampTexturePropertyId, uvAnimation.colorRampTexture);
            MainMaterial.SetInt(CurrentFramePropertyId, 0);
            MainMaterial.SetInt(ColumnsPropertyId, uvAnimation.columns);
            MainMaterial.SetInt(RowsPropertyId, uvAnimation.rows);
            MainMaterial.SetFloat(ShadowThicknessPropertyId, uvAnimation.shadowThickness);
            MainMaterial.SetVector(LightOffsetRangePropertyId,
                new Vector4(uvAnimation.shadowOffsetMin.x, uvAnimation.shadowOffsetMin.y, uvAnimation.shadowOffsetMax.x, uvAnimation.shadowOffsetMax.y));
            MainMaterial.SetColor(ShadowColorPropertyId, uvAnimation.shadowColor);

            SetFlip(false);

            if (!playFirstFrameAtStart) {
                mainMeshRenderer.enabled = false;
            }

            FaceToCamera();
        }
        
        public void Play(EndBehaviour endBehaviour, int frameInterval) {
            mainMeshRenderer.enabled = true;
            m_FrameInterval = Mathf.Max(0, frameInterval);
            EndBehaviour = endBehaviour;
            StartCoroutine(PlayAnimationCo());
        }

        public void SetFrame(int frame) {
            m_CurrentFrame = Mathf.Clamp(frame, 0, LastFrame);
        }

        public void SetFlip(bool right) {
            var flipOffset = new Vector3(right ? this.offset.x : -this.offset.x, this.offset.y, this.offset.z);
            mainMeshRenderer.transform.localPosition = flipOffset;

            if (right) {
                MainMaterial.EnableKeyword(FlipKeywordId);
            }
            else {
                MainMaterial.DisableKeyword(FlipKeywordId);
            }
        }
        
        public void SetPause(bool isPaused) {
            if (isPaused) {
                m_Paused = true;
            }
            else {
                m_Paused = false;
            }
        }

        public void RegisterEvent(int frame, System.Action callback) {
            if (frame > TotalFrameCount - 1 || frame < 0) {
                Debug.LogWarning($"{EntityAddressName} frame {frame} out of range");
            }

            if (m_EventMap.ContainsKey(frame)) {
                m_EventMap[frame] += callback;
            }
            else {
                m_EventMap.Add(frame, callback);
            }
        }

        public void UnregisterAllEventsAt(int frame) {
            if (m_EventMap.ContainsKey(frame)) {
                m_EventMap.Remove(frame);
            }
        }

        public bool TryGetEvent(int frame, out System.Action callback) {
            return m_EventMap.TryGetValue(frame, out callback);
        }

        public void SetMaterial(Material material) {
            mainMeshRenderer.material = material;
        }

        public void ClearEvent() {
            m_EventMap.Clear();
        }

        private void Update() {
            FaceToCamera();
        }

        private void FaceToCamera() {
            if (Camera.main != null) transform.localRotation = UnityEngine.Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up);
        }

        IEnumerator PlayAnimationCo() {
            while (CurrentFrame < TotalFrameCount) {
                yield return StayCurrentFrameCo();
            }

            switch (EndBehaviour) {
                case EndBehaviour.Stop:
                    MainMaterial.SetTexture(SheetTexturePropertyId, Texture2D.blackTexture);
                    break;
                case EndBehaviour.Loop:
                    SetFrame(0);
                    break;
                case EndBehaviour.Pause:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        IEnumerator StayCurrentFrameCo() {
            //set uv
            MainMaterial.SetInt(CurrentFramePropertyId, m_CurrentFrame);

            if (TryGetEvent(m_CurrentFrame, out var action)) {
                action?.Invoke();
            }

            float timer = 0;
            //Debug.Log(AnimationFrameTime);
            while (timer < AnimationFrameTime) {
                if (!m_Paused) {
                    timer += Runtime.Game.LogicTime.ScaledDeltaTime;
                }

                yield return null;
            }

            if (CurrentFrame + 1 >= TotalFrameCount) {
                switch (EndBehaviour) {
                    case EndBehaviour.Stop:

                        break;
                    case EndBehaviour.Loop:
                        SetFrame(0);
                        break;
                    case EndBehaviour.Pause:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(EndBehaviour), EndBehaviour, null);
                }
            }
            else {
                SetFrame(CurrentFrame + 1);
            }
        }
    }
}