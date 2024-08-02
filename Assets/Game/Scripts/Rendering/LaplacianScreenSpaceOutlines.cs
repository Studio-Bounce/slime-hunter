using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class LaplacianScreenSpaceOutlines : ScriptableRendererFeature {

    [System.Serializable]
    private class LaplacianScreenSpaceOutlinesettings {

        [Header("Outline Settings")]
        public Color outlineColor = Color.black;
        
        [Range(0.0f, 10.0f)]
        public float depthThreshold = 0.5f;

        [Range(0.0f, 1.0f)]
        public float normalThreshold = 0.5f;

        [Range(0.0f, 0.5f)]
        public float feather = 0.5f;
        

        [Header("General Scene View Space Normal Texture Settings")]
        public RenderTextureFormat colorFormat;
        public int depthBufferBits;
        public FilterMode filterMode;
        public Color backgroundColor = Color.clear;

        [Header("View Space Normal Texture Object Draw Settings")]
        public PerObjectData perObjectData;
        public bool enableDynamicBatching;
        public bool enableInstancing;

        public bool enableTransparents;

    }

    private class ScreenSpaceOutlinePass : ScriptableRenderPass {
        
        private readonly Material screenSpaceOutlineMaterial;
        private LaplacianScreenSpaceOutlinesettings settings;

        private FilteringSettings filteringSettings;

        private readonly List<ShaderTagId> shaderTagIdList;
        private readonly Material normalsMaterial;

        private RTHandle normals;
        private RendererList normalsRenderersList;

        RTHandle temporaryBuffer;

        public ScreenSpaceOutlinePass(RenderPassEvent renderPassEvent, LayerMask layerMask,
            LaplacianScreenSpaceOutlinesettings settings) {
            this.settings = settings;
            this.renderPassEvent = renderPassEvent;

            screenSpaceOutlineMaterial = new Material(Shader.Find("Hidden/LaplacianOutlines"));
            screenSpaceOutlineMaterial.SetColor("_OutlineColor", settings.outlineColor);

            screenSpaceOutlineMaterial.SetFloat("_DepthThreshold", settings.depthThreshold);
            screenSpaceOutlineMaterial.SetFloat("_Feather", settings.feather);

            screenSpaceOutlineMaterial.SetFloat("_NormalThreshold", settings.normalThreshold);

            
            if (settings.enableTransparents){
                filteringSettings = new FilteringSettings(RenderQueueRange.all, layerMask);
            } else {
                filteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);
            }
            

            shaderTagIdList = new List<ShaderTagId> {
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("LightweightForward"),
                new ShaderTagId("SRPDefaultUnlit")
            };

            normalsMaterial = new Material(Shader.Find("Hidden/ViewSpaceNormals"));
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
            // Normals
            RenderTextureDescriptor textureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            textureDescriptor.colorFormat = settings.colorFormat;
            textureDescriptor.depthBufferBits = settings.depthBufferBits;
            RenderingUtils.ReAllocateIfNeeded(ref normals, textureDescriptor, settings.filterMode);
            
            // Color Buffer
            textureDescriptor.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref temporaryBuffer, textureDescriptor, FilterMode.Bilinear);

            ConfigureTarget(normals, renderingData.cameraData.renderer.cameraDepthTargetHandle);
            ConfigureClear(ClearFlag.Color, settings.backgroundColor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            if (!screenSpaceOutlineMaterial || !normalsMaterial || 
                renderingData.cameraData.renderer.cameraColorTargetHandle.rt == null || temporaryBuffer.rt == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get();
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
                
            // Normals
            DrawingSettings drawSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
            drawSettings.perObjectData = settings.perObjectData;
            drawSettings.enableDynamicBatching = settings.enableDynamicBatching;
            drawSettings.enableInstancing = settings.enableInstancing;
            drawSettings.overrideMaterial = normalsMaterial;
            
            RendererListParams normalsRenderersParams = new RendererListParams(renderingData.cullResults, drawSettings, filteringSettings);
            normalsRenderersList = context.CreateRendererList(ref normalsRenderersParams);
            cmd.DrawRendererList(normalsRenderersList);
            
            // Pass in RT for Outlines shader
            cmd.SetGlobalTexture(Shader.PropertyToID("_SceneViewSpaceNormals"), normals.rt);
            
            using (new ProfilingScope(cmd, new ProfilingSampler("LaplacianScreenSpaceOutlines"))) {

                Blitter.BlitCameraTexture(cmd, renderingData.cameraData.renderer.cameraColorTargetHandle, temporaryBuffer, screenSpaceOutlineMaterial, 0);
                Blitter.BlitCameraTexture(cmd, temporaryBuffer, renderingData.cameraData.renderer.cameraColorTargetHandle);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Release(){
            CoreUtils.Destroy(screenSpaceOutlineMaterial);
            CoreUtils.Destroy(normalsMaterial);
            normals?.Release();
            temporaryBuffer?.Release();
        }

    }

    [SerializeField] private RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingSkybox;
    [SerializeField] private LayerMask outlinesLayerMask;
    
    [SerializeField] private LaplacianScreenSpaceOutlinesettings outlineSettings = new LaplacianScreenSpaceOutlinesettings();

    private ScreenSpaceOutlinePass screenSpaceOutlinePass;
    
    public override void Create() {
        if (renderPassEvent < RenderPassEvent.BeforeRenderingPrePasses)
            renderPassEvent = RenderPassEvent.BeforeRenderingPrePasses;

        screenSpaceOutlinePass = new ScreenSpaceOutlinePass(renderPassEvent, outlinesLayerMask, outlineSettings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        renderer.EnqueuePass(screenSpaceOutlinePass);
    }

    protected override void Dispose(bool disposing){
        if (disposing)
        {
            screenSpaceOutlinePass?.Release();
        }
    }

}