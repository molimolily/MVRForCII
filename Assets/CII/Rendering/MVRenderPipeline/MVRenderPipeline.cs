using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MVRenderPipeline : RenderPipeline
{
    public static int cullingCameraID;

    ScriptableRenderContext context;
    CullingResults cullingResults;

    const string bufferName = "Render Camera";
    CommandBuffer buffer = new CommandBuffer
    {
        name = bufferName
    };

    static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");

    public MVRenderPipeline()
    {
        GraphicsSettings.useScriptableRenderPipelineBatching = true;
    }
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        this.context = context;

        // エディタでの処理
        #region Editor
#if UNITY_EDITOR
        List<Camera> cameraList = new List<Camera>(cameras);
        // 非プレイ時はカメラごとにカリングをする通常の描画を行う
        if (!Application.isPlaying)
        {
            for(int i=0; i < cameraList.Count; i++)
            {
                RenderSceneView(cameraList[i]);
            }
            return;
        }

        // プレイ時はシーンビューおよびプレビューのカメラを描画した後にリストから削除
        cameraList.RemoveAll(camera =>
        {
            switch(camera.cameraType)
            {
                case CameraType.SceneView:
                    RenderSceneView(camera);
                    return true;
                case CameraType.Preview:
                    RenderSceneView(camera);
                    return true;
                default:
                    return false;
            }
        });

        bool hasValidCullingResult = false;
        // カリング用のカメラのサーチ
        for (int i = 0; i < cameraList.Count; i++)
        {
            Camera camera = cameraList[i];
            if (camera.GetInstanceID() == cullingCameraID)
            {
                if (camera.TryGetCullingParameters(out ScriptableCullingParameters p))
                {
                    cullingResults = context.Cull(ref p);
                    hasValidCullingResult = true;
                }
                cameraList.Remove(camera);
                break;
            }
        }

        if (hasValidCullingResult)
        {
            for (int i = 0; i < cameraList.Count; i++)
            {
                RenderGameView(cameraList[i]);
            }
        }
        else
        {
            for (int i = 0; i < cameraList.Count; i++)
            {
                RenderSceneView(cameraList[i]);
            }
        }

        if (Application.isEditor) return;
#endif
        #endregion

        int cullingCameraIndex = -1;
        // カリング用のカメラのサーチ
        for(int i=0; i < cameras.Length; i++)
        {
            Camera camera = cameras[i];

            if (camera.GetInstanceID() == cullingCameraID)
            {
                if (!Cull(camera)) return;

                cullingCameraIndex = i;
                break;
            }
        }

        if(cullingCameraIndex > 0)
        {
            for(int i=0; i < cameras.Length; i++)
            {
                if(i != cullingCameraIndex)
                {
                    RenderGameView(cameras[i]);
                }
            }
        }
        else
        {
            for (int i = 0; i < cameras.Length; i++)
            {
                RenderSceneView(cameras[i]);
            }
        }

    }

    void Setup(Camera camera)
    {
        context.SetupCameraProperties(camera);
        buffer.ClearRenderTarget(true, true, Color.clear);
        buffer.BeginSample(bufferName);
        ExecuteBuffer();
    }

    void ExecuteBuffer()
    {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    void DrawGeometry(Camera camera)
    {
        // 不透明オブジェクトの描画
        SortingSettings sortingSettings = new SortingSettings(camera) { criteria = SortingCriteria.CommonOpaque};
        DrawingSettings drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings);
        FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.opaque, camera.cullingMask);
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);

        // スカイボックスの描画
        context.DrawSkybox(camera);

        // 半透明オブジェクトの描画
        sortingSettings.criteria = SortingCriteria.CommonTransparent;
        drawingSettings.sortingSettings = sortingSettings;
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
    }

    void RenderSceneView(Camera camera)
    {
        if (!Cull(camera))
        {
            return;
        }
        Setup(camera);
        context.DrawWireOverlay(camera);
#if UNITY_EDITOR
        if (UnityEditor.Handles.ShouldRenderGizmos())
        {
            context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
        }
#endif
        DrawGeometry(camera);
        Submit();
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
