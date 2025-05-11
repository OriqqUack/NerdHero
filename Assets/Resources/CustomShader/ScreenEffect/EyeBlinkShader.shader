Shader "Custom/EyeBlinkShader"
{
    Properties
    {
        _Progress ("Open Progress", Range(0,1)) = 0
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" }
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Progress;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; 
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5); // 중심
                float dist = distance(i.uv, center); // 현재 픽셀의 중심까지 거리

                float mask = smoothstep(_Progress, _Progress - 0.05, dist);

                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb *= (1.0 - mask); // 검정색으로 덮어버림

                return col;
            }
            ENDCG
        }
    }
}
