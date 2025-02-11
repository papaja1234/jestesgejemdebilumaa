Shader "INShaders/Unlit_ColorTransparent_GrayOverlay" {
	Properties {
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
		_Blend ("Blend", Float) = 1
	}
	
	SubShader{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard alpha:fade
#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		float _Blend;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = lerp(c.rgb, _Color.rgb, _Blend);//fixed dummy shader
			o.Alpha = c.a * _Color.a;
		}
		ENDCG
	}
}