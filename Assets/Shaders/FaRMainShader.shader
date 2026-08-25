Shader "FaRTeam/FaRMainShaderURP"
{
    Properties
    {
        [Header(Main Surface)]
        [MainColor] _Color ("Color", Color) = (1,1,1,1)
        [MainTexture] _MainTex ("Texture", 2D) = "white" {}
        _Alpha ("Alpha", Range(0, 1)) = 1

        [Header(Cel Shading)]
        _CelSteps ("Cel Shading Steps", Range(1, 20)) = 5
        [Toggle(_USE_RAMP_TEXTURE)] _UseRampTexture ("Use Ramp Texture", Float) = 0
        [NoScaleOffset] _RampTex ("Ramp Texture", 2D) = "gray" {}

        [Header(Multiply Texture)]
        [Toggle(_USE_MULTIPLY_TEXTURE)] _UseMultiplyTexture ("Use Multiply Texture", Float) = 0
        _MultiplyTex ("Multiply Texture", 2D) = "white" {}

        [Header(Outline)]
        [Toggle(_USE_OUTLINE)] _UseOutline ("Use Outline", Float) = 0
        [Toggle(_USE_SCREEN_SPACE_OUTLINE)] _UseScreenSpaceOutline ("Screen Space Outline Width", Float) = 0
        _OutlineColor ("Outline Color", Color) = (0.6, 0, 0.6, 1)
        _OutlineWidth ("Outline Width", Range(0, 100)) = 20
        _PulseSpeed ("Pulse Speed", Range(0, 10)) = 2.5
        _PulseMinWidth ("Pulse Min Width", Range(0, 100)) = 5
        _PulseMaxWidth ("Pulse Max Width", Range(0, 100)) = 20

        [Header(Pixel Perfect Shadows)]
        [Toggle(_USE_PIXEL_PERFECT_SHADOWS)] _UsePixelPerfectShadows ("Use Pixel Perfect Shadows", Float) = 1
        _ShadowThreshold ("Shadow Threshold", Range(0, 1)) = 0.5
        _ShadowSharpness ("Shadow Sharpness", Range(0.001, 0.5)) = 0.01
        _ShadowColor ("Shadow Color", Color) = (0.5, 0.5, 0.7, 1)
        _ShadowAlignmentX ("Shadow Alignment X", Range(-1, 1)) = 0
        _ShadowAlignmentY ("Shadow Alignment Y", Range(-1, 1)) = 0.5001
        _ShadowAlignmentZ ("Shadow Alignment Z", Range(-1, 1)) = 0
        _ShadowGridBias ("Shadow Grid Bias", Range(0, 1)) = 0.1
        _ShadowNormalBias ("Shadow Normal Bias", Range(0, 1)) = 0.1
        _GridOffsetX ("Grid Offset X", Range(-2.1, 2.1)) = 0
        _GridOffsetY ("Grid Offset Y", Range(-2.1, 2.1)) = 0
        _GridOffsetZ ("Grid Offset Z", Range(-2.1, 2.1)) = 0
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

        CBUFFER_START(UnityPerMaterial)
            float4 _Color;
            float4 _MainTex_ST;
            float _CelSteps;
            float _Alpha;
            float _UseRampTexture;
            float _UseMultiplyTexture;
            float4 _MultiplyTex_ST;
            float _UseOutline;
            float _UseScreenSpaceOutline;
            float4 _OutlineColor;
            float _OutlineWidth;
            float _PulseSpeed;
            float _PulseMinWidth;
            float _PulseMaxWidth;
            float _UsePixelPerfectShadows;
            float _ShadowThreshold;
            float _ShadowSharpness;
            float4 _ShadowColor;
            float _ShadowAlignmentX;
            float _ShadowAlignmentY;
            float _ShadowAlignmentZ;
            float _ShadowGridBias;
            float _ShadowNormalBias;
            float _GridOffsetX;
            float _GridOffsetY;
            float _GridOffsetZ;
        CBUFFER_END

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_MultiplyTex);
        TEXTURE2D(_RampTex);
        SAMPLER(sampler_RampTex);
        ENDHLSL

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            Cull Off
            ZWrite On
            ZTest LEqual
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #pragma shader_feature_local _USE_RAMP_TEXTURE
            #pragma shader_feature_local _USE_MULTIPLY_TEXTURE
            #pragma shader_feature_local _USE_PIXEL_PERFECT_SHADOWS

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float3 normalOS   : NORMAL;
                float4 color      : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv         : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                float3 normalWS   : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float4 color      : COLOR;
                float fogFactor   : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);

                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionCS = TransformWorldToHClip(OUT.positionWS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.color = IN.color;
                OUT.fogFactor = ComputeFogFactor(OUT.positionCS.z);
                return OUT;
            }
            
            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Color * IN.color;
                half3 albedo = texColor.rgb;
                half alpha = texColor.a * _Alpha;

                clip(alpha - 0.01f);
            
                float3 normalWS = normalize(IN.normalWS);
                float3 viewDirWS = GetWorldSpaceNormalizeViewDir(IN.positionWS);
                normalWS = dot(normalWS, viewDirWS) < 0.0 ? -normalWS : normalWS;

                float4 shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
                Light mainLight = GetMainLight(shadowCoord);
                
                float NdotL = saturate(dot(normalWS, mainLight.direction) * 0.5 + 0.5);
                
                float shadowAttenuation = 1.0;
                
                bool usePixelPerfect = _UsePixelPerfectShadows > 0.5;
            #if defined(_USE_PIXEL_PERFECT_SHADOWS)
                usePixelPerfect = true;
            #endif

                if (usePixelPerfect)
                {
                    float pixelSize = 0.1;
                    
                    float3 gridBias = normalWS * (pixelSize * _ShadowNormalBias);
                    float3 biasedPosWS = IN.positionWS + gridBias;
                    
                    float3 gridOffset = float3(_GridOffsetX, _GridOffsetY, _GridOffsetZ);
                    float3 offsetWorldPos = biasedPosWS + gridOffset;
                    
                    float3 alignmentOffset = float3(_ShadowAlignmentX * pixelSize, _ShadowAlignmentY * pixelSize, _ShadowAlignmentZ * pixelSize);
                    float3 alignedWorldPos = offsetWorldPos + alignmentOffset;
                    
                    float3 quantizedWorldPos = round(alignedWorldPos / pixelSize) * pixelSize;
                    quantizedWorldPos -= gridOffset;
                    quantizedWorldPos -= mainLight.direction * (pixelSize * _ShadowGridBias);
                    
                    float4 quantizedShadowCoord = TransformWorldToShadowCoord(quantizedWorldPos);
                    float quantizedShadowSample = MainLightRealtimeShadow(quantizedShadowCoord);
                    
                    float shadowStrength = _MainLightShadowParams.x;
                    float rawShadow = 1.0;
                    if (shadowStrength > 0.01)
                    {
                        rawShadow = saturate((quantizedShadowSample - (1.0 - shadowStrength)) / shadowStrength);
                    }
                    
                    float threshold = _ShadowThreshold;
                    float sharpness = max(_ShadowSharpness, 0.001);
                    shadowAttenuation = smoothstep(threshold - sharpness, threshold + sharpness, rawShadow);
                }
                else
                {
                    float shadowSample = MainLightRealtimeShadow(shadowCoord);
                    float softShadow = smoothstep(0.2, 0.8, shadowSample);
                    shadowAttenuation = lerp(0.7, 1.0, softShadow);
                }

                bool useRamp = _UseRampTexture > 0.5;
            #if defined(_USE_RAMP_TEXTURE)
                useRamp = true;
            #endif

                float steps = max(_CelSteps, 1.0);
                float cel;
                
                if (usePixelPerfect)
                {
                    cel = lerp(1.0, shadowAttenuation, _MainLightShadowParams.x);
                }
                else if (useRamp)
                {
                    float rampUV = saturate(NdotL * shadowAttenuation);
                    cel = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, float2(rampUV, 0.5)).r;
                }
                else
                {
                    float celValue = NdotL * shadowAttenuation;
                    cel = smoothstep(0.0, 1.0, frac(celValue * steps)) + floor(celValue * steps);
                    cel /= steps;
                }

                half3 litTint = mainLight.color.rgb;
                half3 shadowTint;
                
                if (usePixelPerfect)
                {
                    shadowTint = _ShadowColor.rgb * mainLight.color.rgb;
                }
                else
                {
                    shadowTint = mainLight.color.rgb * half3(0.8, 0.85, 1.0);
                }

                half3 lightingTint = lerp(shadowTint, litTint, cel);

                half3 additionalLighting = 0;
            #if defined(_ADDITIONAL_LIGHTS)
                uint pixelLightCount = GetAdditionalLightsCount();
                for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
                {
                    Light addLight = GetAdditionalLight(lightIndex, IN.positionWS);
                    float addNdotL = saturate(dot(normalWS, addLight.direction) * 0.5 + 0.5);
                    float addAtten = addLight.distanceAttenuation * addLight.shadowAttenuation;
                    
                    float addCel;
                    if (useRamp)
                    {
                        addCel = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, float2(addNdotL * addAtten, 0.5)).r;
                    }
                    else
                    {
                        float addVal = addNdotL * addAtten;
                        addCel = smoothstep(0.0, 1.0, frac(addVal * steps)) + floor(addVal * steps);
                        addCel /= steps;
                    }
                    additionalLighting += addLight.color.rgb * addCel;
                }
            #endif

                bool useMultiply = _UseMultiplyTexture > 0.5;
            #if defined(_USE_MULTIPLY_TEXTURE)
                useMultiply = true;
            #endif
                if (useMultiply)
                {
                    half4 multiplyTex = SAMPLE_TEXTURE2D(_MultiplyTex, sampler_MainTex, TRANSFORM_TEX(IN.uv, _MultiplyTex));
                    albedo *= multiplyTex.rgb;
                }

                half4 finalColor;
                finalColor.rgb = albedo * (lightingTint + additionalLighting);
                finalColor.a = alpha;

                finalColor.rgb = MixFog(finalColor.rgb, IN.fogFactor);
            
                return finalColor;
            }
            ENDHLSL        
        }
        
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Off

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            #pragma multi_compile_instancing
            #pragma multi_compile _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            float3 _LightDirection;
            float3 _LightPosition;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 texcoord   : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv         : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            float4 GetShadowPositionHClip(Attributes input)
            {
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

            #if _CASTING_PUNCTUAL_LIGHT_SHADOW
                float3 lightVec = _LightPosition - positionWS;
                float lenSq = dot(lightVec, lightVec);
                float3 lightDirectionWS = lenSq > 1e-6 ? lightVec * rsqrt(lenSq) : float3(0, 1, 0);
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
                half alpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).a * _Alpha * _Color.a;
                clip(alpha - 0.01f);
                return 0;
            }
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
            
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            #pragma multi_compile_instancing
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            Varyings DepthOnlyVertex(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(positionWS);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }
            
            half4 DepthOnlyFragment(Varyings input) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);
                half alpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).a * _Alpha * _Color.a;
                clip(alpha - 0.01f);
                return 0;
            }
            ENDHLSL
        }

        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "SRPDefaultUnlit" }
            Cull Front
            ZWrite Off
            ZTest LEqual
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma shader_feature_local _USE_OUTLINE
            #pragma shader_feature_local _USE_SCREEN_SPACE_OUTLINE
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                
                bool useOutline = _UseOutline > 0.5;
            #if defined(_USE_OUTLINE)
                useOutline = true;
            #endif

                if (!useOutline)
                {
                    OUT.positionCS = 0;
                    return OUT;
                }

                float pulseValue = sin(_Time.y * _PulseSpeed) * 0.5 + 0.5;
                float pulseWidth = lerp(_PulseMinWidth, _PulseMaxWidth, pulseValue);
                
                bool isScreenSpace = _UseScreenSpaceOutline > 0.5;
            #if defined(_USE_SCREEN_SPACE_OUTLINE)
                isScreenSpace = true;
            #endif

                if (isScreenSpace)
                {
                    OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                    float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
                    float3 normalCS = TransformWorldToHClipDir(normalWS);
                    float lenSq = dot(normalCS.xy, normalCS.xy);
                    float2 normXY = lenSq > 1e-6 ? normalCS.xy * rsqrt(lenSq) : float2(0, 0);
                    float2 offset = normXY * (pulseWidth * 0.0005) * OUT.positionCS.w;
                    OUT.positionCS.xy += offset;
                }
                else
                {
                    float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
                    float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz) + normalWS * (pulseWidth * 0.001);
                    OUT.positionCS = TransformWorldToHClip(positionWS);
                }
                
                return OUT;
            }            
            
            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                
                bool useOutline = _UseOutline > 0.5;
            #if defined(_USE_OUTLINE)
                useOutline = true;
            #endif

                if (!useOutline)
                    discard;
                    
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}