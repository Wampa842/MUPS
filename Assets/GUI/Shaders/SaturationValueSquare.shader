Shader "MUPS/Unlit/UV to saturation-value"
{
	Properties
	{
		_Hue("Hue", range(0, 1)) = 0.0
	}
		SubShader
	{
		Tags { "RenderType" = "Overlay" }
		

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float _Hue;

			struct appdata
			{
				float4 vertex : POSITION;
				float2  uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float3 desaturate(float3 rgb, float t)
			{
				return lerp(rgb, 1, t);
			}

			float3 hue2rgb(float hue)
			{
				float r = abs(hue * 6 - 3) - 1;
				float g = 2 - abs(hue * 6 - 2);
				float b = 2 - abs(hue * 6 - 4);
				return saturate(float3(r, g, b));
			}
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float3 rgb = lerp(hue2rgb(_Hue), 1, 1 - i.uv.x) * i.uv.y;	// Linear colorspace - Unity probably doesn't need conversion to srgb.
				//float3 rgb = pow(max(1e-5f, lerp(hue2rgb(_Hue), 1, 1 - i.uv.x) * i.uv.y;), 2.2);	
				return fixed4(rgb.x, rgb.y, rgb.z, 1);
			}
			ENDCG
		}
	}
}