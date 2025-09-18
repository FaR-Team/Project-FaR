Shader "FaRTeam/Fog/StaticFog"
{
    Properties
    {
        _FogColor("Fog Color", Color) = (1,1,1,1)
        _UseSoftFade("Use Soft Visual Fade", Float) = 1
        _VisibleFadeStart("Visible Fade Start (norm 0..1)", Range(0,1)) = 0.0
        _VisibleFadeEnd("Visible Fade End (norm 0..1)", Range(0,1)) = 0.3
        _Alpha("Overlay Alpha", Range(0,1)) = 1.0
        _DepthAlphaThreshold("Depth Write Alpha Threshold", Range(0,1)) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Cull Off

        // ---------- Pass 1: DepthOnly (writes depth where wall is visible) ----------
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
            ZWrite On
            ColorMask 0
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex DepthVert
            #pragma fragment DepthFrag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _FogColor;
                float _UseSoftFade;
                float _VisibleFadeStart;
                float _VisibleFadeEnd;
                float _Alpha;
                float _DepthAlphaThreshold;
            CBUFFER_END

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings { float4 positionCS : SV_POSITION; float3 posOS : TEXCOORD0; };

            Varyings DepthVert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.posOS = IN.positionOS.xyz;
                return OUT;
            }

            half4 DepthFrag(Varyings IN) : SV_Target
            {
                // If not using soft fade, fully write depth.
                float alpha = _Alpha;

                // else compute alpha from local Z (object-space coords of cube are normally -0.5..0.5)
                if (_UseSoftFade > 0.5)
                {
                    float zNorm = saturate(IN.posOS.z + 0.5); // map -0.5..0.5 -> 0..1
                    if (_VisibleFadeEnd > _VisibleFadeStart)
                        alpha = saturate((zNorm - _VisibleFadeStart) / (_VisibleFadeEnd - _VisibleFadeStart));
                    else
                        alpha = 1.0;
                }

                // write depth only if alpha is above threshold
                if (alpha < _DepthAlphaThreshold)
                    discard;

                return 0;
            }
            ENDHLSL
        }

        // ---------- Pass 2: Visual overlay (transparent), optional soft fade ----------
        Pass
        {
            Name "Overlay"
            Tags { }
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex OverlayVert
            #pragma fragment OverlayFrag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _FogColor;
                float _UseSoftFade;
                float _VisibleFadeStart;
                float _VisibleFadeEnd;
                float _Alpha;
            CBUFFER_END

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings { float4 positionCS : SV_POSITION; float3 posOS : TEXCOORD0; };

            Varyings OverlayVert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.posOS = IN.positionOS.xyz;
                return OUT;
            }

            half4 OverlayFrag(Varyings IN) : SV_Target
            {
                float alpha = _Alpha;
                if (_UseSoftFade > 0.5)
                {
                    float zNorm = saturate(IN.posOS.z + 0.5);
                    if (_VisibleFadeEnd > _VisibleFadeStart)
                        alpha *= saturate((zNorm - _VisibleFadeStart) / (_VisibleFadeEnd - _VisibleFadeStart));
                }

                return half4(_FogColor.rgb, alpha * _FogColor.a);
            }
            ENDHLSL
        }
    }
}