Shader "Custom/Mine"
{
    Properties
    {
        _MineColor ("Mine Color", Color) = (0.05, 0.05, 0.05, 1)
        _HighlightColor ("Highlight Color", Color) = (0.8, 0.8, 0.8, 1)
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineThickness ("Outline Thickness", Range(0,0.1)) = 0.02
        _Radius ("Mine Radius", Range(0,0.5)) = 0.35
        _Feather ("Edge Feather", Range(0,0.2)) = 0.02
        _ShineIntensity ("Shine Intensity", Range(0,1)) = 0.4
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

            float4 _MineColor;
            float4 _HighlightColor;
            float4 _OutlineColor;
            float _OutlineThickness;
            float _Radius;
            float _Feather;
            float _ShineIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv * 2.0 - 1.0; // center at (0,0)
                float dist = length(uv);

                // distance from edge
                float inner = _Radius - _Feather;
                float outer = _Radius;

                // 0 inside circle, 1 outside
                float mask = smoothstep(inner, outer, dist);

                // If completely outside the circle + outline, discard
                if (dist > _Radius + _OutlineThickness)
                    discard;

                // Base mine color
                fixed4 col = _MineColor;

                // Add highlight (a small lighter spot in the top-left)
                float2 lightPos = float2(-0.3, 0.3);
                float lightDist = distance(uv, lightPos);
                float highlight = smoothstep(0.3, 0.0, lightDist);
                col = lerp(col, _HighlightColor, highlight * _ShineIntensity);

                // Apply circle fade at edges
                col.a = 1.0 - mask;

                // Outline ring around circle
                float outlineInner = _Radius;
                float outlineOuter = _Radius + _OutlineThickness;
                float outline = smoothstep(outlineOuter, outlineInner, dist);
                col = lerp(_OutlineColor, col, outline);

                return col;
            }
            ENDCG
        }
    }
}
