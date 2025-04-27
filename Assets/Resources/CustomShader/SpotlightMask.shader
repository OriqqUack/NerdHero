Shader "UI/SpotlightFixedCircle"
{
    Properties
    {
        _Color ("Overlay Color", Color) = (0, 0, 0, 0.7)

        _RectCenter ("Rect Center", Vector) = (0.5, 0.5, 0, 0)
        _RectSize ("Rect Size", Vector) = (0.3, 0.1, 0, 0)

        _CircleCenter ("Circle Center", Vector) = (960, 540, 0, 0) // screen space 중심
        _CircleRadius ("Circle Radius", Float) = 100
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 screenPos : TEXCOORD1;
            };

            float4 _Color;
            float4 _RectCenter;
            float4 _RectSize;

            float4 _CircleCenter;
            float _CircleRadius;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;

                // 화면 픽셀 위치 계산
                o.screenPos = ComputeScreenPos(o.vertex).xy / ComputeScreenPos(o.vertex).w;
                o.screenPos *= _ScreenParams.xy;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 🔷 사각형 spotlight (UV 기반)
                float2 rectDiff = abs(i.uv - _RectCenter.xy);
                bool inRect = rectDiff.x < _RectSize.x * 0.5 && rectDiff.y < _RectSize.y * 0.5;

                // 🔵 원형 spotlight (픽셀 기준)
                float2 diff = i.screenPos - _CircleCenter.xy;
                float dist = length(diff);
                bool inCircle = dist < _CircleRadius;

                if (inRect || inCircle)
                    discard;

                return _Color;
            }
            ENDCG
        }
    }
}
