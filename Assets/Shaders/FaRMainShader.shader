Shader "FaRTeam/FaRMainShaderURP" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _CelShadingBlurWidth ("Cel Shading Blur", Range(0,2)) = 0.2
        _Alpha ("Alpha", Range(0,1)) = 1
    }
    SubShader {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200
        
        Cull Off
        
        ZWrite On
        
        Pass {
            Name "BASE"
            Tags { "LightMode"="UniversalForward" }
            
            Blend SrcAlpha OneMinusSrcAlpha
            
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
            float _Alpha;
            
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
                fixed alpha = texColor.a * _Alpha;
                
                fixed NdotL = dot(i.vertex.xyz, normalize(_WorldSpaceLightPos0));
                
                fixed cel;
                
                if (NdotL < 0.5 - _CelShadingBlurWidth / 2)
                    cel = 1;
                else if (NdotL > 0.5 + _CelShadingBlurWidth / 2)
                    cel = 2;
                else
                    cel = 3 - ((0.5 + _CelShadingBlurWidth / 2 - NdotL) / _CelShadingBlurWidth);
                
                fixed4 finalColor;
                finalColor.rgb = (cel + 0.3) / 2.5 * albedo;
                finalColor.a = alpha;
                
                if (finalColor.a < 0.01)
                    discard;
                
                return finalColor;
            }
            
            ENDCG
        }
    }
    FallBack "Diffuse"
}
