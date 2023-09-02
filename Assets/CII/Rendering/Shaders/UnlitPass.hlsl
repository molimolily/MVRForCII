#ifndef CUSTOM_UnlIT_PASS_INCLUDED
#define CUSTOM_UnlIT_PASS_INCLUDED

#include "../ShaderLibrary/Common.hlsl"

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
half _Cutoff;
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

#endif