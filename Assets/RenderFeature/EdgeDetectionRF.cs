using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EdgeDetectionRF : ScriptableRendererFeature
{
    
    class EdgeDetectionPass: ScriptableRenderPass
    {
        
        private RTHandle Source { get; set; }
        private readonly Material _outlineMaterial;
        
        public void Setup(RTHandle _source)
        {
            Source = _source;
        }

        public EdgeDetectionPass(Material outlineMaterial)
        {
            _outlineMaterial = outlineMaterial;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            ConfigureInput(ScriptableRenderPassInput.Normal);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            
            if (renderingData.cameraData.cameraType != CameraType.Game)
            {
                return;
            }
            
            var cmd = CommandBufferPool.Get("_EdgeDetectionPass");

            // Do I need this?
            var opaqueDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDescriptor.depthBufferBits = 0;
            
            var cameraColor = renderingData.cameraData.renderer.cameraColorTargetHandle;

            Blit(cmd, Source, cameraColor, _outlineMaterial);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);        
        }
    }

    [Serializable]
    public class OutlineSettings
    {
        public Material outlineMaterial;
    }

    public OutlineSettings settings = new();
    EdgeDetectionPass _edgeDetectionPass;
    
    public string sourceTextureName = "_DiscontinuityTexture";

    public override void Create()
    {
        _edgeDetectionPass = new EdgeDetectionPass(settings.outlineMaterial)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents
        };

    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.outlineMaterial == null)
        {
            Debug.LogWarningFormat("Missing Outline Material");
            return;
        }

        var sourceTextureHandle = RTHandles.Alloc(Shader.GetGlobalTexture(sourceTextureName), name: sourceTextureName);
        _edgeDetectionPass.Setup(sourceTextureHandle);
        renderer.EnqueuePass(_edgeDetectionPass);
    }
}

