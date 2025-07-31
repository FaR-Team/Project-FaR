Shader "FaRTeam/FaRFogShader"
{
    Properties
    {
        _FogColor ("Fog Color", Color) = (0.7, 0.8, 0.9, 1)
        [Toggle] _ReverseGradient ("Reverse Gradient", Float) = 0
        _Density ("Max Opacity", Range(0, 1)) = 0.8
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "ForwardLit"
            Tags {"LightMode" = "UniversalForward"}
            
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float3 viewDirWS : TEXCOORD3;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _FogColor;
                float _ReverseGradient;
                float _Density;
            CBUFFER_END

            Varyings vert (Attributes input)
            {
                Varyings output;
                
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.worldPos = TransformObjectToWorld(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.viewDirWS = GetWorldSpaceViewDir(output.worldPos);
                
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                // Use the dot product of view direction and normal to create depth effect
                // Faces pointing away from camera = back = more opaque
                // Faces pointing toward camera = front = more transparent
                float3 viewDir = normalize(input.viewDirWS);
                float3 normal = normalize(input.normalWS);
                
                float viewDot = dot(viewDir, normal);
                
                // Convert to 0-1 range where:
                // viewDot = -1 (facing away) = back = opaque
                // viewDot = 1 (facing toward) = front = transparent
                float gradient = saturate((viewDot + 1.0) * 0.5);
                
                // Reverse if needed
                if (_ReverseGradient > 0.5) {
                    gradient = 1.0 - gradient;
                }
                
                // Final alpha
                float alpha = gradient * _Density;
                
                return half4(_FogColor.rgb, alpha);
            }
            ENDHLSL
        }
    }
    
    Fallback "Hidden/Universal Render Pipeline/FallbackError"
}