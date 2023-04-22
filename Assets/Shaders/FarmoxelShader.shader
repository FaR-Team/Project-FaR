Shader "FaRTeam/FarmoxelShader"
{
    Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Textura", 2D) = "white" {}
		// La luz ambiente se aplica de forma uniforme a todas las superficies del objeto.
		[HDR]
		_AmbientColor("Color de Ambiente", Color) = (0.4,0.4,0.4,1)
		[HDR]
		_SpecularColor("Color de Reflejo (Specular)", Color) = (0.9,0.9,0.9,1)
		// Controla el tamaño del reflejo specular.
		_Glossiness("Brillo", Float) = 32
		//[HDR]
		//_RimColor("Color de Rim", Color) = (1,1,1,1)
		//_RimAmount("Cantidad de Rim", Range(0, 1)) = 1
		// Controla cuán suavemente se mezlca el rim cuando nos acercamos a partes de la superficie sin luz.
		//_RimThreshold("Límite de Rim", Range(0, 1)) = 0		
	}
	SubShader
	{
		Pass
		{
			// Configura el "Pass" para usar Forward rendering, y sólo recibir
			// datos en la dirección principal de la luz direccional y ambiental.
			Tags
			{
				"LightMode" = "ForwardBase"
				"PassFlags" = "OnlyDirectional"
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#pragma multi_compile_fwdbase
			
			#include "UnityCG.cginc"
			
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct appdata
			{
				float4 vertex : POSITION;				
				float4 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 worldNormal : NORMAL;
				float2 uv : TEXCOORD0;
				float3 viewDir : TEXCOORD1;	
				
				SHADOW_COORDS(2)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);		
				o.viewDir = WorldSpaceViewDir(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				
				TRANSFER_SHADOW(o)
				return o;
			}
			
			float4 _Color;

			float4 _AmbientColor;

			float4 _SpecularColor;
			float _Glossiness;		

			//float4 _RimColor;
			//float _RimAmount;
			//float _RimThreshold;	

			float4 frag (v2f i) : SV_Target
			{
				float3 normal = normalize(i.worldNormal);
				float3 viewDir = normalize(i.viewDir);

				// La luz abajo está calculada usando linn-Phong,
				// con valores límites para crear ese look "toon".
				// https://en.wikipedia.org/wiki/Blinn-Phong_shading_model

				// Calcula la iluminación de la luz direccional.
				// _WorldSpaceLightPos0 es un vector apuntando la dirección
				// opuesta a la luz direccional principal.
				float NdotL = dot(_WorldSpaceLightPos0, normal);

				float shadow = SHADOW_ATTENUATION(i);

				float lightIntensity = smoothstep(0, 0.01, NdotL * shadow);	

				float4 light = lightIntensity * _LightColor0;

				// Calcula la reflexión specular.
				float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
				float NdotH = dot(normal, halfVector);
				// Multiplica _Glossiness por sí mismo para usar menos brillo en el inspector.
				float specularIntensity = pow(NdotH * lightIntensity, _Glossiness * _Glossiness);
				float specularIntensitySmooth = smoothstep(0.005, 0.01, specularIntensity);
				float4 specular = specularIntensitySmooth * _SpecularColor;				

				// Calcula la luz rim.
				//float rimDot = 1 - dot(viewDir, normal);
				// Sólo tiene que aparecer en el lado iluminado de la superficie,
				// así que lo multiplico por NdotL, y lo elevo a un poder para hacer una transición suave.
				//float rimIntensity = rimDot * pow(NdotL, _RimThreshold);
				//rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);
				//float4 rim = rimIntensity * _RimColor;

				float4 sample = tex2D(_MainTex, i.uv);

				return (light + _AmbientColor + specular) * _Color * sample;
			}
			ENDCG
		}

		// Soporte para sombras
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
}
