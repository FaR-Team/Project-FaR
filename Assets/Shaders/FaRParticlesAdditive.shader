Shader "FaRTeam/FaRParticlesAdditive"
{
    Properties
    {
        _MainTex ("Particle Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _EmissionStrength ("Emission Strength", Range(0, 10)) = 1
        [Toggle] _VertexColorMultiplier ("Use Vertex Color", Float) = 1
        [Toggle] _SoftParticles ("Soft Particles", Float) = 0
        _SoftParticlesDistance ("Soft Particles Distance", Range(0.01, 3.0)) = 0.5
    }
    
    SubShader
    {
        Tags { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent+100" 
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
        }
        
        Blend SrcAlpha One
        ZWrite Off
        ZTest LEqual
        Cull Off
        
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        ENDHLSL
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma shader_feature_local _SOFTPARTICLES_ON
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float fogFactor : TEXCOORD1;
                float4 projPos : TEXCOORD2;
                float3 positionWS : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _EmissionStrength;
                float _VertexColorMultiplier;
                float _SoftParticles;
                float _SoftParticlesDistance;
            CBUFFER_END
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.projPos = ComputeScreenPos(OUT.positionCS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                // Reduce intensity for rain particles
                float4 rainColor = IN.color * _Color;
                rainColor.a *= 0.7; // Reduced alpha for less visibility
                OUT.color = rainColor * lerp(float4(1,1,1,1), IN.color, _VertexColorMultiplier);
                OUT.fogFactor = ComputeFogFactor(OUT.positionCS.z);
                
                return OUT;
            }
            
            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
                
                // Sample texture
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                
                // Apply color and emission
                half4 finalColor = texColor * IN.color * (_EmissionStrength * 0.8);
                
                // Make rain less strong
                finalColor.rgb *= 0.8;
                finalColor.a *= 0.7;
                
                // Soft particles effect (optional)
                #if defined(_SOFTPARTICLES_ON)
                if (_SoftParticles > 0.5) {
                    float sceneZ = LinearEyeDepth(SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, IN.projPos.xy / IN.projPos.w).r, _ZBufferParams);
                    float partZ = IN.projPos.z;
                    float fade = saturate((sceneZ - partZ) / _SoftParticlesDistance);
                    finalColor.a *= fade;
                }
                #endif
                
                // Apply fog
                finalColor.rgb = MixFog(finalColor.rgb, IN.fogFactor);
                
                return finalColor;
            }
            ENDHLSL
        }
    }
    
    CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
}