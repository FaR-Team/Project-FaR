Shader "FaRTeam/FaRMainShader" {
	Properties{
		_Color("Color", Color) = (1,1,1,1) //Color multiplicado a la textura
		_MainTex("Textura", 2D) = "white" {} //Textura
		_CelShadingBlurWidth("Difuminado del Cell Shading", Range(0,2)) = 0.2 //Difuminado entre umbrales
	}
		SubShader{
		Tags{ "RenderType" = "Cutout"}
		LOD 200

		CGPROGRAM

#pragma surface surf Toon fullforwardshadows 

#pragma target 3.0

	sampler2D _MainTex;
	sampler2D _RampTex;

	struct Input {
		float2 uv_MainTex;
	};

	half _CelShadingBlurWidth;
	fixed4 _Color;

	void surf(Input IN, inout SurfaceOutput o) {

		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}

	fixed4 LightingToon(SurfaceOutput s, fixed3 lightDir, fixed atten)
	{
		half NdotL = dot(s.Normal, lightDir);  //Valor entre 0 y 1

        half cel;

        // 0 | threshold 1  |  blur  | threshold 2 | 1
        // 0 |**************|<- .5 ->|xxxxxxxxxxxxx| 1

		if (NdotL < 0.5 - _CelShadingBlurWidth / 2)                                         // Fuera del difuminado pero oscuro
            cel = 1;
		else if (NdotL > 0.5 + _CelShadingBlurWidth / 2)                                    // Fuera del difuminado pero claro
            cel = 2;
		else                                                                                // Dentro del difuminado 
            cel = 3- ((0.5 + _CelShadingBlurWidth / 2 - NdotL) / _CelShadingBlurWidth);
		half4 c;

		c.rgb = (cel + 0.3)/2.5  * s.Albedo * _LightColor0.rgb * atten; // Así no se ve tan iluminado
		c.a = s.Alpha;

		return c;
	}

	ENDCG
	}
		FallBack "Diffuse"
}