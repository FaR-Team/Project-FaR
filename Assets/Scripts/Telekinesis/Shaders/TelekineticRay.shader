Shader "Custom/TelekineticRay"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (0.9,0.3,1,1)
        _SecondaryColor ("Secondary Magic Color", Color) = (1,0.4,0.8,1)
        _InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
        _EmissionStrength ("Emission Strength", Range(0, 8)) = 3.0
        _ScrollSpeed ("Scroll Speed", Range(-5, 5)) = 1.5
        _NoiseScale ("Noise Scale", Range(0, 15)) = 2.0
        _PulseFreq ("Pulse Frequency", Range(0, 15)) = 4.0
        _MagicShimmer ("Magic Shimmer", Range(0, 2)) = 0.8
        _SparkleIntensity ("Sparkle Intensity", Range(0, 3)) = 1.2
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
                fixed4 _SecondaryColor;
                float _InvFade;
                float _EmissionStrength;
                float _ScrollSpeed;
                float _NoiseScale;
                float _PulseFreq;
                float _MagicShimmer;
                float _SparkleIntensity;

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

                    float2 scrolledUV = i.texcoord;
                    scrolledUV.x += _Time.y * _ScrollSpeed;
                    
                    float2 scrolledUV2 = i.texcoord;
                    scrolledUV2.x += _Time.y * _ScrollSpeed * 0.7;

                    float noise1 = sin(scrolledUV.x * _NoiseScale + _Time.y * 12.0) * 0.08;
                    float noise2 = sin(scrolledUV.x * _NoiseScale * 1.7 + _Time.y * 8.0) * 0.05;
                    float magicWave = sin(scrolledUV.x * _NoiseScale * 0.5 + _Time.y * 3.0) * 0.03;
                    
                    scrolledUV.y += noise1 + magicWave;
                    scrolledUV2.y += noise2 - magicWave * 0.5;

                    float pulse1 = sin(_Time.y * _PulseFreq) * 0.5 + 0.5;
                    float pulse2 = sin(_Time.y * _PulseFreq * 1.3 + 1.57) * 0.5 + 0.5;
                    float shimmer = sin(_Time.y * _PulseFreq * 2.1) * 0.5 + 0.5;
                    
                    fixed4 col1 = tex2D(_MainTex, scrolledUV);
                    fixed4 col2 = tex2D(_MainTex, scrolledUV2);
                    
                    fixed4 magicColor = lerp(_TintColor, _SecondaryColor, pulse2);
                    fixed4 baseCol = 2.0f * i.color * magicColor * col1;
                    fixed4 shimmerCol = 1.5f * i.color * _SecondaryColor * col2;
                    
                    fixed4 col = lerp(baseCol, shimmerCol, shimmer * _MagicShimmer * 0.5);
                    
                    float magicPulse = pulse1 * 0.4 + pulse2 * 0.3 + shimmer * 0.3;
                    col.rgb *= _EmissionStrength * (0.7 + magicPulse);
                    
                    float sparkle = sin(scrolledUV.x * _NoiseScale * 4.0 + _Time.y * 15.0) * 
                                   cos(scrolledUV.y * _NoiseScale * 6.0 + _Time.y * 12.0);
                    sparkle = saturate(sparkle * sparkle * sparkle);
                    
                    col.rgb += sparkle * _SparkleIntensity * _SecondaryColor.rgb * pulse1;
                    
                    float edgeFade = abs(i.texcoord.y - 0.5) * 2.0;
                    edgeFade = smoothstep(0.7, 1.0, edgeFade);
                    col.a *= (1.0 - edgeFade);
                    
                    float centerGlow = 1.0 - edgeFade;
                    col.rgb += centerGlow * centerGlow * 0.2 * _TintColor.rgb * pulse1;

                    UNITY_APPLY_FOG(i.fogCoord, col);
                    return col;
                }
                ENDCG
            }
        }
    }
}
