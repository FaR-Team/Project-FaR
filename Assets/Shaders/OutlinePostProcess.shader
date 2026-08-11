Shader "FaRTeam/OutlinePostProcess" // Si alguien toca este código, 40 balazos
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineThickness ("Near Thickness", Range(0.5, 3.0)) = 1.2
        _ReferenceDistance ("Reference Distance", Range(1.0, 50.0)) = 15.0
        _MinThickness ("Min Thickness", Range(0.1, 1.0)) = 0.4
        _DistanceFadeStart ("Distance Fade Start", Range(10.0, 500.0)) = 300.0
        _MaxDistance ("Max Distance", Range(50.0, 2000.0)) = 1000.0
        _DepthThreshold ("Silhouette Depth Threshold", Range(0.005, 0.2)) = 0.01
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
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }
            
            float SampleLinearDepth(float2 uv)
            {
                float rawDepth = SampleSceneDepth(uv);
                return LinearEyeDepth(rawDepth, _ZBufferParams);
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                float rawCenterDepth = SampleSceneDepth(input.uv);
                float centerDepth = LinearEyeDepth(rawCenterDepth, _ZBufferParams);
                
                if (centerDepth >= _MaxDistance)
                    return sceneColor;
                    
                float depthScale = _ReferenceDistance / max(1.0, centerDepth);
                float currentThickness = clamp(_OutlineThickness * depthScale, _MinThickness, _OutlineThickness);
                float2 texel = _MainTex_TexelSize.xy * currentThickness;
                
                float dC = centerDepth;
                float dL  = SampleLinearDepth(input.uv + float2(-texel.x, 0.0));
                float dR  = SampleLinearDepth(input.uv + float2( texel.x, 0.0));
                float dT  = SampleLinearDepth(input.uv + float2(0.0,  texel.y));
                float dB  = SampleLinearDepth(input.uv + float2(0.0, -texel.y));
                
                float dTL = SampleLinearDepth(input.uv + float2(-texel.x,  texel.y));
                float dBR = SampleLinearDepth(input.uv + float2( texel.x, -texel.y));
                float dTR = SampleLinearDepth(input.uv + float2( texel.x,  texel.y));
                float dBL = SampleLinearDepth(input.uv + float2(-texel.x, -texel.y));
                
                float iC  = 1.0 / max(0.0001, dC);
                float iL  = 1.0 / max(0.0001, dL);
                float iR  = 1.0 / max(0.0001, dR);
                float iT  = 1.0 / max(0.0001, dT);
                float iB  = 1.0 / max(0.0001, dB);
                
                float iTL = 1.0 / max(0.0001, dTL);
                float iBR = 1.0 / max(0.0001, dBR);
                float iTR = 1.0 / max(0.0001, dTR);
                float iBL = 1.0 / max(0.0001, dBL);
                
                float varH  = abs(iR + iL - 2.0 * iC);
                float varV  = abs(iT + iB - 2.0 * iC);
                float varD1 = abs(iTR + iBL - 2.0 * iC);
                float varD2 = abs(iTL + iBR - 2.0 * iC);
                
                float maxSlopeVar = max(max(varH, varV), max(varD1, varD2)) * dC;
                
                float edge = smoothstep(_DepthThreshold, _DepthThreshold * 1.5, maxSlopeVar);
                
                float distAlpha = lerp(1.0, 0.35, saturate((centerDepth - 12.0) / 70.0));
                
                float fadeFactor = 1.0 - smoothstep(_DistanceFadeStart, _MaxDistance, centerDepth);
                float outlineStrength = fadeFactor * _OutlineColor.a * distAlpha;
                
                return lerp(sceneColor, float4(_OutlineColor.rgb, sceneColor.a), edge * outlineStrength);
            }
            ENDHLSL
        }
    }
}