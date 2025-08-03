Shader "Custom/EdgeBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurRadius ("Blur Radius", Range(0, 0.1)) = 0.05
        _EdgeStart ("Edge Start", Range(0, 1)) = 0.7
        _BlurIntensity ("Blur Intensity", Range(0, 5)) = 1.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // 添加目标平台指令，避免循环展开问题
            #pragma target 3.0
            #pragma exclude_renderers gles gles3
            #include "UnityCG.cginc"

            // 定义固定的采样次数（编译时常量）
            #define BLUR_SAMPLES 8

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _BlurRadius;
            float _EdgeStart;
            float _BlurIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 计算到屏幕中心的距离
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);
                
                // 计算模糊强度（基于距离）
                float blurFactor = saturate((dist - _EdgeStart) / (1 - _EdgeStart));
                float actualBlur = blurFactor * _BlurRadius * _BlurIntensity;
                
                // 如果没有模糊，直接返回原像素
                if (actualBlur <= 0) {
                    return tex2D(_MainTex, i.uv);
                }
                
                // 进行模糊采样 - 使用固定采样次数
                fixed4 col = tex2D(_MainTex, i.uv);
                float2 dir = center - i.uv;
                float distanceToCenter = length(dir);
                dir = normalize(dir);
                
                // 使用固定次数的循环
                for (int j = 1; j < BLUR_SAMPLES; j++)
                {
                    // 在径向方向上采样
                    float t = j / (float)BLUR_SAMPLES;
                    float offset = actualBlur * (t - 0.5);
                    float2 sampleUV = i.uv + dir * offset;
                    col += tex2D(_MainTex, sampleUV);
                }
                
                // 平均采样结果
                return col / BLUR_SAMPLES;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}