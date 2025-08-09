Shader "Hidden/EdgeBlurSeparable"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Range(0, 20)) = 6.0
        _EdgeStart ("Edge Start", Range(0,1)) = 0.3
        _EdgeEnd ("Edge End", Range(0,1)) = 0.8
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }
        LOD 100

        Pass
        {
            Name "H"
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize; 
            float _BlurSize;
            float _EdgeStart;
            float _EdgeEnd;

            // 9-tap gaussian weights (symmetric)
            static const float w0 = 0.227027f;
            static const float w1 = 0.1945946f;
            static const float w2 = 0.1216216f;
            static const float w3 = 0.054054f;
            static const float w4 = 0.016216f;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center) / 0.70710678;
                dist = saturate(dist);

                float bf = smoothstep(_EdgeStart, _EdgeEnd, dist) * _BlurSize;

                if (bf < 0.001)
                {
                    return tex2D(_MainTex, i.uv);
                }

                float2 dx = float2(_MainTex_TexelSize.x, 0) * bf;

                fixed4 col = tex2D(_MainTex, i.uv) * w0;
                col += tex2D(_MainTex, i.uv + dx * 1.0) * w1;
                col += tex2D(_MainTex, i.uv - dx * 1.0) * w1;
                col += tex2D(_MainTex, i.uv + dx * 2.0) * w2;
                col += tex2D(_MainTex, i.uv - dx * 2.0) * w2;
                col += tex2D(_MainTex, i.uv + dx * 3.0) * w3;
                col += tex2D(_MainTex, i.uv - dx * 3.0) * w3;
                col += tex2D(_MainTex, i.uv + dx * 4.0) * w4;
                col += tex2D(_MainTex, i.uv - dx * 4.0) * w4;

                return col;
            }
            ENDCG
        }

        Pass
        {
            Name "V"
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragv
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurSize;
            float _EdgeStart;
            float _EdgeEnd;

            static const float w0 = 0.227027f;
            static const float w1 = 0.1945946f;
            static const float w2 = 0.1216216f;
            static const float w3 = 0.054054f;
            static const float w4 = 0.016216f;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 fragv (v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center) / 0.70710678;
                dist = saturate(dist);

                float bf = smoothstep(_EdgeStart, _EdgeEnd, dist) * _BlurSize;

                if (bf < 0.001)
                {
                    return tex2D(_MainTex, i.uv);
                }

                float2 dy = float2(0, _MainTex_TexelSize.y) * bf;

                fixed4 col = tex2D(_MainTex, i.uv) * w0;
                col += tex2D(_MainTex, i.uv + dy * 1.0) * w1;
                col += tex2D(_MainTex, i.uv - dy * 1.0) * w1;
                col += tex2D(_MainTex, i.uv + dy * 2.0) * w2;
                col += tex2D(_MainTex, i.uv - dy * 2.0) * w2;
                col += tex2D(_MainTex, i.uv + dy * 3.0) * w3;
                col += tex2D(_MainTex, i.uv - dy * 3.0) * w3;
                col += tex2D(_MainTex, i.uv + dy * 4.0) * w4;
                col += tex2D(_MainTex, i.uv - dy * 4.0) * w4;

                return col;
            }
            ENDCG
        }
    }
    FallBack Off
}
