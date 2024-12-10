Shader "FaRTeam/TranslucentShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TintColor ("Outer Tint Color", Color) = (0.5, 0.5, 0.5, 0.5)
        _InnerColor ("Inner Color", Color) = (1, 0.5, 0.8, 0.7)
        _TopTransparency ("Top Transparency", Range(0, 1)) = 0.1
        _BottomTransparency ("Bottom Transparency", Range(0, 1)) = 0.9
        _GradientHeight ("Gradient Height", Range(0, 2)) = 1
        _DistortionAmount ("Distortion", Range(0, 1)) = 0.1
        _PulseSpeed ("Pulse Speed", Range(0, 5)) = 1
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent+1"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
        }

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 viewDir : TEXCOORD1;
                float3 normal : NORMAL;
                float3 worldPos : TEXCOORD2;
                float3 localPos : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _TintColor;
            float4 _InnerColor;
            float _TopTransparency;
            float _BottomTransparency;
            float _GradientHeight;
            float _DistortionAmount;
            float _PulseSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.localPos = v.vertex.xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float pulse = (sin(_Time.y * _PulseSpeed) + 1) * 0.5;
                float rim = 1.0 - saturate(dot(i.viewDir, i.normal));
                rim = pow(rim, 3);
                
                float2 distortedUV = i.uv + (rim * _DistortionAmount);
                
                // Calculate vertical gradient based on local position
                float heightGradient = saturate((i.localPos.y + 0.5) / _GradientHeight);
                float alpha = lerp(_BottomTransparency, _TopTransparency, heightGradient);
                
                fixed4 col = tex2D(_MainTex, distortedUV);
                col *= _TintColor;
                col.a *= alpha * (1 + (pulse * 0.2));
                col.rgb += rim * _TintColor.rgb * 0.5;
                
                // Add some variation based on height
                col.rgb += (1 - heightGradient) * _InnerColor.rgb * 0.2 * pulse;
                
                return col;
            }
            ENDCG
        }
    }
}