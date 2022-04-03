using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class KuchuFeature : ScriptableRendererFeature {
    class KuchuRenderPass : ScriptableRenderPass {
        private const string KuchuRtName = "_KuchuRT";
        private const string TempRtName = "_KuchuTempRT";

        public Material blurMaterial;
        public Material mergeMaterial;
        public int blurPass;
        public bool scenePreview;

        private List<ShaderTagId> m_PassNames = new List<ShaderTagId>();
        private RenderQueueType m_RenderQueueType;
        private LayerMask m_LayerMask;
        private ProfilingSampler m_ProfilingSampler;

        private int m_KuchuRtId;
        private int m_TempRtId;

        private RenderTargetIdentifier m_CameraRt;
        private RenderTargetIdentifier m_KuchuRt;
        private RenderTargetIdentifier m_TempRt;

        private FilteringSettings m_FilteringSettings;
        private RenderStateBlock m_RenderStateBlock;

        public KuchuRenderPass(string[] passNames, RenderQueueType renderQueueType, LayerMask layerMask) {
            m_ProfilingSampler = new ProfilingSampler("Kuchu Feature");

            this.m_RenderQueueType = renderQueueType;

            RenderQueueRange renderQueueRange =
                renderQueueType == RenderQueueType.Transparent
                    ? RenderQueueRange.transparent
                    : RenderQueueRange.opaque;

            m_FilteringSettings = new FilteringSettings(renderQueueRange, layerMask);

            if (passNames != null && passNames.Length > 0) {
                foreach (var passName in passNames) {
                    m_PassNames.Add(new ShaderTagId(passName));
                }
            }
            else {
                m_PassNames.Add(new ShaderTagId("SRPDefaultUnlit"));
                m_PassNames.Add(new ShaderTagId("UniversalForward"));
                m_PassNames.Add(new ShaderTagId("UniversalForwardOnly"));
                m_PassNames.Add(new ShaderTagId("LightweightForward"));
            }

            m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
            //init kuchu rt
            m_KuchuRtId = Shader.PropertyToID(KuchuRtName);
            cmd.GetTemporaryRT(m_KuchuRtId, renderingData.cameraData.cameraTargetDescriptor, FilterMode.Bilinear);
            m_KuchuRt = new RenderTargetIdentifier(m_KuchuRtId);

            //init temp rt
            m_TempRtId = Shader.PropertyToID(TempRtName);
            cmd.GetTemporaryRT(m_TempRtId, renderingData.cameraData.cameraTargetDescriptor, FilterMode.Bilinear);
            m_TempRt = new RenderTargetIdentifier(m_TempRtId);

            ConfigureTarget(m_KuchuRt);

            //get cam rt
            m_CameraRt = renderingData.cameraData.renderer.cameraColorTarget;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            if (!scenePreview) {
                if (renderingData.cameraData.isSceneViewCamera) {
                    return;
                }
            }

            var sortingCriteria =
                (m_RenderQueueType == RenderQueueType.Transparent)
                    ? SortingCriteria.CommonTransparent
                    : renderingData.cameraData.defaultOpaqueSortFlags;
            var drawingSetting = CreateDrawingSettings(m_PassNames, ref renderingData, sortingCriteria);

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingSampler)) {
                //clear rt
                cmd.ClearRenderTarget(true, true, Color.clear);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                //draw to rt (dont know what render state block does yet)
                // context.DrawRenderers(renderingData.cullResults, ref drawingSetting, ref m_FilteringSettings, ref m_RenderStateBlock);
                context.DrawRenderers(renderingData.cullResults, ref drawingSetting, ref m_FilteringSettings);

                //blur
                for (int i = 0; i < blurPass; i++) {
                    cmd.Blit(m_KuchuRt, m_TempRt, blurMaterial);
                    cmd.Blit(m_TempRt, m_KuchuRt, blurMaterial);
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();
                }

                //store cam to temp
                cmd.Blit(m_CameraRt, m_TempRt);

                cmd.SetGlobalTexture("_KuchuRTGlobal", m_KuchuRt);
                cmd.Blit(m_TempRt, m_CameraRt, mergeMaterial);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd) {
            cmd.ReleaseTemporaryRT(m_KuchuRtId);
            cmd.ReleaseTemporaryRT(m_TempRtId);
        }
    }

    KuchuRenderPass m_ScriptablePass;

    public bool enable;
    public RenderPassEvent renderPassEvent;
    public RenderObjects.FilterSettings filterSettings = new RenderObjects.FilterSettings();

    public Material blurMaterial;
    public Material mergeMaterial;
    [Range(0, 10)] public int blurPass = 5; //define how many times to apply the blur
    public bool scenePreview = false;

    /// <inheritdoc/>
    public override void Create() {
        m_ScriptablePass = new KuchuRenderPass(filterSettings.PassNames, filterSettings.RenderQueueType, filterSettings.LayerMask) {
            blurMaterial = blurMaterial,
            mergeMaterial = mergeMaterial,
            blurPass = blurPass,
            scenePreview = scenePreview,

            renderPassEvent = renderPassEvent,
        };
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        if (!enable) {
            return;
        }

        renderer.EnqueuePass(m_ScriptablePass);
    }
}