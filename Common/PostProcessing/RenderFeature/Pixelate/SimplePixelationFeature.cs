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

        bool sceneView;
        bool preview;

        public PixelateRenderPass(int blockSize, bool sceneView, bool preview) {
            _blockSize = blockSize;
            this.sceneView = sceneView;
            this.preview = preview;
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
            if (renderingData.cameraData.isSceneViewCamera && !sceneView)
                return;
            if (renderingData.cameraData.isPreviewCamera && !preview) {
                return;
            }

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

    [Range(1, 40)]
    public int BlockSize = 3;

    public RenderPassEvent renderPassEvent;

    public bool sceneView;
    public bool preview;

    public override void Create() {
        _scriptablePass = new PixelateRenderPass(BlockSize, sceneView, preview);
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