Shader "FaRTeam/FogShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FogColor ("Fog Color", Color) = (0.5, 0.5, 0.5, 1)
        _FogDensity ("Fog Density", Range(0, 1)) = 0.5
        _FogStart ("Fog Start Distance", Float) = 0
        _FogEnd ("Fog End Distance", Float) = 100
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _FogColor;
            float _FogDensity;
            float _FogStart;
            float _FogEnd;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dist = length(_WorldSpaceCameraPos - i.worldPos);
                float fogFactor = saturate((dist - _FogStart) / (_FogEnd - _FogStart));
                fogFactor = saturate(fogFactor * _FogDensity);
                
                fixed4 col = tex2D(_MainTex, i.uv);
                return lerp(col, _FogColor, fogFactor);
            }
            ENDCG
        }
    }
}
