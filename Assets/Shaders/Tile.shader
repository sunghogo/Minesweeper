Shader "Custom/Tile"
{
    Properties
    {
        _HiddenColor ("Hidden Color", Color) = (0.6, 0.6, 0.6, 1)
        _RevealedColor ("Revealed Color", Color) = (0.9, 0.9, 0.9, 1)
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineThickness ("Outline Thickness", Range(0,0.1)) = 0.02
        _BevelLightColor ("Bevel Dark (Bottom Left)", Color) = (0.3, 0.3, 0.3, 1)
        _BevelDarkColor ("Bevel Light (Top Right)", Color) = (1, 1, 1, 1)
        _State ("Tile State (0=Hidden,1=Revealed)", Range(0,1)) = 0
        _Bevel ("Bevel Amount (only when hidden)", Range(0,0.5)) = 0.1
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

            float4 _HiddenColor;
            float4 _RevealedColor;
            float4 _OutlineColor;
            float4 _BevelLightColor;
            float4 _BevelDarkColor;
            float _State;
            float _Bevel;
            float _OutlineThickness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // --- Outline ---
                float edge = min(min(uv.x, uv.y), min(1 - uv.x, 1 - uv.y));
                if (edge < _OutlineThickness)
                {
                    return _OutlineColor;
                }

                // --- Base tile color ---
                fixed4 col = (_State < 0.5) ? _HiddenColor : _RevealedColor;

                // --- Bevel (only when hidden) ---
                if (_State < 0.5)
                {
                    // Distance to edges
                    float left   = uv.x;
                    float right  = 1.0 - uv.x;
                    float top    = uv.y;
                    float bottom = 1.0 - uv.y;

                    // Top-left bevel
                    if (min(left, top) < _Bevel)
                    {
                        float factor = min(left, top) / _Bevel;
                        col = lerp(_BevelLightColor, col, factor);
                    }
                    // Bottom-right bevel
                    else if (min(right, bottom) < _Bevel)
                    {
                        float factor = min(right, bottom) / _Bevel;
                        col = lerp(_BevelDarkColor, col, factor);
                    }
                }

                return col;
            }
            ENDCG
        }
    }
}
