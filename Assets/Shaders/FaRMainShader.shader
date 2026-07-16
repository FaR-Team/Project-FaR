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
        _ShadowAlignmentY("Shadow Alignment Y", Range(-1, 1)) = 0.5001
        _ShadowAlignmentZ("Shadow Alignment Z", Range(-1, 1)) = 0
        _ShadowGridBias("Shadow Grid Bias", Range(0, 1)) = 0.1
        _ShadowNormalBias("Shadow Normal Bias", Range(0, 1)) = 0.1
        _GridOffsetX("Grid Offset X", Range(-2.1, 2.1)) = 0
        _GridOffsetY("Grid Offset Y", Range(-2.1, 2.1)) = 0
        _GridOffsetZ("Grid Offset Z", Range(-2.1, 2.1)) = 0
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
            float _UseOutline;
            float4 _OutlineColor;
            float _OutlineWidth;
            float _PulseSpeed;
            float _PulseMinWidth;
            float _PulseMaxWidth;
            float _UseMultiplyTexture;
            float4 _MultiplyTex_ST;
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
            #pragma multi_compile_instancing
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            Varyings DepthOnlyVertex(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }
            
            half4 DepthOnlyFragment(Varyings input) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);
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
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                
                float useOutline = _UseOutline;
                
                float pulseValue = (sin(_Time.y * _PulseSpeed) * 0.5 + 0.5);
                float pulseWidth = lerp(_PulseMinWidth, _PulseMaxWidth, pulseValue);
                
                float3 pos = IN.positionOS.xyz + IN.normalOS * (pulseWidth * 0.001 * useOutline);
                OUT.positionCS = TransformObjectToHClip(pos);
                return OUT;
            }            
            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                float useOutline = _UseOutline;
                
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
            
            Cull Off
            ZWrite On
            ZTest LEqual
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float4 color : COLOR;
                float fogFactor : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            TEXTURE2D(_MainTex);
            TEXTURE2D(_MultiplyTex);
            SAMPLER(sampler_MainTex);

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
            
            half4 frag(Varyings IN, bool isFrontFace : SV_IsFrontFace) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Color * IN.color;
                half3 albedo = texColor.rgb;
                half alpha = texColor.a * _Alpha;
            
                float3 normalWS = normalize(IN.normalWS);
                normalWS = isFrontFace ? normalWS : -normalWS;

                float4 shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
                Light mainLight = GetMainLight(shadowCoord);
                
                float NdotL = dot(normalWS, mainLight.direction) * 0.5 + 0.5;
                
                float shadowAttenuation = 1.0;
                
                if (_UsePixelPerfectShadows > 0.5)
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
                    
                    // Move point towards light source slightly to avoid self-shadowing acne after quantization
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
                    shadowAttenuation = rawShadow > threshold ? 1.0 : 0.0;
                }
                else
                {
                    float shadowSample = MainLightRealtimeShadow(shadowCoord);
                    float softShadow = smoothstep(0.2, 0.8, shadowSample);
                    shadowAttenuation = lerp(0.7, 1.0, softShadow);
                }

                float ambientOcclusion = 1.0;

                float cel;
                
                if (_UsePixelPerfectShadows > 0.5)
                {
                    cel = lerp(1.0, shadowAttenuation, _MainLightShadowParams.x);
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

                if (_UseMultiplyTexture > 0.5)
                {
                    half4 multiplyTex = SAMPLE_TEXTURE2D(_MultiplyTex, sampler_MainTex, TRANSFORM_TEX(IN.uv, _MultiplyTex));
                    albedo *= multiplyTex.rgb;
                }

                half4 finalColor;
                finalColor.rgb = albedo * lightingTint * ambientOcclusion;
                finalColor.a = alpha;
            
                if (finalColor.a < 0.01)
                    discard;

                finalColor.rgb = MixFog(finalColor.rgb, IN.fogFactor);
            
                return finalColor;
            }
            ENDHLSL        
        }
    }
}