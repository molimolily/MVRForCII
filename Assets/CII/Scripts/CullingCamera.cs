using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class CullingCamera : MonoBehaviour
{
    int cullingCameraID;

    private void Awake()
    {
        cullingCameraID = GetComponent<Camera>().GetInstanceID();
        MVRenderPipeline.cullingCameraID = cullingCameraID;
    }
}
