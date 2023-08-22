using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/MVRP Asset")]
public class MVRenderPipelineAsset : RenderPipelineAsset
{
    public bool enableSRPBatcher = true;

    [Header("Drawing Flag")]
    public bool drawSkybox = true;
    public bool drawTransparent = true;

    protected override RenderPipeline CreatePipeline()
    {
        return new MVRenderPipeline(this);
    }
}