Shader "MVR/Unlit"
{
    Properties
    {
        [MainTexture] _MainTex ("Main Tex", 2D) = "white" {}
        [MainColor] _BaseColor("Base Color", Color) = (1.0, 1.0 ,1.0 ,1.0)
        _Clipping("Alpha Clipping",Float) = 0
        _Cutoff("Alpha Cutoff", Range(0.0,1.0)) = 0.5
        _SurfaceType("Surface Type", Float) = 0
        _BlendMode("Blend Mode",Float) = 0
        _SrcBlend("Src Blend", Float) = 1
        _DstBlend("Dst Blend", Float) = 0
        _ZWrite("Z Write", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]

            HLSLPROGRAM
            #pragma shader_feature ENABLE_CLIPPING
            #pragma vertex vert
            #pragma fragment frag

            #include "UnlitPass.hlsl"
            
            ENDHLSL
        }
    }
    CustomEditor "MVRShaderGUI"
}
