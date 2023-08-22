Shader "MVRShader/Transparent"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1.0, 1.0 ,1.0 ,1.0)
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent"  "Queue" = "Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            HLSLPROGRAM
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

            CBUFFER_START(UnityPerMaterial)
            sampler2D _MainTex;
            half4 _MainTex_ST;
            float4 _BaseColor;
            CBUFFER_END

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv) * _BaseColor;
                col.a = _BaseColor.a;
                return col;
            }
            ENDHLSL
        }
    }
}
