using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Courtesy of Daniel Ilett - "Gaussian Blur Post Process in Unity 2021 URP"

public class BlurRendererFeature : ScriptableRendererFeature
{
    #region RenderPass
    public class BlurRenderPass : ScriptableRenderPass
    {
        private Material material;
        private BlurSettings blurSettings;

        private RenderTargetIdentifier source;
        private RenderTargetHandle blurTex;
        private int blurTexID;

        // Setup variables for pass
        public bool Setup(ScriptableRenderer renderer)
        {
            blurSettings = VolumeManager.instance.stack.GetComponent<BlurSettings>();
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

            if (blurSettings != null && blurSettings.IsActive())
            {
                material = new Material(Shader.Find("PostProcessing/Blur"));
                return true;
            }
            return false;
        }

        // Setup resources before applying effects
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            base.Configure(cmd, cameraTextureDescriptor);
            if (blurSettings == null || !blurSettings.IsActive()) return;

            blurTexID = Shader.PropertyToID("_BlurTex");
            blurTex = new RenderTargetHandle();
            blurTex.id = blurTexID;
            cmd.GetTemporaryRT(blurTex.id, cameraTextureDescriptor);

            base.Configure(cmd, cameraTextureDescriptor);
        }

        // Set up shader properties and apply to camera texture
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (blurSettings == null || !blurSettings.IsActive()) return;

            CommandBuffer cmd = CommandBufferPool.Get("Blur Post Process");


            // Set Blur effect properties
            int gridSize = Mathf.CeilToInt(blurSettings.strength.value * 3.0f);

            if (gridSize % 2 == 0)
            {
                gridSize++;
            }

            material.SetInteger("_GridSize", gridSize);
            material.SetFloat("_Spread", blurSettings.strength.value);
            source = renderingData.cameraData.renderer.cameraColorTarget;

            // Execute effect using effect material
            cmd.Blit(source, blurTex.id, material, 0);
            cmd.Blit(blurTex.id, source, material, 1);
            context.ExecuteCommandBuffer(cmd);

            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(blurTex.id);
            base.FrameCleanup(cmd);
        }
    }

    #endregion

    BlurRenderPass blurRenderPass;

    // Create pass and any resources before adding
    public override void Create()
    {
        blurRenderPass = new BlurRenderPass();
        name = "Blur";
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (blurRenderPass.Setup(renderer))
        {
            renderer.EnqueuePass(blurRenderPass);
        }
    }
}


