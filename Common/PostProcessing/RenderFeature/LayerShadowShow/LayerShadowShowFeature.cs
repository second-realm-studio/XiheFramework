using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LayerShadowShowFeature : ScriptableRendererFeature {
    class CustomRenderPass : ScriptableRenderPass {
        private RenderTargetHandle m_Temp;

        private Material m_Material;
        private FilterSettings m_FilteringSettings;

        public CustomRenderPass(Material material, FilterSettings filterSettings) {
            m_Material = material;
            m_FilteringSettings = filterSettings;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
            cmd.GetTemporaryRT(m_Temp.id, renderingData.cameraData.cameraTargetDescriptor);
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            CommandBuffer cmd = CommandBufferPool.Get();

            cmd.Blit(renderingData.cameraData.renderer.cameraColorTarget, m_Temp.id);
            // cmd.Blit;

            // context.DrawRenderers();
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd) {
        }
    }

    CustomRenderPass m_ScriptablePass;

    [System.Serializable]
    public class FilterSettings {
        // TODO: expose opaque, transparent, all ranges as drop down
        public RenderQueueType RenderQueueType;
        public LayerMask LayerMask;
        public string[] PassNames;

        public FilterSettings() {
            RenderQueueType = RenderQueueType.Opaque;
            LayerMask = 0;
        }
    }

    public FilterSettings filterSettings = new FilterSettings();
    public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    public Material overrideMaterial;

    /// <inheritdoc/>
    public override void Create() {
        m_ScriptablePass = new CustomRenderPass(overrideMaterial, filterSettings);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = renderPassEvent;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}