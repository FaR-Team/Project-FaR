Shader "Custom/TelekineticRay"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (0.5,0.5,1,1)
        _InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
        _EmissionStrength ("Emission Strength", Range(0, 5)) = 2.0
        _ScrollSpeed ("Scroll Speed", Range(-5, 5)) = 1.0
        _NoiseScale ("Noise Scale", Range(0, 10)) = 1.0
        _PulseFreq ("Pulse Frequency", Range(0, 10)) = 2.0
    }

    Category
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB
        Cull Off Lighting Off ZWrite Off

        SubShader
        {
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0
                #pragma multi_compile_particles
                #pragma multi_compile_fog

                #include "UnityCG.cginc"

                sampler2D _MainTex;
                fixed4 _TintColor;
                float _InvFade;
                float _EmissionStrength;
                float _ScrollSpeed;
                float _NoiseScale;
                float _PulseFreq;

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                    UNITY_FOG_COORDS(1)
                    #ifdef SOFTPARTICLES_ON
                    float4 projPos : TEXCOORD2;
                    #endif
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                float4 _MainTex_ST;

                v2f vert (appdata_t v)
                {
                    v2f o;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    #ifdef SOFTPARTICLES_ON
                    o.projPos = ComputeScreenPos (o.vertex);
                    COMPUTE_EYEDEPTH(o.projPos.z);
                    #endif
                    o.color = v.color;
                    o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }

                UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
                float _DepthFade;

                fixed4 frag (v2f i) : SV_Target
                {
                    #ifdef SOFTPARTICLES_ON
                    float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
                    float partZ = i.projPos.z;
                    float fade = saturate (_InvFade * (sceneZ-partZ));
                    i.color.a *= fade;
                    #endif

                    // Animación de desplazamiento
                    float2 scrolledUV = i.texcoord;
                    scrolledUV.x += _Time.y * _ScrollSpeed;

                    // Efecto de ruido/electricidad
                    float noise = sin(scrolledUV.x * _NoiseScale + _Time.y * 10.0) * 0.1;
                    scrolledUV.y += noise;

                    // Pulsación
                    float pulse = sin(_Time.y * _PulseFreq) * 0.5 + 0.5;
                    
                    // Sampling de textura
                    fixed4 col = 2.0f * i.color * _TintColor * tex2D(_MainTex, scrolledUV);
                    
                    // Aplicar emisión y pulsación
                    col.rgb *= _EmissionStrength * (0.8 + pulse * 0.2);
                    
                    // Efecto de fade en los bordes
                    float edgeFade = abs(i.texcoord.y - 0.5) * 2.0;
                    col.a *= (1.0 - edgeFade * edgeFade);

                    UNITY_APPLY_FOG(i.fogCoord, col);
                    return col;
                }
                ENDCG
            }
        }
    }
}
