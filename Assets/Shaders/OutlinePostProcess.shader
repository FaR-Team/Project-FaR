Shader "FaRTeam/OutlinePostProcess"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0.6, 0, 0.6, 1)
        _OutlineThickness ("Outline Thickness", Range(1, 5)) = 1
        _DepthThreshold ("Depth Threshold", Range(0.001, 1)) = 0.1
        _DepthSensitivity ("Depth Sensitivity", Range(0.1, 10)) = 1
        _NormalThreshold ("Normal Threshold", Range(0.1, 1)) = 0.4
        _MaxDistance ("Max Distance", Range(10, 200)) = 100
        [Toggle] _UseAdaptiveThreshold ("Use Adaptive Threshold", Float) = 1
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
                float _DepthThreshold;
                float _DepthSensitivity;
                float _NormalThreshold;
                float _MaxDistance;
                float _UseAdaptiveThreshold;
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }
            
            float SampleDepth(float2 uv)
            {
                float rawDepth = SampleSceneDepth(uv);
                return LinearEyeDepth(rawDepth, _ZBufferParams);
            }
            
            float3 SampleNormal(float2 uv)
            {
                return SampleSceneNormals(uv);
            }
            
            // Sobel edge detection for depth
            float SobelDepth(float2 uv, float2 texelSize)
            {
                float tl = SampleDepth(uv + float2(-texelSize.x, texelSize.y));   // top left
                float tm = SampleDepth(uv + float2(0, texelSize.y));             // top middle
                float tr = SampleDepth(uv + float2(texelSize.x, texelSize.y));   // top right
                float ml = SampleDepth(uv + float2(-texelSize.x, 0));            // middle left
                float mr = SampleDepth(uv + float2(texelSize.x, 0));             // middle right
                float bl = SampleDepth(uv + float2(-texelSize.x, -texelSize.y)); // bottom left
                float bm = SampleDepth(uv + float2(0, -texelSize.y));            // bottom middle
                float br = SampleDepth(uv + float2(texelSize.x, -texelSize.y));  // bottom right
                
                float sobelX = (tr + 2.0 * mr + br) - (tl + 2.0 * ml + bl);
                float sobelY = (tl + 2.0 * tm + tr) - (bl + 2.0 * bm + br);
                
                return sqrt(sobelX * sobelX + sobelY * sobelY);
            }
            
            // Sobel edge detection for normals
            float SobelNormal(float2 uv, float2 texelSize)
            {
                float3 tl = SampleNormal(uv + float2(-texelSize.x, texelSize.y));
                float3 tm = SampleNormal(uv + float2(0, texelSize.y));
                float3 tr = SampleNormal(uv + float2(texelSize.x, texelSize.y));
                float3 ml = SampleNormal(uv + float2(-texelSize.x, 0));
                float3 mr = SampleNormal(uv + float2(texelSize.x, 0));
                float3 bl = SampleNormal(uv + float2(-texelSize.x, -texelSize.y));
                float3 bm = SampleNormal(uv + float2(0, -texelSize.y));
                float3 br = SampleNormal(uv + float2(texelSize.x, -texelSize.y));
                
                float3 sobelX = (tr + 2.0 * mr + br) - (tl + 2.0 * ml + bl);
                float3 sobelY = (tl + 2.0 * tm + tr) - (bl + 2.0 * bm + br);
                
                return length(sobelX) + length(sobelY);
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                float2 texelSize = _MainTex_TexelSize.xy * _OutlineThickness;
                float centerDepth = SampleDepth(input.uv);
                
                // Distance-based fade
                float fadeDistance = saturate(centerDepth / _MaxDistance);
                if (fadeDistance > 0.95) return col;
                
                // Use Sobel edge detection for better edge detection
                float depthEdge = SobelDepth(input.uv, texelSize);
                float normalEdge = SobelNormal(input.uv, texelSize);
                
                // Adaptive threshold based on distance
                float adaptiveDepthThreshold = _DepthThreshold;
                if (_UseAdaptiveThreshold > 0.5)
                {
                    // Closer objects need lower threshold, farther objects need higher
                    adaptiveDepthThreshold = _DepthThreshold * (1.0 + centerDepth * _DepthSensitivity * 0.1);
                }
                
                // Combine depth and normal edge detection
                bool isDepthEdge = depthEdge > adaptiveDepthThreshold;
                bool isNormalEdge = normalEdge > _NormalThreshold;
                
                // Use either depth or normal edges
                bool isBorder = isDepthEdge || isNormalEdge;
                
                // Smooth the outline with distance fade
                float outlineStrength = 1.0 - fadeDistance;
                
                return isBorder ? lerp(col, _OutlineColor, _OutlineColor.a * outlineStrength) : col;
            }
            ENDHLSL
        }
    }
}