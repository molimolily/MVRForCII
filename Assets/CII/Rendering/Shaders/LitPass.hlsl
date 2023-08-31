#ifndef CUSTOM_LIT_PASS_INCLUDED
#define CUSTOM_LIT_PASS_INCLUDED

#include "../ShaderLibrary/Common.hlsl"
#include "../ShaderLibrary/Surface.hlsl"
#include "../ShaderLibrary/Light.hlsl"
#include "../ShaderLibrary/BRDF.hlsl"
#include "../ShaderLibrary/Lighting.hlsl"

struct appdata
{
    float3 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float4 positionCS : SV_POSITION;
    float3 positionWS : VAR_POSITION;
    float3 normalWS : VAR_NORMAL;
    float2 uv : TEXCOORD0;
};

sampler2D _MainTex;
CBUFFER_START(UnityPerMaterial)
float4 _MainTex_ST;
half4 _BaseColor;
float _Metallic;
float _Smoothness;
half _Cutoff;
CBUFFER_END

v2f vert (appdata v)
{
    v2f o;
                
    o.positionWS = TransformObjectToWorld(v.positionOS);
    o.positionCS = TransformWorldToHClip(o.positionWS);
    o.normalWS = TransformObjectToWorldNormal(v.normalOS);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    return o;
}

half4 frag(v2f i) : SV_Target
{
    half4 col = tex2D(_MainTex, i.uv) * _BaseColor;

    #if ENABLE_CLIPPING
    clip(col.a - _Cutoff);
    #endif

    Surface surface;
    surface.normal = normalize(i.normalWS);
    surface.viewDirection = normalize(_WorldSpaceCameraPos - i.positionWS);
    surface.color = col.rgb;
    surface.alpha = col.a;
    surface.metallic = _Metallic;
    surface.smoothness = _Smoothness;

    BRDF brdf = GetBRDF(surface);

    float3 color = GetLighting(surface, brdf);

    return half4(color, surface.alpha);
}
#endif