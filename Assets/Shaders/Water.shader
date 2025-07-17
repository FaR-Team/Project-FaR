Shader "FaRTeam/Water"
{
	Properties
	{
		_DepthGradientShallow("Shallow Depth Gradient", Color) = (0.325, 0.807, 0.971, 0.525)
		_DepthGradientDeep("Deep Depth Gradient", Color) = (0.086, 0.407, 1, 0.549)
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
		[Header(Cel Shading)]
		_CelSteps("Cel Shading Steps", Range(1, 20)) = 5
		_ShadowColor("Shadow Color", Color) = (0.5, 0.5, 0.7, 1)
		[Header(Pixel Perfect Shadows)]
		[Toggle] _UsePixelPerfectShadows("Use Pixel Perfect Shadows", Float) = 1
		_ShadowThreshold("Shadow Threshold", Range(0, 1)) = 0.5
		_ShadowDepthBias("Shadow Depth Bias", Range(0, 0.01)) = 0.00001
		_GridOffsetX("Grid Offset X", Range(-2.1, 2.1)) = 0
		_GridOffsetY("Grid Offset Y", Range(-2.1, 2.1)) = 0
		_GridOffsetZ("Grid Offset Z", Range(-2.1, 2.1)) = 0
		[Header(Lighting)]
		_Glossiness("Smoothness", Range(0,1)) = 0.8
		_Metallic("Metallic", Range(0,1)) = 0.0
		_LightIntensity("Light Intensity", Range(0,2)) = 1.0
	}
	
	SubShader
	{
		Tags {"RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent-100"}
		
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
			
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			
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
				output.uv = input.uv;
				return output;
			}
			
			half4 DepthOnlyFragment(Varyings input) : SV_TARGET
			{
				return 0;
			}
			ENDHLSL
		}
		
		Pass
		{
			Name "ForwardLit"
			Tags {"LightMode" = "UniversalForward"}
			
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Cull Off
			
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile_fragment _ _DEPTH_NO_MSAA
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
				float _LightIntensity;
				float _Glossiness;
				float _Metallic;
				float _CelSteps;
				float4 _ShadowColor;
				float _UsePixelPerfectShadows;
				float _ShadowThreshold;
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
				#if UNITY_REVERSED_Z
					float sceneDepth = SampleSceneDepth(screenUV);
				#else
					float sceneDepth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(screenUV));
				#endif
				float linearEyeDepth = LinearEyeDepth(sceneDepth, _ZBufferParams);
				float waterDepth = IN.screenPosition.w;
				float depthDifference = linearEyeDepth - waterDepth;
				float waterDepthDifference01 = saturate(depthDifference / _DepthMaxDistance);
				float4 waterColor = lerp(_DepthGradientShallow, _DepthGradientDeep, waterDepthDifference01);

				// Cel-shaded lighting matching main shader
				float4 shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
				Light mainLight = GetMainLight(shadowCoord);
				
				float3 normalWS = normalize(IN.normalWS);
				float NdotL = dot(normalWS, mainLight.direction) * 0.5 + 0.5;
				
				float shadowSample = MainLightRealtimeShadow(shadowCoord);
				float shadowAttenuation = 1.0;
				
				if (_UsePixelPerfectShadows > 0.5)
				{
					float pixelSize = 0.1;
					
					float3 gridOffset = float3(_GridOffsetX, _GridOffsetY, _GridOffsetZ);
					float3 offsetWorldPos = IN.positionWS + gridOffset;
					
					float3 quantizedWorldPos = round(offsetWorldPos / pixelSize) * pixelSize;
					quantizedWorldPos -= gridOffset;
					
					// Apply bias to prevent self-shadowing
					float3 normalWSBias = normalize(IN.normalWS);
					float NdotL_bias = dot(normalWSBias, mainLight.direction);
					float surfaceBias = (1.0 - abs(NdotL_bias)) * _ShadowDepthBias * 10.0;
					quantizedWorldPos += mainLight.direction * (surfaceBias + _ShadowDepthBias);
					
					float4 quantizedShadowCoord = TransformWorldToShadowCoord(quantizedWorldPos);
					float quantizedShadowSample = MainLightRealtimeShadow(quantizedShadowCoord);
					
					shadowAttenuation = quantizedShadowSample > _ShadowThreshold ? 1.0 : 0.0;
				}
				else
				{
					float softShadow = smoothstep(0.2, 0.8, shadowSample);
					shadowAttenuation = lerp(0.7, 1.0, softShadow);
				}

				float cel;
				if (_UsePixelPerfectShadows > 0.5)
				{
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
				waterColor.rgb *= lightingTint * _LightIntensity;			
				
				
				float foamDepth = saturate(depthDifference / _FoamMaxDistance);
				float foamGradient = 1 - foamDepth;
				float foamVisibility = smoothstep(_FoamMinDistance, _FoamMaxDistance * 0.7, foamGradient);
				
				float surfaceNoiseCutoff = foamVisibility * _SurfaceNoiseCutoff * 0.8;
				
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
				
				float4 finalColor = lerp(waterColor, surfaceNoiseColor, surfaceNoiseColor.a * foamVisibility);
				finalColor.a = lerp(0.2, 0.6, saturate(depthDifference)) * _DepthGradientShallow.a;
				return finalColor;
			}			
			ENDHLSL
		}
	}
}
