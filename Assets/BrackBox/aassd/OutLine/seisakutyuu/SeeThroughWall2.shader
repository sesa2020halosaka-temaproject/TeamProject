﻿
Shader "chenjd/SeeThroughWall2"
{
	Properties
	{
		[NoScaleOffset]_MainTex("Render Texture", 2D) = "white" {}
		[NoScaleOffset]_MaskTex("Render Texture", 2D) = "white" {}
		_SubTex("Pattern Texture", 2D) = "white" {}
		[HDR]_Color("Main Color", Color) = (1,1,1,1)
		_Outline("Outline thickness", Range(0,1)) = 0.1
		_DrawNum("Drawing times", Range(2,32)) = 2
	}
		SubShader
		{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
			LOD 100
			ZWrite off
			Blend srcAlpha OneMinusSrcAlpha

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				#pragma multi_compile_fog

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				sampler2D _MaskTex;
				float4 _MainTex_ST;
				float4 _MainTex_TexelSize;
				sampler2D _SubTex;
				float4 _SubTex_ST;
				float4 _SubTex_TexelSize;
				float4 _Color;
				float _Outline;
				int _DrawNum;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				float2 PolarToCartesian(float r, float theta)
				{
					float x = r * cos(theta);
					float y = r * sin(theta);
					return float2(x, y);
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv);
					fixed4 Maskcol = tex2D(_MaskTex, i.uv);

					float2 tiling_offset_uv = float2(i.uv.xy *  _SubTex_ST *float2(1, _MainTex_TexelSize.x / _MainTex_TexelSize.y) + _SubTex_ST.zw);
					fixed4 subcol = tex2D(_SubTex, tiling_offset_uv);

					float2 outline = _Outline * 0.1;

					float PI = 3.14159265358979323;
					int n = _DrawNum;
					float theta;
					for (int j = 0; j < 2 * n; j++)
					{
						theta = PI * j / n;
						col += tex2D(_MainTex, i.uv + PolarToCartesian(outline, theta)*float2(_MainTex_TexelSize.x / _MainTex_TexelSize.y, 1));
					}
					//境界をなめらかに
					col = smoothstep(0.5, 0.51, col);
					// apply fog
					UNITY_APPLY_FOG(i.fogCoord, col);




					//一定以下のalphaはゼロにする
					_Color.a *= step(1 - col.a, 0.01);


					_Color.a -= Maskcol.a;
					return subcol * _Color;
				}
				ENDCG
			}
		}
}
