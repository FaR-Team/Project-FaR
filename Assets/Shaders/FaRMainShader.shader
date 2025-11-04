Shader "FaRTeam/FaRMainShaderURP"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _CelSteps ("Cel Shading Steps", Range(1, 20)) = 5
        _Alpha ("Alpha", Range(0,1)) = 1
        [Toggle] _UseOutline("Use Outline", Float) = 0
        _OutlineColor("Outline Color", Color) = (0.6,0,0.6,1)
        _OutlineWidth("Outline Width", Range(0, 100)) = 20
        _PulseSpeed ("Pulse Speed", Range(0, 10)) = 2.5
        _PulseMinWidth ("Pulse Min Width", Range(0, 100)) = 5
        _PulseMaxWidth ("Pulse Max Width", Range(0, 100)) = 20
        [Toggle] _UseMultiplyTexture("Use Multiply Texture", Float) = 0
        _MultiplyTex ("Multiply Texture", 2D) = "white" {}
        [Header(Pixel Perfect Shadows)]
        [Toggle] _UsePixelPerfectShadows("Use Pixel Perfect Shadows", Float) = 1
        _ShadowThreshold("Shadow Threshold", Range(0, 1)) = 0.5
        _ShadowSharpness("Shadow Sharpness", Range(0.01, 0.5)) = 0.1
        _ShadowColor("Shadow Color", Color) = (0.5, 0.5, 0.7, 1)
        _ShadowAlignmentX("Shadow Alignment X", Range(-1, 1)) = 0
        _ShadowAlignmentssssY("Shadow Alignment Y", Range(-1, 1)) = 0.5001
        _ShadowAlignmentZ("Shadow Alignment Z", Range(-1, 1)) = 0
        _ShadowDepthBias("Shadow Depth Bias", Range(0, 0.01)) = 0.00001
        _GridOffsetX("Grid Offset X", Range(-2.1, 2.1)) = 0
        _GridOffsetY("Grid Offset Y", Range(-2.1, 2.1)) = 0
        _GridOffsetZ("Grid Offset Z", Range(-2.1, 2.1)) = 0
        [Toggle(_USE_FOG)] _UseFog ("Use Fog", Float) = 0
    }
    SubShader
    {
        Tags {
            "Queue" = "AlphaTest" 
            "RenderType" = "TransparentCutout" 
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
        }
        LOD 200
        
        Cull Off
        ZWrite On
        
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
        ENDHLSL
        
        Pass
        {
            Name "DepthOnly"
            Tags {"LightMode" = "DepthOnly"}
            
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _Alpha;
            CBUFFER_END
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            Varyings DepthOnlyVertex(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }
            
            half4 DepthOnlyFragment(Varyings input) : SV_TARGET
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half alpha = texColor.a * _Alpha;
                
                if (alpha < 0.01)
                    discard;
                    
                return 0;
            }
            ENDHLSL
        }
        
        Pass
        {
            Name "ShadowCaster"
            Tags {"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Off

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            #pragma multi_compile_instancing
            #pragma multi_compile _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _Alpha;
                float _UseMultiplyTexture;
                float4 _MultiplyTex_ST;
            CBUFFER_END

            float3 _LightDirection;
            float3 _LightPosition;

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float2 texcoord     : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv           : TEXCOORD0;
                float4 positionCS   : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            float4 GetShadowPositionHClip(Attributes input)
            {
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

            #if _CASTING_PUNCTUAL_LIGHT_SHADOW
                float3 lightDirectionWS = normalize(_LightPosition - positionWS);
            #else
                float3 lightDirectionWS = _LightDirection;
            #endif

                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

            #if UNITY_REVERSED_Z
                positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
            #else
                positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
            #endif

                return positionCS;
            }

            Varyings ShadowPassVertex(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
                output.positionCS = GetShadowPositionHClip(input);
                
                return output;
            }

            half4 ShadowPassFragment(Varyings input) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);
                
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half alpha = texColor.a * _Alpha;
                
                // Very low threshold to ensure maximum shadow casting
                if (alpha < 0.01)
                    discard;
                
                return 0;
            }
            ENDHLSL
        }

        Pass
        {
            Name "Outline"
            Tags { }
            Cull Front
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _UseOutline)
            UNITY_INSTANCING_BUFFER_END(Props)
            
            CBUFFER_START(UnityPerMaterial)
                float _OutlineWidth;
                float4 _OutlineColor;
                float _PulseSpeed;
                float _PulseMinWidth;
                float _PulseMaxWidth;
            CBUFFER_END
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                
                float useOutline = UNITY_ACCESS_INSTANCED_PROP(Props, _UseOutline);
                
                float pulseValue = (sin(_Time.y * _PulseSpeed) * 0.5 + 0.5);
                float pulseWidth = lerp(_PulseMinWidth, _PulseMaxWidth, pulseValue);
                
                float3 pos = IN.positionOS.xyz + IN.normalOS * (pulseWidth * 0.001 * useOutline);
                OUT.positionCS = TransformObjectToHClip(pos);
                return OUT;
            }            
            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                float useOutline = UNITY_ACCESS_INSTANCED_PROP(Props, _UseOutline);
                
                if (useOutline < 0.5)
                    discard;
                    
                return _OutlineColor;
            }
            ENDHLSL
        }
        Pass
        {
            Name "ForwardLit"
            Tags {"LightMode" = "UniversalForward"}
            
            ZWrite On
            ZTest LEqual
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma shader_feature_local _USE_FOG

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            TEXTURE2D(_MultiplyTex);
            SAMPLER(sampler_MainTex);
            
            // ---------- Fog globals (set from C# manager) ----------
float4 _FogColor;
float4x4 _FogWorldToLocal[4];
float _FadeStart; // normalized 0..1
float _FadeEnd;   // normalized 0..1
// ------------------------------------------------------

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _MainTex_ST;
                float _CelSteps;
                float _Alpha;
                float _UseMultiplyTexture;
                float4 _MultiplyTex_ST;
                float _UsePixelPerfectShadows;
                float _ShadowThreshold;
                float _ShadowSharpness;
                float4 _ShadowColor;
                float _PixelSized;
                float _ShadowAlignmentX;
                float _ShadowAlignmentssssY;
                float _ShadowAlignmentZ;
                float _ShadowDepthBias;
                float _GridOffsetX;
                float _GridOffsetY;
                float _GridOffsetZ;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionCS = TransformWorldToHClip(OUT.positionWS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.color = IN.color;
                return OUT;
            }
            
            half4 frag(Varyings IN) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Color * IN.color;
                half3 albedo = texColor.rgb;
                half alpha = texColor.a * _Alpha;
            
                float4 shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
                Light mainLight = GetMainLight(shadowCoord);
                
                float NdotL = dot(IN.normalWS, mainLight.direction) * 0.5 + 0.5;
                
                float shadowSample = MainLightRealtimeShadow(shadowCoord);
                float shadowAttenuation = 1.0;
                
                if (_UsePixelPerfectShadows > 0.5)
                {
                    float pixelSize = 0.1;
                    
                    float3 gridOffset = float3(_GridOffsetX, _GridOffsetY, _GridOffsetZ);
                    float3 offsetWorldPos = IN.positionWS + gridOffset;
                    
                    float3 alignmentOffset = float3(_ShadowAlignmentX * pixelSize, _ShadowAlignmentssssY * pixelSize, _ShadowAlignmentZ * pixelSize);
                    float3 alignedWorldPos = offsetWorldPos + alignmentOffset;
                    
                    float3 quantizedWorldPos = round(alignedWorldPos / pixelSize) * pixelSize;
                    
                    quantizedWorldPos -= gridOffset;
                    
                    // Improved bias calculation to prevent seams
                    float3 normalWS = normalize(IN.normalWS);
                    float NdotL_bias = dot(normalWS, mainLight.direction);
                    
                    // Surface-aware bias - more bias for surfaces facing away from light
                    float surfaceBias = (1.0 - abs(NdotL_bias)) * _ShadowDepthBias * 10.0;
                    
                    // Additional bias for steep surfaces to prevent seams
                    float steepnessBias = (1.0 - abs(normalWS.y)) * _ShadowDepthBias * 5.0;
                    
                    // Apply bias in light direction to prevent self-shadowing
                    quantizedWorldPos += mainLight.direction * (surfaceBias + steepnessBias + _ShadowDepthBias);
                    
                    float4 quantizedShadowCoord = TransformWorldToShadowCoord(quantizedWorldPos);
                    
                    float quantizedShadowSample = MainLightRealtimeShadow(quantizedShadowCoord);
                    
                    // Slightly softer threshold to reduce harsh seams
                    float threshold = _ShadowThreshold;
                    shadowAttenuation = quantizedShadowSample > threshold ? 1.0 : 0.0;
                    
                    // Optional: Add tiny bit of smoothing for very harsh edges
                    float smoothRange = 0.02;
                    shadowAttenuation = smoothstep(threshold - smoothRange, threshold + smoothRange, quantizedShadowSample);
                    shadowAttenuation = shadowAttenuation > 0.5 ? 1.0 : 0.0; // Keep it binary but smoother
                }
                else
                {
                    float softShadow = smoothstep(0.2, 0.8, shadowSample);
                    shadowAttenuation = lerp(0.7, 1.0, softShadow);
                }

                float ambientOcclusion = 1.0;

                float cel;
                
                if (_UsePixelPerfectShadows > 0.5)
                {
                    // For pixel perfect shadows, use pure binary decision - no NdotL mixing
                    cel = shadowAttenuation;
                }
                else
                {
                    float celValue = NdotL * shadowAttenuation;
                    cel = smoothstep(0, 1, frac(celValue * _CelSteps)) + floor(celValue * _CelSteps);
                    cel /= _CelSteps;
                }

                half3 litTint = mainLight.color.rgb;
                half3 shadowTint;
                
                if (_UsePixelPerfectShadows > 0.5)
                {
                    shadowTint = _ShadowColor.rgb * mainLight.color.rgb;
                }
                else
                {
                    shadowTint = mainLight.color.rgb * half3(0.8, 0.85, 1.0);
                }

                half3 lightingTint = lerp(shadowTint, litTint, cel);

                half4 multiplyTex = SAMPLE_TEXTURE2D(_MultiplyTex, sampler_MainTex, TRANSFORM_TEX(IN.uv, _MultiplyTex));
                albedo = lerp(albedo, albedo * multiplyTex.rgb, _UseMultiplyTexture);

                half4 finalColor;
                finalColor.rgb = albedo * lightingTint * ambientOcclusion;
                finalColor.a = alpha;
            
                if (finalColor.a < 0.01)
                    discard;

                #ifdef _USE_FOG
    // ----------------- Fog blending (4 walls) -----------------
    float maxFog = 0.0;

    // Transform world position to each fog local space and compute a normalized Z
    for (int i = 0; i < 4; i++)
    {
        // pos in fog local space
        float3 posLS = mul(_FogWorldToLocal[i], float4(IN.positionWS, 1)).xyz;

        // check XY bounds: default cube extents are -0.5..+0.5 => inside box if abs(x) <= 0.5 && abs(y) <= 0.5
        // you can tweak these checks if your fog mesh differs (or remove check to make infinite plane)
        if (abs(posLS.x) <= 0.5 && abs(posLS.y) <= 0.5 && posLS.z >= -0.5 && posLS.z <= 0.5)
        {
            // normalize Z to 0..1 (local -0.5..0.5 -> 0..1)
            float zNorm = saturate(posLS.z + 0.5);

            // compute fog contribution from this wall
            float f = 0.0;
            if (_FadeEnd > _FadeStart) // avoid div-by-zero
                f = saturate((zNorm - _FadeStart) / (_FadeEnd - _FadeStart));

            // take max across walls
            maxFog = max(maxFog, f);
        }
    }
                finalColor.rgb = lerp(finalColor.rgb, _FogColor.rgb, maxFog);
#endif
            
                return finalColor;
            }
            ENDHLSL        
        }
    }
}