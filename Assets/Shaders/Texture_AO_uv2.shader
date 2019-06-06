Shader "Custom/Texture_AO_uv2"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,0.5,0.5,1)
		_AoTex ("AO Texture", 2D) = "white" {}
		_Filter ("Filter", Range(0, 1)) = 1
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
			#pragma fragmentoption ARB_precision_hint_fastest
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

            sampler2D _MainTex;
			sampler2D _AoTex;
			float4 _Color;
			float4 _MainTex_ST;
			float4 _AoTex_ST;
			fixed _Filter;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv2 = TRANSFORM_TEX(v.uv2, _AoTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 col2 = tex2D(_AoTex, i.uv2) ;//+ _Color ;
				fixed  gven = _Color * (1-col2);
				fixed  mapet = gven * _Color;
				//fixed  mister = col * col2;
				col.rgb = col * col2 + (mapet* (col.b * _Color));//col.rgb = col * col2 + (mapet* (col.b * _Color * col2));
				return col;
			}
			ENDCG
		}
	}
}

