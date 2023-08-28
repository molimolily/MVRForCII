using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class MVRenderPipeline : RenderPipeline
{
    partial void RenderForEditor(ScriptableRenderContext context, List<Camera> cameras);
    partial void PrepareForSceneView(Camera camera);
    partial void DrawWireOverlayForSceneView(Camera camera);
    partial void DrawGizmos(Camera camera);
    partial void RenderPerCamera(Camera camera);

#if UNITY_EDITOR
    bool hasValidCullingResult = false;
    partial void RenderForEditor(ScriptableRenderContext context, List<Camera> cameras)
    {
        // 非プレイ時はカメラごとにカリングをする通常の描画を行う
        if (!Application.isPlaying)
        {
            for (int i = 0; i < cameras.Count; i++)
            {
                RenderPerCamera(cameras[i]);
            }
            return;
        }

        // プレイ時はシーンビューおよびプレビューのカメラを描画した後にそのカメラをリストから削除
        cameras.RemoveAll(camera =>
        {
            switch (camera.cameraType)
            {
                case CameraType.SceneView:
                    RenderPerCamera(camera);
                    return true;
                case CameraType.Preview:
                    RenderPerCamera(camera);
                    return true;
                default:
                    return false;
            }
        });

        // cullingCameraのサーチ
        cullingCameraIndex = SearchCullingCameraAndCull(cameras);

        // cullingCameraでカリングに成功した場合にカリング結果を共有して描画を行う
        if (cullingCameraIndex >= 0)
        {
            for (int i = 0; i < cameras.Count; i++)
            {
                if(i != cullingCameraIndex)
                {
                    RenderGameView(cameras[i]);
                }
            }
        }
        else
        {
            for (int i = 0; i < cameras.Count; i++)
            {
                RenderPerCamera(cameras[i]);
            }
        }
    }

    // シーンビューにUIを描画する
    partial void PrepareForSceneView(Camera camera)
    {
        if (camera.cameraType == CameraType.SceneView)
        {
            ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
        }
    }

    // シーンビューにワイヤーフレームをオーバーレイする
    partial void DrawWireOverlayForSceneView(Camera camera)
    {
        if (camera.cameraType == CameraType.SceneView && SceneView.currentDrawingSceneView.cameraMode.drawMode == DrawCameraMode.TexturedWire)
        {
            context.DrawWireOverlay(camera);
        }
    }

    // ギズモを描画する
    partial void DrawGizmos(Camera camera)
    {
        if (UnityEditor.Handles.ShouldRenderGizmos())
        {
            context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
            context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
        }
    }

    // カメラごとに描画を行う
    partial void RenderPerCamera(Camera camera)
    {
        PrepareForSceneView(camera);
        if (!Cull(camera))
        {
            return;
        }
        Setup(camera);
        DrawGeometry(camera);
        DrawGizmos(camera);
        DrawWireOverlayForSceneView(camera);
        Submit();
    }
#endif
}
