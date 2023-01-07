using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DiscontinuitySourceRF : ScriptableRendererFeature
{
    class DiscontinuitySourcePass : ScriptableRenderPass
    {
        int kDepthBufferBits = 32;
        private RTHandle DiscontinuityAttachmentHandle { get; set; }
        internal RenderTextureDescriptor Descriptor { get; private set; }

        private FilteringSettings _mFilteringSettings;
        private const string m_ProfilerTag = "Discontinuity Prepass";
        readonly ShaderTagId m_ShaderTagId = new("Outline");

        public DiscontinuitySourcePass(RenderQueueRange renderQueueRange, LayerMask layerMask)
        {
            _mFilteringSettings = new FilteringSettings(renderQueueRange, layerMask);
        }

        public void Setup(RenderTextureDescriptor baseDescriptor, RTHandle outlineAttachmentHandle)
        {
            DiscontinuityAttachmentHandle = outlineAttachmentHandle;
            baseDescriptor.colorFormat = RenderTextureFormat.ARGB32;
            baseDescriptor.depthBufferBits = kDepthBufferBits;
            Descriptor = baseDescriptor;
        }
        
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(Shader.PropertyToID(DiscontinuityAttachmentHandle.name), Descriptor, FilterMode.Point);
            ConfigureTarget(DiscontinuityAttachmentHandle);
            ConfigureClear(ClearFlag.All, Color.black);
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            
            if (renderingData.cameraData.cameraType != CameraType.Game)
            {
                return;
            }
            
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            
            using (new ProfilingScope(cmd, new ProfilingSampler(m_ProfilerTag)))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
                var drawSettings = CreateDrawingSettings(m_ShaderTagId, ref renderingData, sortFlags);
                drawSettings.perObjectData = PerObjectData.None;
                
                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref _mFilteringSettings);

                cmd.SetGlobalTexture("_DiscontinuityTexture", DiscontinuityAttachmentHandle);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        
    }
    
    class TransparentDiscontinuitySourcePass : ScriptableRenderPass
    {
        int kDepthBufferBits = 32;
        private RTHandle TransparentDiscontinuityAttachmentHandle { get; set; }
        private RTHandle TransparentBackFaceDiscontinuityAttachmentHandle { get; set; }
        private RenderTextureDescriptor Descriptor { get; set; }

        private FilteringSettings _mFilteringSettings;
        private const string m_ProfilerTag = "Transparent Discontinuity Prepass";
        readonly ShaderTagId m_FrontFaceShaderTagId = new("OutlineTransparent");
        

        public TransparentDiscontinuitySourcePass(RenderQueueRange renderQueueRange, LayerMask layerMask)
        {
            _mFilteringSettings = new FilteringSettings(renderQueueRange, layerMask);
        }

        public void Setup(RenderTextureDescriptor baseDescriptor, RTHandle outlineAttachmentHandle)
        {
            TransparentDiscontinuityAttachmentHandle = outlineAttachmentHandle;
            baseDescriptor.colorFormat = RenderTextureFormat.ARGBFloat;
            baseDescriptor.depthBufferBits = kDepthBufferBits;
            Descriptor = baseDescriptor;
            
        }
        
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(Shader.PropertyToID(TransparentDiscontinuityAttachmentHandle.name), Descriptor, FilterMode.Bilinear);
            ConfigureTarget(TransparentDiscontinuityAttachmentHandle);
            ConfigureClear(ClearFlag.All, Color.black);
            ConfigureInput(ScriptableRenderPassInput.Depth);
            

        }

        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            
            if (renderingData.cameraData.cameraType != CameraType.Game)
            {
                return;
            }
            
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            
            using (new ProfilingScope(cmd, new ProfilingSampler(m_ProfilerTag)))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                // Front Faces
                var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
                var drawSettings = CreateDrawingSettings(m_FrontFaceShaderTagId, ref renderingData, sortFlags);
                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref _mFilteringSettings);
                
                cmd.SetGlobalTexture("_TransparentDiscontinuityTexture", TransparentDiscontinuityAttachmentHandle);
                
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            base.OnCameraCleanup(cmd);
            cmd.ReleaseTemporaryRT(Shader.PropertyToID(TransparentDiscontinuityAttachmentHandle.name));
        }
        
    }
    
    class TransparentBackDiscontinuitySourcePass : ScriptableRenderPass
    {
        int kDepthBufferBits = 32;
        private RTHandle TransparentBackFaceDiscontinuityAttachmentHandle { get; set; }
        private RenderTextureDescriptor Descriptor { get; set; }

        private FilteringSettings _mFilteringSettings;
        private const string m_ProfilerTag = "Transparent Backface Discontinuity Prepass";
        readonly ShaderTagId m_BackFaceShaderTagId = new("BackOutlineTransparent");
        

        public TransparentBackDiscontinuitySourcePass(RenderQueueRange renderQueueRange, LayerMask layerMask)
        {
            _mFilteringSettings = new FilteringSettings(renderQueueRange, layerMask);
        }

        public void Setup(RenderTextureDescriptor baseDescriptor, RTHandle outlineAttachmentHandle)
        {
            TransparentBackFaceDiscontinuityAttachmentHandle = outlineAttachmentHandle;
            baseDescriptor.colorFormat = RenderTextureFormat.ARGBFloat;
            baseDescriptor.depthBufferBits = kDepthBufferBits;
            Descriptor = baseDescriptor;
            
        }
        
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(Shader.PropertyToID(TransparentBackFaceDiscontinuityAttachmentHandle.name), Descriptor, FilterMode.Bilinear);
            ConfigureTarget(TransparentBackFaceDiscontinuityAttachmentHandle);
            ConfigureClear(ClearFlag.All, Color.black);
            ConfigureInput(ScriptableRenderPassInput.Depth);
            

        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            
            if (renderingData.cameraData.cameraType != CameraType.Game)
            {
                return;
            }
            
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            
            using (new ProfilingScope(cmd, new ProfilingSampler(m_ProfilerTag)))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                // Back Faces
                var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
                var drawSettings = CreateDrawingSettings(m_BackFaceShaderTagId, ref renderingData, sortFlags);
                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref _mFilteringSettings);
                
                cmd.SetGlobalTexture("_TransparentBackfaceDiscontinuityTexture", TransparentBackFaceDiscontinuityAttachmentHandle);
                
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            base.OnCameraCleanup(cmd);
            cmd.ReleaseTemporaryRT(Shader.PropertyToID(TransparentBackFaceDiscontinuityAttachmentHandle.name));
        }
        
    }

    private DiscontinuitySourcePass _discontinuitySourcePass;
    private TransparentDiscontinuitySourcePass _transparentDiscontinuitySourcePass;
    private TransparentBackDiscontinuitySourcePass _transparentBackfaceDiscontinuitySourcePass;
    private RTHandle _discontinuitySourceTexture;
    private RTHandle _transparentDiscontinuitySourceTexture;
    private RTHandle _transparentBackfaceDiscontinuityTexture;
    public RenderPassEvent renderPassEvent;
    public RenderPassEvent transparentRenderPassEvent;

    public override void Create()
    {
        _discontinuitySourcePass = new DiscontinuitySourcePass(RenderQueueRange.opaque, -1)
        {
            renderPassEvent = renderPassEvent
        };
        _discontinuitySourceTexture = RTHandles.Alloc("_DiscontinuityTexture", name: "_DiscontinuityTexture");
        
        
        _transparentDiscontinuitySourcePass = new TransparentDiscontinuitySourcePass(RenderQueueRange.all, -1)
        {
            renderPassEvent = transparentRenderPassEvent
        };
        
        _transparentDiscontinuitySourceTexture = RTHandles.Alloc("_TransparentDiscontinuityTexture", name: "_TransparentDiscontinuityTexture");
        
        
        _transparentBackfaceDiscontinuitySourcePass = new TransparentBackDiscontinuitySourcePass(RenderQueueRange.all, -1)
        {
            renderPassEvent = transparentRenderPassEvent
        };
        
        _transparentBackfaceDiscontinuityTexture = RTHandles.Alloc("_TransparentBackfaceDiscontinuityTexture", name: "_TransparentBackfaceDiscontinuityTexture");
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {

        _discontinuitySourcePass.Setup(renderingData.cameraData.cameraTargetDescriptor, _discontinuitySourceTexture);
        renderer.EnqueuePass(_discontinuitySourcePass);
        _transparentDiscontinuitySourcePass.Setup(renderingData.cameraData.cameraTargetDescriptor, _transparentDiscontinuitySourceTexture);
        renderer.EnqueuePass(_transparentDiscontinuitySourcePass);
        _transparentBackfaceDiscontinuitySourcePass.Setup(renderingData.cameraData.cameraTargetDescriptor, _transparentBackfaceDiscontinuityTexture);
        renderer.EnqueuePass(_transparentBackfaceDiscontinuitySourcePass);
    }
}

