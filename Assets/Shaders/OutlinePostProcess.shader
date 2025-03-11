Shader "FaRTeam/OutlinePostProcess"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0.6, 0, 0.6, 1)
        _OutlineThickness ("Outline Thickness", Range(0, 10)) = 1
        _DepthThreshold ("Depth Threshold", Range(0, 1)) = 0.1
        _NormalThreshold ("Normal Threshold", Range(0, 1)) = 0.4
        _DistanceCutoff ("Distance Cutoff", Range(0, 100)) = 50
        _FadeDistance ("Fade Distance", Range(0, 20)) = 5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
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
            float4 _OutlineColor;
            float _OutlineThickness;
            float _DepthThreshold;
            float _DistanceCutoff;
            float _FadeDistance;
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }
            
            // Sample a cross pattern around the pixel for better edge detection
            float SampleDepth(float2 uv)
            {
                return LinearEyeDepth(SampleSceneDepth(uv), _ZBufferParams);
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                float2 texelSize = _MainTex_TexelSize.xy * _OutlineThickness;
                
                // Use a cross pattern for sampling
                float depth = SampleDepth(input.uv);
                float depthLeft = SampleDepth(input.uv - float2(texelSize.x, 0));
                float depthRight = SampleDepth(input.uv + float2(texelSize.x, 0));
                float depthUp = SampleDepth(input.uv + float2(0, texelSize.y));
                float depthDown = SampleDepth(input.uv - float2(0, texelSize.y));
                
                // Calculate relative depth difference
                float threshold = _DepthThreshold;
                
                // Check if we're at a depth discontinuity (object edge)
                bool isBorder = false;
                
                float maxDiff = 0;
                
                // Compare with neighbors and find max difference
                // For skybox neighbors, this will create a large difference
                maxDiff = max(maxDiff, abs(depth - depthLeft));
                maxDiff = max(maxDiff, abs(depth - depthRight));
                maxDiff = max(maxDiff, abs(depth - depthUp));
                maxDiff = max(maxDiff, abs(depth - depthDown));
                
                // Adjusted threshold for skybox boundaries
                // If any neighbor is skybox (very large depth), we should detect an edge
                isBorder = maxDiff > threshold * depth;
                
                // Distance cutoff - fade outline as it approaches cutoff distance
                float distanceFactor = 1.0;
                if (depth > _DistanceCutoff - _FadeDistance)
                {
                    distanceFactor = saturate((_DistanceCutoff - depth) / _FadeDistance);
                }
                
                // Apply distance cutoff - only show outline if within cutoff distance
                if (depth > _DistanceCutoff || distanceFactor <= 0)
                {
                    return col;
                }
                
                // Apply outline with distance-based fade
                return isBorder ? lerp(col, _OutlineColor, distanceFactor) : col;
            }
            ENDHLSL
        }
    }
}