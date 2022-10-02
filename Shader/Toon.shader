Shader "Custom/Toon"
{
	Properties{
		_Color("Main Color", Color) = (.5,.5,.5,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Ramp("Toon Ramp (RGB)", 2D) = "gray" {}
		_OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
		_OutlineWidth("Outline Width", Range(0, 0.05)) = 0.03
	}

		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			Pass
		{

			Cull Front

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float4 _OutlineColor;
			float _OutlineWidth;

			float4 vert(appdata_full v) : SV_POSITION
			{
				v.vertex.xyz += v.normal * _OutlineWidth;
			return UnityObjectToClipPos(v.vertex);
		}

		fixed4 frag() : SV_Target
		{
			return _OutlineColor;
		}

		ENDCG
	}

			CGPROGRAM
			#pragma surface surf ToonRamp

			sampler2D _Ramp;

	#pragma lighting ToonRamp exclude_path:prepass
	inline half4 LightingToonRamp(SurfaceOutput s, half3 lightDir, half atten)
	{
		#ifndef USING_DIRECTIONAL_LIGHT
			lightDir = normalize(lightDir);
		#endif

			half d = (dot(s.Normal, lightDir) * 0.5 + 0.5) * atten;
			half3 ramp = tex2D(_Ramp, float2(d,d)).rgb;

			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * ramp * 2;
			c.a = 0;
			return c;
	}

	sampler2D _MainTex;
	float4 _Color;

	struct Input {
		float2 uv_MainTex : TEXCOORD0;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}
	ENDCG

		}

			Fallback "Diffuse"
}
