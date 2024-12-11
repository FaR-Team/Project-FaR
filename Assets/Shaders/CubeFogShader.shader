Shader "FaRTeam/CubeFogShader"
{
    Properties
    {
        _InnerCubeSize ("Inner Cube Size", Range(0, 0.9)) = 0.5
        _FogDensity ("Fog Density", Range(0, 5)) = 2
        _InnerColor ("Inner Color", Color) = (1, 1, 1, 1)
        _OuterColor ("Outer Color", Color) = (0.5, 0.5, 0.5, 0)
        _PulseSpeed ("Pulse Speed", Range(0, 2)) = 0.5
        _EdgeSharpness ("Edge Sharpness", Range(1, 50)) = 20
        _InnerCubeOpacity ("Inner Cube Opacity", Range(0, 1)) = 1
        _InnerCubeHardness ("Inner Cube Hardness", Range(0, 100)) = 50
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent+1"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 localPos : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };

            float _InnerCubeSize;
            float _FogDensity;
            float4 _InnerColor;
            float4 _OuterColor;
            float _PulseSpeed;
            float _EdgeSharpness;
            float _InnerCubeOpacity;
            float _InnerCubeHardness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.localPos = v.vertex.xyz;
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 absPos = abs(i.localPos);
                float maxCoord = max(max(absPos.x, absPos.y), absPos.z);
                
                // Super sharp inner cube with hard cutoff
                float innerCube = pow(step(maxCoord, _InnerCubeSize), _InnerCubeHardness);
                
                // Fog transition
                float distFromCenter = length(i.localPos);
                float fogFactor = pow(saturate(1 - distFromCenter), _FogDensity);
                float pulse = sin(_Time.y * _PulseSpeed) * 0.15 + 0.85;
                
                // Combine with completely solid inner cube
                float4 finalColor = _OuterColor;
                finalColor = lerp(finalColor, _InnerColor, fogFactor);
                
                // Force inner cube to be completely solid
                if (maxCoord < _InnerCubeSize) {
                    finalColor = _InnerColor;
                    finalColor.a = _InnerCubeOpacity;
                    return finalColor;
                }
                
                finalColor.a *= fogFactor * pulse;
                return finalColor;
            }
            ENDCG
        }
    }
}