using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SimplePixelationFeature : ScriptableRendererFeature {
    #region Renderer Pass

    class PixelateRenderPass : ScriptableRenderPass {
        private RenderTargetIdentifier m_CameraRt;
        private RenderTargetHandle m_Temp;
        int _renderTextureWidth;
        int _renderTextureHeight;

        int _blockSize;

        public PixelateRenderPass(int blockSize) {
            _blockSize = blockSize;
        }

        public void Setup(RenderTargetIdentifier cameraRT) {
            m_CameraRt = cameraRT;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
            var cameraTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            cameraTargetDescriptor.enableRandomWrite = true;

            _renderTextureWidth = cameraTargetDescriptor.width / _blockSize;
            _renderTextureHeight = cameraTargetDescriptor.height / _blockSize;
            cmd.GetTemporaryRT(m_Temp.id, _renderTextureWidth, _renderTextureHeight, 0, FilterMode.Point, RenderTextureFormat.ARGB32);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            if (renderingData.cameraData.isSceneViewCamera)
                return;

            CommandBuffer cmd = CommandBufferPool.Get();
            cmd.Blit(m_CameraRt, m_Temp.Identifier());

            cmd.Blit(m_Temp.Identifier(), m_CameraRt);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd) {
            cmd.ReleaseTemporaryRT(m_Temp.id);
        }
    }

    #endregion

    #region Renderer Feature

    PixelateRenderPass _scriptablePass;
    bool _initialized;

    [Range(1, 40)] public int BlockSize = 3;
    public RenderPassEvent renderPassEvent;

    public override void Create() {
        _scriptablePass = new PixelateRenderPass(BlockSize);
        _scriptablePass.renderPassEvent = renderPassEvent;
        _initialized = true;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        if (_initialized) {
            _scriptablePass.Setup(renderer.cameraColorTarget);
            renderer.EnqueuePass(_scriptablePass);
        }
    }

    #endregion
}