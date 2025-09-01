Shader "Custom/Flag"
{
    Properties
    {
        _FlagColor ("Flag Color", Color) = (1, 0, 0, 1)
        _PoleColor ("Pole Color", Color) = (0.2, 0.2, 0.2, 1)
        _FlagWidth ("Flag Width", Range(0,1)) = 0.6
        _FlagHeight ("Flag Height", Range(0,1)) = 0.5
        _PoleWidth ("Pole Width", Range(0,0.2)) = 0.08
        _FlagOffset ("Flag Vertical Offset", Range(-0.5,0.5)) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
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
            };

            float4 _FlagColor;
            float4 _PoleColor;
            float _FlagWidth;
            float _FlagHeight;
            float _PoleWidth;
            float _FlagOffset;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Point-in-triangle test
            float insideTriangle(float2 p, float2 p0, float2 p1, float2 p2)
            {
                float2 v0 = p2 - p0;
                float2 v1 = p1 - p0;
                float2 v2 = p - p0;

                float dot00 = dot(v0, v0);
                float dot01 = dot(v0, v1);
                float dot02 = dot(v0, v2);
                float dot11 = dot(v1, v1);
                float dot12 = dot(v1, v2);

                float invDenom = 1.0 / (dot00 * dot11 - dot01 * dot02);
                float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
                float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

                return (u >= 0.0 && v >= 0.0 && (u + v) <= 1.0) ? 1.0 : 0.0;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                fixed4 col = float4(0,0,0,0); // fully transparent background

                // --- Pole (vertical rectangle centered at x=0.5) ---
                float poleLeft  = 0.5 - _PoleWidth * 0.5;
                float poleRight = 0.5 + _PoleWidth * 0.5;
                if (uv.x > poleLeft && uv.x < poleRight && uv.y > 0.0 && uv.y < 1.0)
                {
                    col = _PoleColor;
                }

                // --- Flag triangle (extends left from pole, slides vertically) ---
                float2 baseTop    = float2(0.5, 0.75 + _FlagOffset);                // top of pole + offset
                float2 baseBottom = float2(0.5, 0.75 - _FlagHeight + _FlagOffset);  // bottom of flag
                float2 tip        = float2(0.5 - _FlagWidth, 0.75 - (_FlagHeight * 0.5) + _FlagOffset);

                float inside = insideTriangle(uv, baseBottom, tip, baseTop);

                if (inside > 0.5)
                {
                    col = _FlagColor; // draw red flag (covers pole if overlapping)
                }

                return col;
            }
            ENDCG
        }
    }
}
