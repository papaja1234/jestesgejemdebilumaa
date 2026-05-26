Shader "INShaders/Unlit_ColorTransparent_GrayOverlay"
{
    Properties
    {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _Overlay ("Overlay Blend", Range(0,1)) = 0.8
    }

    SubShader
    {
        Tags
        {
            "IGNOREPROJECTOR"="true"
            "QUEUE"="Transparent"
            "RenderType"="Transparent"
        }

        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            ZClip On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _Color;
            float _Overlay;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);

                fixed lum = dot(tex.rgb, fixed3(0.2989, 0.5870, 0.1140));
                fixed invLum = 1.0 - lum;

                fixed3 overlay =
                    invLum * (1.0 - 2.0 * invLum * (1.0 - _Color.rgb)) +
                    (2.0 * lum * lum) * _Color.rgb;

                fixed3 rgb = lerp(tex.rgb, overlay, _Overlay);

                return fixed4(rgb, tex.a * _Color.a);
            }
            ENDCG
        }
    }
}