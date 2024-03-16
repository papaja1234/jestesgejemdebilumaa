Shader "Custom/RetroStyle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainTex_TexelSize ("MainTex Texel Size", Vector) = (1,1,0,0)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            // Yeah hard coded stuff...
#define CURVATURE 0.2
#define BLUR 0.021
#define CA_AMT 1.01 // Chromatic Abberation
#define PS_X 4 // Pixel Spacing
#define PS_Y 4
#define PT_X 0.5 // Pixel Thickness
#define PT_Y 0.5
#define LINE_INTENSITY 0.9
#define PIXEL_INTENSITY 1.2

float random(inout uint state)
{
    state = state * 747796405 + 2891336453;
    uint result = ((state >> ((state >> 28) + 4)) ^ state) * 277803737;
    result = (result >> 22) ^ result;
    return result / 4294967295.0;
}

fixed4 frag(v2f i) : SV_Target
{
    float2 texSize = _MainTex_TexelSize.zw;

    // Noice!
    uint pixelIndex = (uint)(i.uv.y * texSize.y / PS_Y * texSize.x + i.uv.x * texSize.x / PS_X);
    float rnd = random(pixelIndex) * 0.1 + 0.95;

    // Curvature
    float2 curveUV = i.uv * 2 - 1;
    float2 offset = curveUV.yx * CURVATURE;
    curveUV += curveUV * offset * offset;
    curveUV = (curveUV + 1) / 2;

    // Blur
    float2 edge = smoothstep(0.0, BLUR, curveUV) * (1 - smoothstep(1 - BLUR, 1, curveUV));

    // Pixel
    float lineIntensity = PIXEL_INTENSITY;
    if (fmod(curveUV.x * texSize.x, PS_X) < 1.0 / PT_X || fmod(curveUV.y * texSize.y, PS_Y) < 1.0 / PT_Y)
        lineIntensity = LINE_INTENSITY;

    fixed4 lineCol = fixed4(lineIntensity, lineIntensity, lineIntensity, 1.0);

    // Chromatic Abberation
    fixed4 texCol = fixed4(
        tex2D(_MainTex, (curveUV - 0.5) * CA_AMT + 0.5)[0],
        tex2D(_MainTex, curveUV)[1],
        tex2D(_MainTex, (curveUV - 0.5) * CA_AMT + 0.5)[2],
        1.0
    );

    // Apply line effect before curvature effect
    fixed4 col = texCol * edge.x * edge.y * rnd * lineCol;

    return col;
}
            ENDCG
        }
    }
}
