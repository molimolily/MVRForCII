Shader "MVR/Standard"
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

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            half4 _BaseColor;
            #if ENABLE_CLIPPING
            half _Cutoff;
            #endif
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv) * _BaseColor;

                #if ENABLE_CLIPPING
                clip(col.a - _Cutoff);
                #endif

                return col;
            }
            ENDHLSL
        }
    }
    CustomEditor "MVRShaderGUI"
}
