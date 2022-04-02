using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SobelOutlineFeature : ScriptableRendererFeature {
    class DepthEdgeRenderPass : ScriptableRenderPass {
        private Material m_BlitMaterial;

        private RenderTargetIdentifier m_CameraRT;
        private RenderTargetHandle m_Temp;

        public DepthEdgeRenderPass(Material blitMaterial) {
            m_BlitMaterial = blitMaterial;
        }

        public void Setup(RenderTargetIdentifier cameraRT) {
            m_CameraRT = cameraRT;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
            cmd.GetTemporaryRT(m_Temp.id, renderingData.cameraData.cameraTargetDescriptor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            CommandBuffer cmd = CommandBufferPool.Get();
            cmd.Blit(m_CameraRT, m_Temp.Identifier(), m_BlitMaterial);
            cmd.Blit(m_Temp.Identifier(), m_CameraRT);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd) {
            cmd.ReleaseTemporaryRT(m_Temp.id);
        }
    }

    DepthEdgeRenderPass m_ScriptablePass;


    public bool enable = false;
    public RenderPassEvent renderPassEvent;
    public Material blitMat;

    /// <inheritdoc/>
    public override void Create() {
        m_ScriptablePass = new DepthEdgeRenderPass(blitMat);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = renderPassEvent;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        if (!enable) {
            return;
        }

        m_ScriptablePass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}