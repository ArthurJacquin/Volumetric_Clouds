Shader "Unlit/VolumeShader"
{
    Properties
    {
        _MainTex("Texture", 3D) = "white" {}
        _Alpha("Alpha", float) = 0.02
        _StepSize("Step Size", float) = 0.01
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend One OneMinusSrcAlpha
        LOD 100
        ZTest Always
        ZWrite On
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // Maximum amount of raymarching samples
            #define MAX_STEP_COUNT 1024

            // Allowed floating point inaccuracy
            #define EPSILON 0.00001f

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 vertexLocal : TEXCOORD1;
            };

            sampler3D _MainTex;
            float4 _MainTex_ST;
            float _Alpha;
            float _StepSize;

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.vertexLocal = v.vertex;

                return o;
            }

            float4 BlendUnder(float4 color, float4 newColor)
            {
                color.rgb += (1.0 - color.a) * newColor.a * newColor.rgb;
                color.a += (1.0 - color.a) * newColor.a;
                return color;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Start raymarching at the front surface of the object
                float3 rayOrigin = i.vertexLocal;

                // Use vector from camera to object surface to get ray direction
                float3 rayDirection = ObjSpaceViewDir(float4(i.vertexLocal, 0.0f));
                rayDirection = normalize(rayDirection);

                float4 color = float4(0, 0, 0, 0);
                float3 samplePosition = rayOrigin;
                // Raymarch through object space
                for (int i = 0; i < MAX_STEP_COUNT; i++)
                {
                    // Accumulate color only within unit cube bounds
                    if (max(abs(samplePosition.x), max(abs(samplePosition.y), abs(samplePosition.z))) < 0.5f + EPSILON)
                    {
                        float4 sampledColor = tex3Dlod(_MainTex, float4(samplePosition + float3(0.5f, 0.5f, 0.5f), 0.0f));
                        sampledColor.a *= _Alpha;
                        color = BlendUnder(color, sampledColor);
                        samplePosition += rayDirection * _StepSize;
                    }
                }

                return color;
            }
            ENDCG
        }
    }
}