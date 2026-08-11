using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlinePostProcess : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        public Material outlineMaterial = null;
        [Range(1, 3)] public int downsampling = 1;
    }

    public Settings settings = new Settings();
    private OutlinePass outlinePass;

    public override void Create()
    {
        outlinePass = new OutlinePass(settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.outlineMaterial != null)
        {
            renderer.EnqueuePass(outlinePass);
        }
    }

    class OutlinePass : ScriptableRenderPass
    {
        private Settings settings;
        private RenderTargetIdentifier source;
        private RenderTargetHandle tempTexture;

        public OutlinePass(Settings settings)
        {
            this.settings = settings;
            tempTexture.Init("_TempOutlineTexture");
            renderPassEvent = settings.renderPassEvent;
        }
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            source = renderingData.cameraData.renderer.cameraColorTarget;
            ConfigureInput(ScriptableRenderPassInput.Depth);
        }
        

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (settings.outlineMaterial == null) return;
            if (renderingData.cameraData.cameraType == CameraType.Preview) return;

            CommandBuffer cmd = CommandBufferPool.Get("Outline Effect");

            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.width /= settings.downsampling;
            descriptor.height /= settings.downsampling;
            descriptor.depthBufferBits = 0;

            cmd.GetTemporaryRT(tempTexture.id, descriptor);
            Blit(cmd, source, tempTexture.Identifier());
            Blit(cmd, tempTexture.Identifier(), source, settings.outlineMaterial, 0);
            cmd.ReleaseTemporaryRT(tempTexture.id);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
