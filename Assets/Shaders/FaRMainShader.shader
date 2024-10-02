Shader "FaRTeam/FaRMainShaderURP"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _CelShadingBlurWidth ("Cel Shading Blur", Range(0,2)) = 0.2
        _Alpha ("Alpha", Range(0,1)) = 1
    }
    SubShader
    {
        Tags {"Queue" = "Geometry" "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 200
        
        Cull Off
        ZWrite On
        
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
        ENDHLSL
        
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
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
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _MainTex_ST;
                float _CelShadingBlurWidth;
                float _Alpha;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionCS = TransformWorldToHClip(OUT.positionWS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                return OUT;
            }
            
            half4 frag(Varyings IN) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Color;
                half3 albedo = texColor.rgb;
                half alpha = texColor.a * _Alpha;
            
                float4 shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
                Light mainLight = GetMainLight(shadowCoord);
                float NdotL = dot(IN.normalWS, mainLight.direction);

                float shadowSample = MainLightRealtimeShadow(shadowCoord);
                float softShadow = smoothstep(0.3, 0.7, shadowSample);
                float shadowAttenuation = lerp(0.5, 1.0, softShadow);

                float ambientOcclusion = lerp(0.8, 1.0, shadowSample);

                float cel = smoothstep(0.4, 0.6, NdotL * shadowAttenuation * ambientOcclusion);

                half3 litTint = half3(1.1, 1.05, 1.0);
                half3 shadowTint = half3(0.7, 0.8, 1.0);

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