using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OPFeature : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        public LayerMask layerMask;
        const string ProfilerTag = "Oil Painting";
        ProfilingSampler m_ProfilierSampler = new(ProfilerTag);
        public Material m_Material;
        RTHandle _cameraColorTgt;
        RTHandle _tempRT;
        public OPVolume m_Volume;
        public void GetTempRT(in RenderingData data)
        {
            RenderingUtils.ReAllocateIfNeeded(ref _tempRT, data.cameraData.cameraTargetDescriptor);
        }

        public void SetUp(RTHandle cameraColor)
        {
            _cameraColorTgt = cameraColor;
        }
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ConfigureInput(ScriptableRenderPassInput.Color);
            ConfigureTarget(_cameraColorTgt);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(ProfilerTag);
            Vector4 v4 = new()
            {
                x = m_Volume.screenPos.value.x,
                y = m_Volume.screenPos.value.y,
                z = m_Volume.intensity.value,
                w = m_Volume.scatter.value
            };
            m_Material.SetVector(CenterPosStrScatter, v4);
            using (new ProfilingScope(cmd, m_ProfilierSampler))
            {
                CoreUtils.SetRenderTarget(cmd, _tempRT);
                Blitter.BlitTexture(cmd, _cameraColorTgt, new Vector4(1, 1, 0, 0), m_Material, 0);
                CoreUtils.SetRenderTarget(cmd, _cameraColorTgt);
                Blitter.BlitTexture(cmd, _cameraColorTgt, _cameraColorTgt, m_Material, 0);
            }
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            cmd.Dispose();
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            _tempRT?.Release();
        }
    }

    public LayerMask layerMask;
    CustomRenderPass m_ScriptablePass;
    public RenderPassEvent m_RenderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    public Material m_Material;
    VolumeStack m_Stack;
    OPVolume m_volume;
    static readonly int CenterPosStrScatter = Shader.PropertyToID("_CenterPos_Str_Scatter");
    public override void Create()
    {
        if (m_Material == null) return;
        m_Stack = VolumeManager.instance.stack;
        m_volume = m_Stack.GetComponent<OPVolume>();
        m_ScriptablePass = new CustomRenderPass()
        {
            m_Material = m_Material,
            m_Volume = m_volume,
            layerMask = layerMask,
        };
        m_ScriptablePass.renderPassEvent = m_RenderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!ShouldRender(in renderingData)) return;
        renderer.EnqueuePass(m_ScriptablePass);
        m_ScriptablePass.GetTempRT(in renderingData);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        if (!ShouldRender(in renderingData)) return;
        m_ScriptablePass.SetUp(renderer.cameraColorTargetHandle);
    }
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
#if UNITY_EDITOR
        //如有需要,在此处销毁生成的资源,如Material等
        if (EditorApplication.isPlaying)
        {
            // Destroy(null_Material);
        }
        else
        {
            // DestroyImmediate(null_Material);
        }
#else
           //   Destroy(material);
#endif
    }
    bool ShouldRender(in RenderingData data)
    {
        if (!data.cameraData.postProcessEnabled || data.cameraData.cameraType != CameraType.Game)
        {
            return false;
        }
        if (m_ScriptablePass == null)
        {
            Debug.LogError($"RenderPass = null!");
            return false;
        }
        return true;
    }
}


