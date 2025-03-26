Shader "FaRTeam/TranslucentFogShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TintColor ("Outer Tint Color", Color) = (0.5, 0.5, 0.5, 0.5)
        _InnerColor ("Inner Color", Color) = (1, 0.5, 0.8, 0.7)
        _CoreDensity ("Core Density", Range(0, 1)) = 0.9
        _EdgeDensity ("Edge Density", Range(0, 0.5)) = 0.1
        _EdgeFade ("Edge Fade", Range(0.1, 10)) = 4.0
        _NoiseTexture ("Noise Texture", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Range(0, 10)) = 1.0
        _NoiseSpeed ("Noise Speed", Vector) = (0.1, 0.1, 0, 0)
        _NoiseInfluence ("Noise Influence", Range(0, 1)) = 0.3
        _DistortionAmount ("Distortion", Range(0, 1)) = 0.1
        _PulseSpeed ("Pulse Speed", Range(0, 5)) = 1
        _PulseAmount ("Pulse Amount", Range(0, 1)) = 0.2
        _DensityMultiplier ("Density Multiplier", Range(1, 5)) = 2.0
        _MinAlpha ("Minimum Alpha", Range(0, 1)) = 0.05
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
            "RenderPipeline" = "UniversalPipeline"
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
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 viewDir : TEXCOORD1;
                float3 normal : NORMAL;
                float3 worldPos : TEXCOORD2;
                float3 localPos : TEXCOORD3;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            sampler2D _NoiseTexture;
            float4 _MainTex_ST;
            float4 _TintColor;
            float4 _InnerColor;
            float _CoreDensity;
            float _EdgeDensity;
            float _EdgeFade;
            float _NoiseScale;
            float4 _NoiseSpeed;
            float _NoiseInfluence;
            float _DistortionAmount;
            float _PulseSpeed;
            float _PulseAmount;
            float _DensityMultiplier;
            float _MinAlpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.localPos = v.vertex.xyz;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate distance from center in local space
                float3 centerOffset = i.localPos;
                float distFromCenter = length(centerOffset) * 2.0; // Normalized to object bounds
                
                // Create a stronger radial gradient from center with more pronounced falloff
                float radialGradient = saturate(pow(distFromCenter, _EdgeFade));
                
                // Animate noise for movement
                float2 noiseUV = i.worldPos.xz * _NoiseScale * 0.1;
                noiseUV += _Time.y * _NoiseSpeed.xy;
                float noise = tex2D(_NoiseTexture, noiseUV).r;
                
                // Second noise layer for more variation
                float2 noiseUV2 = i.worldPos.xy * _NoiseScale * 0.15;
                noiseUV2 += _Time.y * _NoiseSpeed.xy * 0.7;
                float noise2 = tex2D(_NoiseTexture, noiseUV2).r;
                
                // Third noise layer for depth feeling
                float2 noiseUV3 = i.worldPos.yz * _NoiseScale * 0.2;
                noiseUV3 -= _Time.y * _NoiseSpeed.xy * 0.5;
                float noise3 = tex2D(_NoiseTexture, noiseUV3).r;
                
                // Combine noises with different weights for more depth
                noise = noise * 0.5 + noise2 * 0.3 + noise3 * 0.2;
                
                // Pulse effect
                float pulse = (sin(_Time.y * _PulseSpeed) + 1) * 0.5;
                
                // Rim effect for edge highlighting and depth perception
                float rim = 1.0 - saturate(dot(i.viewDir, i.normal));
                rim = pow(rim, 3);
                
                // Distort UVs based on noise and rim
                float2 distortedUV = i.uv + (rim * _DistortionAmount) + (noise * _DistortionAmount * 0.5);
                
                // Apply density multiplier to create more pronounced center
                float densityFactor = 1.0 - radialGradient;
                densityFactor = pow(densityFactor, _DensityMultiplier);
                
                // Calculate alpha based on distance from center with stronger falloff
                float alpha = lerp(_EdgeDensity, _CoreDensity, densityFactor);
                
                // Add noise variation to alpha with controlled influence
                alpha = alpha * (1.0 - (noise * _NoiseInfluence));
                
                // Add pulse effect
                alpha = alpha * (1.0 + (pulse * _PulseAmount));
                
                // Ensure minimum alpha
                alpha = max(alpha, _MinAlpha);
                
                // Sample texture with distorted UVs
                fixed4 col = tex2D(_MainTex, distortedUV);
                
                // Create a more pronounced color gradient from center to edge
                fixed4 finalColor = lerp(_InnerColor, _TintColor, pow(radialGradient, 0.5));
                col *= finalColor * i.color; // Apply vertex color
                
                // Add rim lighting for depth perception
                col.rgb += rim * _TintColor.rgb * 0.5;
                
                // Add noise-based color variation that's stronger in the center
                col.rgb += noise * 0.15 * _InnerColor.rgb * pulse * (1.0 - radialGradient);
                
                // Apply final alpha
                col.a *= alpha;
                
                // Ensure we don't discard the fragment completely
                if (col.a < 0.01)
                    col.a = 0.01;
                
                return col;
            }
            ENDCG
        }
    }
    
    Fallback "Transparent/VertexLit"
}