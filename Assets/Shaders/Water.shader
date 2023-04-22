Shader "FaRTeam/Water"
{
    Properties
    {
		// De qué color se va a ver el agua cuando haya un agujero poco profundo abajo.
		_DepthGradientShallow("Degradé de Poca Profundidad", Color) = (0.325, 0.807, 0.971, 0.725)

		// De qué color se va a ver el agua cuando la superficie debajo esté lo más profundo posible.
		_DepthGradientDeep("Degradé de Profundidad Profunda", Color) = (0.086, 0.407, 1, 0.749)

		// Distancia máxima para que la superficie bajo el agua afecte al degradé.
		_DepthMaxDistance("Distancia Máxima de Profundidad", Float) = 1

		// Color para renderizar la espuma generada por objetos que intersecten.
		_FoamColor("Color de la Espuma", Color) = (1,1,1,1)

		// Textura de sonido para generar olas.
		_SurfaceNoise("Sonido en la Superficie", 2D) = "white" {}

		// Velocidad en UVs por segundo para que se muevan las olas. Sólo usa xy.
		_SurfaceNoiseScroll("Cantidad de Desplazamiento de Sonido en Superficie", Vector) = (0.03, 0.03, 0, 0)

		// Vlores en el sonido mayores al corte se renderizan en la superficie.
		_SurfaceNoiseCutoff("Corte Espuma Superficie", Range(0, 1)) = 0.7

		// Los canales rojo y verde de esta textura se usan para compensar
		// textura de sonido y crear distorsión en las olas.
		_SurfaceDistortion("Distorsión de la Superficie", 2D) = "white" {}	

		// Multiplica la distorsión por este valor.
		_SurfaceDistortionAmount("Cantidad de Distorsión", Range(0, 1)) = 0.8

		// Controla la distancia que las superficies bajo el agua van a renderizar espuma.
		_FoamMaxDistance("Distancia Máxima Espuma", Float) = 3
		_FoamMinDistance("Distancia Mínima Espuma", Float) = 0.04

		//_WaveScale("Tamaño de las Olas", Float) = 3
		//WaveSpeed("Velocidad de las Olas", Color) = (1,1,1,1)
    }
    SubShader
    {
		Tags
		{
			"Queue" = "Transparent"
		}

        Pass
        {
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off

            CGPROGRAM
			#define SMOOTHSTEP_AA 0.01

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"


			float4 alphaBlend(float4 top, float4 bottom)
			{
				float3 color = (top.rgb * top.a) + (bottom.rgb * (1 - top.a));
				float alpha = top.a + bottom.a * (1 - top.a);

				return float4(color, alpha);
			}

            struct appdata
            {
                float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
				float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;	
				float2 noiseUV : TEXCOORD0;
				float2 distortUV : TEXCOORD1;
				float4 screenPosition : TEXCOORD2;
				float3 viewNormal : NORMAL;
            };

			sampler2D _SurfaceNoise;
			float4 _SurfaceNoise_ST;

			sampler2D _SurfaceDistortion;
			float4 _SurfaceDistortion_ST;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPosition = ComputeScreenPos(o.vertex);
				o.distortUV = TRANSFORM_TEX(v.uv, _SurfaceDistortion);
				o.noiseUV = TRANSFORM_TEX(v.uv, _SurfaceNoise);
				o.viewNormal = COMPUTE_VIEW_NORMAL;

                return o;
            }

			float4 _DepthGradientShallow;
			float4 _DepthGradientDeep;
			float4 _FoamColor;

			float _DepthMaxDistance;
			float _FoamMaxDistance;
			float _FoamMinDistance;
			float _SurfaceNoiseCutoff;
			float _SurfaceDistortionAmount;

			float2 _SurfaceNoiseScroll;

			sampler2D _CameraDepthTexture;
			sampler2D _CameraNormalsTexture;

            float4 frag (v2f i) : SV_Target
            {
				
				float existingDepth01 = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r;
				
				float existingDepthLinear = LinearEyeDepth(existingDepth01);

				
				float depthDifference = existingDepthLinear - i.screenPosition.w;

				float waterDepthDifference01 = saturate(depthDifference / _DepthMaxDistance);
				float4 waterColor = lerp(_DepthGradientShallow, _DepthGradientDeep, waterDepthDifference01);
				

				float3 existingNormal = tex2Dproj(_CameraNormalsTexture, UNITY_PROJ_COORD(i.screenPosition));
				
				float3 normalDot = saturate(dot(existingNormal, i.viewNormal));
				float foamDistance = lerp(_FoamMaxDistance, _FoamMinDistance, normalDot);
				float foamDepthDifference01 = saturate(depthDifference / foamDistance);

				float surfaceNoiseCutoff = foamDepthDifference01 * _SurfaceNoiseCutoff;

				float2 distortSample = (tex2D(_SurfaceDistortion, i.distortUV).xy * 2 - 1) * _SurfaceDistortionAmount;

				float2 noiseUV = float2((i.noiseUV.x + _Time.y * _SurfaceNoiseScroll.x) + distortSample.x, 
				(i.noiseUV.y + _Time.y * _SurfaceNoiseScroll.y) + distortSample.y);
				float surfaceNoiseSample = tex2D(_SurfaceNoise, noiseUV).r;

				float surfaceNoise = smoothstep(surfaceNoiseCutoff - SMOOTHSTEP_AA, surfaceNoiseCutoff + SMOOTHSTEP_AA, surfaceNoiseSample);

				float4 surfaceNoiseColor = _FoamColor;
				surfaceNoiseColor.a *= surfaceNoise;

				
				return alphaBlend(surfaceNoiseColor, waterColor);
            }
            ENDCG
        }
    }
}
