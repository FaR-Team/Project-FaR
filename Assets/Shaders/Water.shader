Shader "FaRTeam/Water"
{
	Properties
	{
		_DepthGradientShallow("Shallow Depth Gradient", Color) = (0.325, 0.807, 0.971, 0.725)
		_DepthGradientDeep("Deep Depth Gradient", Color) = (0.086, 0.407, 1, 0.749)
		_DepthMaxDistance("Depth Maximum Distance", Float) = 1
		_FoamColor("Foam Color", Color) = (1,1,1,1)
		_SurfaceNoise("Surface Noise", 2D) = "white" {}
		_SurfaceNoiseScroll("Surface Noise Scroll", Vector) = (0.03, 0.03, 0, 0)
		_SurfaceNoiseCutoff("Surface Noise Cutoff", Range(0, 1)) = 0.7
		_SurfaceDistortion("Surface Distortion", 2D) = "white" {}    
		_SurfaceDistortionAmount("Surface Distortion Amount", Range(0, 1)) = 0.8
		_FoamMaxDistance("Foam Maximum Distance", Float) = 3
		_FoamMinDistance("Foam Minimum Distance", Float) = 0.04
		_WaveSpeed("Wave Speed", Float) = 1
		_WaveAmplitude("Wave Amplitude", Float) = 0.5
		_WaveFrequency("Wave Frequency", Float) = 2
	}
	
	SubShader
	{
		Tags {"RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent"}
		
		Pass
		{
			Name "ForwardLit"
			Tags {"LightMode" = "UniversalForward"}
			
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _SHADOWS_SOFT
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
			
			struct Attributes
			{
				float4 positionOS : POSITION;
				float2 uv : TEXCOORD0;
				float3 normalOS : NORMAL;
			};
			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float2 noiseUV : TEXCOORD0;
				float2 distortUV : TEXCOORD1;
				float4 screenPosition : TEXCOORD2;
				float3 viewNormal : NORMAL;
				float3 normalWS : TEXCOORD3;
				float3 positionWS : TEXCOORD4;
			};
			
			TEXTURE2D(_SurfaceNoise); SAMPLER(sampler_SurfaceNoise);
			TEXTURE2D(_SurfaceDistortion); SAMPLER(sampler_SurfaceDistortion);
			
			CBUFFER_START(UnityPerMaterial)
				float4 _SurfaceNoise_ST;
				float4 _SurfaceDistortion_ST;
				float4 _DepthGradientShallow;
				float4 _DepthGradientDeep;
				float4 _FoamColor;
				float _DepthMaxDistance;
				float _FoamMaxDistance;
				float _FoamMinDistance;
				float _SurfaceNoiseCutoff;
				float _SurfaceDistortionAmount;
				float2 _SurfaceNoiseScroll;
				float _WaveSpeed;
				float _WaveAmplitude;
				float _WaveFrequency;
			CBUFFER_END
			
			Varyings vert(Attributes IN)
			{
				Varyings OUT;
				OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
				OUT.positionCS = TransformWorldToHClip(OUT.positionWS);
				OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
				OUT.screenPosition = ComputeScreenPos(OUT.positionCS);
				OUT.distortUV = TRANSFORM_TEX(IN.uv, _SurfaceDistortion);
				OUT.noiseUV = TRANSFORM_TEX(IN.uv, _SurfaceNoise);
				OUT.viewNormal = TransformWorldToViewDir(OUT.normalWS);
				return OUT;
			}
			
			float4 frag(Varyings IN) : SV_Target
			{
				float2 screenUV = IN.screenPosition.xy / IN.screenPosition.w;
				float sceneDepth = SampleSceneDepth(screenUV);
				float linearEyeDepth = LinearEyeDepth(sceneDepth, _ZBufferParams);
				float waterDepth = IN.screenPosition.w;
				
				float depthDifference = linearEyeDepth - waterDepth;
				
				float waterDepthDifference01 = saturate(depthDifference / _DepthMaxDistance);
				float4 waterColor = lerp(_DepthGradientShallow, _DepthGradientDeep, waterDepthDifference01);
				float4 shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
				Light mainLight = GetMainLight(shadowCoord);
				float NdotL = dot(IN.normalWS, mainLight.direction);

				float shadowSample = MainLightRealtimeShadow(shadowCoord);
				float softShadow = smoothstep(0.2, 0.8, shadowSample);
				float shadowAttenuation = lerp(0.5, 1.0, softShadow);

				float ambientOcclusion = lerp(0.8, 1.0, shadowSample);

				half3 litTint = mainLight.color.rgb * 1.2;
				half3 shadowTint = mainLight.color.rgb * half3(0.7, 0.8, 1.0);

				float celValue = NdotL * shadowAttenuation * ambientOcclusion;
				float cel = smoothstep(0, 1, celValue);

				half3 lightingTint = lerp(shadowTint, litTint, cel);
				waterColor.rgb *= lightingTint * ambientOcclusion;				
				// Improve foam calculation
				float foamDepth = saturate(depthDifference / _FoamMaxDistance);
				float foamGradient = 1 - foamDepth;
				float foamVisibility = smoothstep(_FoamMinDistance, _FoamMaxDistance, foamGradient);
				
				float surfaceNoiseCutoff = foamVisibility * _SurfaceNoiseCutoff;
				
				// Adjust distortion calculation
				float2 distortSample = (SAMPLE_TEXTURE2D(_SurfaceDistortion, sampler_SurfaceDistortion, IN.distortUV).xy * 2 - 1) * _SurfaceDistortionAmount;
				float2 objectOffset = IN.positionWS.xz * 0.1;
				float2 noiseUV = float2(
					(IN.noiseUV.x + _Time.y * _SurfaceNoiseScroll.x + objectOffset.x) + distortSample.x,
					(IN.noiseUV.y + _Time.y * _SurfaceNoiseScroll.y + objectOffset.y) + distortSample.y
				);
				
				float surfaceNoiseSample = SAMPLE_TEXTURE2D(_SurfaceNoise, sampler_SurfaceNoise, noiseUV).r;
				float surfaceNoise = smoothstep(surfaceNoiseCutoff - 0.01, surfaceNoiseCutoff + 0.01, surfaceNoiseSample);
				
				float4 surfaceNoiseColor = _FoamColor;
				surfaceNoiseColor.a *= surfaceNoise;
				
				return lerp(waterColor, surfaceNoiseColor, surfaceNoiseColor.a);
			}			
			ENDHLSL
		}
	}
}
