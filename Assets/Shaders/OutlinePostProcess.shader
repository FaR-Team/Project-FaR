Shader "FaRTeam/OutlinePostProcess"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineThickness ("Base Thickness (Near)", Range(0.5, 5.0)) = 2.0
        _ReferenceDistance ("Reference Distance (Meters)", Range(1.0, 50.0)) = 5.0
        _MinThickness ("Min Thickness (Far)", Range(0.0, 2.0)) = 0.2
        _DistanceFadeStart ("Distance Fade Start", Range(1.0, 300.0)) = 60.0
        _MaxDistance ("Max Distance", Range(10.0, 1000.0)) = 300.0
        _DepthThreshold ("Depth Threshold (Non-Touching)", Range(0.001, 0.5)) = 0.02
        _DepthSensitivity ("Depth Sensitivity", Range(0.1, 10.0)) = 2.0
        _NormalThreshold ("Normal Threshold (Creases)", Range(0.01, 1.0)) = 0.25
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        ZTest Always ZWrite Off Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;
            
            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float _OutlineThickness;
                float _ReferenceDistance;
                float _MinThickness;
                float _DistanceFadeStart;
                float _MaxDistance;
                float _DepthThreshold;
                float _DepthSensitivity;
                float _NormalThreshold;
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }
            
            float SampleInvDepth(float2 uv)
            {
                float rawDepth = SampleSceneDepth(uv);
                float linearDepth = LinearEyeDepth(rawDepth, _ZBufferParams);
                return 1.0 / max(0.0001, linearDepth);
            }
            
            float3 SampleNormal(float2 uv)
            {
                return SampleSceneNormals(uv);
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                float rawCenterDepth = SampleSceneDepth(input.uv);
                float centerDepth = LinearEyeDepth(rawCenterDepth, _ZBufferParams);
                
                if (centerDepth >= _MaxDistance)
                    return sceneColor;
                    
                float perspectiveScale = _ReferenceDistance / max(0.1, centerDepth);
                float currentThickness = max(_MinThickness, _OutlineThickness * saturate(perspectiveScale));
                float2 texel = _MainTex_TexelSize.xy * currentThickness;
                
                float2 uv0 = input.uv + float2(-texel.x, -texel.y);
                float2 uv1 = input.uv + float2( texel.x,  texel.y);
                float2 uv2 = input.uv + float2(-texel.x,  texel.y);
                float2 uv3 = input.uv + float2( texel.x, -texel.y);
                
                float i0 = SampleInvDepth(uv0);
                float i1 = SampleInvDepth(uv1);
                float i2 = SampleInvDepth(uv2);
                float i3 = SampleInvDepth(uv3);
                float ic = 1.0 / max(0.0001, centerDepth);
                
                float lap1 = abs(i1 + i0 - 2.0 * ic);
                float lap2 = abs(i3 + i2 - 2.0 * ic);
                float invDepthLaplacian = sqrt(lap1 * lap1 + lap2 * lap2);
                float isDepthEdge = step(_DepthThreshold, invDepthLaplacian * _DepthSensitivity);
                
                float3 n0 = SampleNormal(uv0);
                float3 n1 = SampleNormal(uv1);
                float3 n2 = SampleNormal(uv2);
                float3 n3 = SampleNormal(uv3);
                
                float3 nDiff1 = n1 - n0;
                float3 nDiff2 = n3 - n2;
                float normalEdge = sqrt(dot(nDiff1, nDiff1) + dot(nDiff2, nDiff2));
                float isNormalEdge = step(_NormalThreshold, normalEdge);
                
                float edge = max(isDepthEdge, isNormalEdge);
                
                float fadeFactor = 1.0 - smoothstep(_DistanceFadeStart, _MaxDistance, centerDepth);
                float outlineStrength = fadeFactor * _OutlineColor.a;
                
                return lerp(sceneColor, float4(_OutlineColor.rgb, sceneColor.a), edge * outlineStrength);
            }
            ENDHLSL
        }
    }
}