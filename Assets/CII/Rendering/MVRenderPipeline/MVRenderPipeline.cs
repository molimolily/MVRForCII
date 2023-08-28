using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class MVRenderPipeline : RenderPipeline
{
    public static int cullingCameraID;

    int cullingCameraIndex = -1;

    ScriptableRenderContext context;
    CullingResults cullingResults;

    const string bufferName = "Render Camera";
    CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };

    static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");

    bool drawSkybox;
    bool drawTransparent;

    public MVRenderPipeline(MVRenderPipelineAsset asset)
    {
        if(asset.enableSRPBatcher)
        {
            GraphicsSettings.useScriptableRenderPipelineBatching = true;
        }

        drawSkybox = asset.drawSkybox;
        drawTransparent = asset.drawTransparent;
    }
    protected override void Render(ScriptableRenderContext context, Camera[] cameras) { }
    protected override void Render(ScriptableRenderContext context, List<Camera> cameras)
    {
        this.context = context;

        buffer.ClearRenderTarget(true, true, Color.clear);

        // �G�f�B�^�ł̏���
        #region Editor
#if UNITY_EDITOR
        RenderForEditor(context, cameras);

        if (Application.isEditor) return;
#endif
        #endregion

        // �J�����O�p�̃J�����̃T�[�`
        cullingCameraIndex = SearchCullingCameraAndCull(cameras);

        if(cullingCameraIndex >= 0)
        {
            for(int i=0; i < cameras.Count; i++)
            {
                if(i != cullingCameraIndex)
                {
                    RenderGameView(cameras[i]);
                }
            }
        }
    }

    void Setup(Camera camera)
    {
        context.SetupCameraProperties(camera);
        buffer.BeginSample(bufferName);
        ExecuteBuffer();
    }

    void ExecuteBuffer()
    {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    // cullingCamera���T�[�`���ăJ�����O���s��
    // ������cameras�ɂ�����cullingCamera�̃C���f�b�N�X��Ԃ��A������Ȃ��ꍇ��-1��Ԃ�
    int SearchCullingCameraAndCull(List<Camera> cameras)
    {
        Camera camera;
        for(int i=0;i<cameras.Count;i++)
        {
            camera = cameras[i];
            if(camera.GetInstanceID() == cullingCameraID)
            {
                if (!Cull(camera))
                {
                    return -1;
                }
                return i;
            }
        }
        return -1;
    }
    void DrawGeometry(Camera camera)
    {
        // �s�����I�u�W�F�N�g�̕`��
        SortingSettings sortingSettings = new SortingSettings(camera) { criteria = SortingCriteria.CommonOpaque};
        DrawingSettings drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings);
        FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.opaque, camera.cullingMask);
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);

        // �X�J�C�{�b�N�X�̕`��
        if(drawSkybox)
        {
            context.DrawSkybox(camera);
        }

        // �������I�u�W�F�N�g�̕`��
        if(drawTransparent)
        {
            sortingSettings.criteria = SortingCriteria.CommonTransparent;
            drawingSettings.sortingSettings = sortingSettings;
            filteringSettings.renderQueueRange = RenderQueueRange.transparent;
            context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
        }
    }

    void RenderGameView(Camera camera)
    {
        Setup(camera);
        DrawGeometry(camera);
        Submit();
    }

    void Submit()
    {
        buffer.EndSample(bufferName);
        ExecuteBuffer();
        context.Submit();
    }

    bool Cull(Camera camera)
    {
        if (camera.TryGetCullingParameters(out ScriptableCullingParameters p))
        {
            cullingResults = context.Cull(ref p);
            return true;
        }

        return false;
    }
}
