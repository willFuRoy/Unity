Shader "Unlit/Transparent Packed"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		_AlphaTex("Alpha (A)", 2D) = "white" {}
	}

	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Offset -1, -1
			Fog { Mode Off }
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex, _AlphaTex;
			half4 _MainTex_ST;

			struct appdata_t
			{
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			v2f o;

			v2f vert (appdata_t v)
			{
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = v.texcoord;
				return o;
			}

			half4 frag (v2f IN) : COLOR
			{
				half4 mask = tex2D(_MainTex, IN.texcoord);
				half4 mixed = saturate(ceil(IN.color - 0.5));
				half4 col = saturate((mixed * 0.51 - IN.color) / -0.49);

				half a = tex2D(_AlphaTex, IN.texcoord).r;
				col.a *= a;

				mask *= mixed;
				col.a *= mask.r + mask.g + mask.b + mask.a;
				return col;
			}
			ENDCG
		}
	}
	Fallback Off
}
