Shader "FaRTeam/FaRMainShaderURP" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1) // Color multiplied to the texture
        _MainTex ("Texture", 2D) = "white" {}  // Texture
        _CelShadingBlurWidth ("Cel Shading Blur", Range(0,2)) = 0.2 // Blur between thresholds
    }
    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" } // Adjust queue and render type for opaque objects
        LOD 200
        
        Cull Off // No culling for double-sided rendering
        
        Pass {
            Name "BASE"
            Tags { "LightMode"="UniversalForward" } // Light mode for URP
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };
            
            fixed4 _Color;
            float _CelShadingBlurWidth;
            sampler2D _MainTex;
            
            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target {
                fixed4 texColor = tex2D(_MainTex, i.uv) * _Color;
                fixed3 albedo = texColor.rgb;
                fixed alpha = texColor.a;
                
                fixed NdotL = dot(i.vertex.xyz, normalize(_WorldSpaceLightPos0)); // Use world space light position
                
                fixed cel;
                
                if (NdotL < 0.5 - _CelShadingBlurWidth / 2) // Outside the blur but dark
                    cel = 1;
                else if (NdotL > 0.5 + _CelShadingBlurWidth / 2) // Outside the blur but light
                    cel = 2;
                else // Inside the blur
                    cel = 3 - ((0.5 + _CelShadingBlurWidth / 2 - NdotL) / _CelShadingBlurWidth);
                
                fixed4 finalColor;
                finalColor.rgb = (cel + 0.3) / 2.5 * albedo; // Use _WorldSpaceLightPos0 to get light color
                finalColor.a = alpha;
                
                return finalColor;
            }
            
            ENDCG
        }
    }
    FallBack "Diffuse"
}
