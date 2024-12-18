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
    }
    SubShader
    {
        Tags {
            "Queue" = "Transparent" 
            "RenderType" = "Transparent" 
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent+50"
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
            Name "ShadowCaster"
            Tags {"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            ColorMask 0

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _Alpha;
            CBUFFER_END

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            float3 _LightDirection;

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float2 texcoord     : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv           : TEXCOORD0;
                float4 positionCS   : SV_POSITION;
            };

            Varyings ShadowPassVertex(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);

                output.uv = input.texcoord;
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
                return output;
            }

            half4 ShadowPassFragment(Varyings input) : SV_TARGET
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half alpha = texColor.a * _Alpha;
                
                if (alpha < 0.5)
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
                
                // Discard fragment if outline is not active
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
            
            Blend SrcAlpha OneMinusSrcAlpha
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT

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
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _MainTex_ST;
                float _CelSteps;
                float _Alpha;
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
                float softShadow = smoothstep(0.2, 0.8, shadowSample);
                float shadowAttenuation = lerp(0.7, 1.0, softShadow);

                float ambientOcclusion = lerp(0.9, 1.0, shadowSample);

                float celValue = NdotL * shadowAttenuation * ambientOcclusion;
                float cel = smoothstep(0, 1, frac(celValue * _CelSteps)) + floor(celValue * _CelSteps);
                cel /= _CelSteps;

                half3 litTint = mainLight.color.rgb;
                half3 shadowTint = mainLight.color.rgb * half3(0.8, 0.85, 1.0);

                half3 lightingTint = lerp(shadowTint, litTint, cel);

                half4 finalColor;
                finalColor.rgb = albedo * lightingTint * ambientOcclusion;
                finalColor.a = alpha;
            
                if (finalColor.a < 0.01)
                    discard;
            
                return finalColor;
            }
            ENDHLSL        
        }
    }
}