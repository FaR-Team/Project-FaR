Shader "FaRTeam/FaRMainShader" {
    Properties {
        _Color("Color", Color) = (1, 1, 1, 1) // Color multiplied to the texture
        _MainTex("Texture", 2D) = "white" {}  // Texture
        _CelShadingBlurWidth("Cel Shading Blur", Range(0, 2)) = 0.2 // Blur between thresholds
    }
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200
		cull off
		pass{
		Name "Cutout"
            AlphaTest Greater 0.9
			SetTexture[_MainTex]
		}

        CGPROGRAM

        #pragma surface surf Toon fullforwardshadows Lambert alpha

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
            o.Alpha = c.a; // Set alpha channel based on texture alpha
        }

        fixed4 LightingToon(SurfaceOutput s, fixed3 lightDir, fixed atten) {
            half NdotL = dot(s.Normal, lightDir); // Value between 0 and 1

            half cel;

            // 0 | threshold 1 | blur | threshold 2 | 1
            // 0 |**************|<- .5 ->|xxxxxxxxxxxxx| 1

            if (NdotL < 0.5 - _CelShadingBlurWidth / 2) // Outside the blur but dark
                cel = 1;
            else if (NdotL > 0.5 + _CelShadingBlurWidth / 2) // Outside the blur but light
                cel = 2;
            else // Inside the blur
                cel = 3 - ((0.5 + _CelShadingBlurWidth / 2 - NdotL) / _CelShadingBlurWidth);

            half4 c;
            c.rgb = (cel + 0.3) / 2.5 * s.Albedo * _LightColor0.rgb * atten; // Not too bright
            c.a = s.Alpha; // Set alpha channel based on lighting calculations

            return c;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
