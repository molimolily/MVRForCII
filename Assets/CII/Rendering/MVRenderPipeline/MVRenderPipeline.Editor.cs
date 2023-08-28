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
        // ��v���C���̓J�������ƂɃJ�����O������ʏ�̕`����s��
        if (!Application.isPlaying)
        {
            for (int i = 0; i < cameras.Count; i++)
            {
                RenderPerCamera(cameras[i]);
            }
            return;
        }

        // �v���C���̓V�[���r���[����уv���r���[�̃J������`�悵����ɂ��̃J���������X�g����폜
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

        // cullingCamera�̃T�[�`
        cullingCameraIndex = SearchCullingCameraAndCull(cameras);

        // cullingCamera�ŃJ�����O�ɐ��������ꍇ�ɃJ�����O���ʂ����L���ĕ`����s��
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

    // �V�[���r���[��UI��`�悷��
    partial void PrepareForSceneView(Camera camera)
    {
        if (camera.cameraType == CameraType.SceneView)
        {
            ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
        }
    }

    // �V�[���r���[�Ƀ��C���[�t���[�����I�[�o�[���C����
    partial void DrawWireOverlayForSceneView(Camera camera)
    {
        if (camera.cameraType == CameraType.SceneView && SceneView.currentDrawingSceneView.cameraMode.drawMode == DrawCameraMode.TexturedWire)
        {
            context.DrawWireOverlay(camera);
        }
    }

    // �M�Y����`�悷��
    partial void DrawGizmos(Camera camera)
    {
        if (UnityEditor.Handles.ShouldRenderGizmos())
        {
            context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
            context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
        }
    }

    // �J�������Ƃɕ`����s��
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
