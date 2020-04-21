Shader "Shader/Test" {
//#pragma target 4.6
//
//#pragma vertex Vertex
//#pragma geometry Geometry
//#pragma fragment Fragment
//#pragma only_renderers d3d11 ps4 xboxone vulkan metal
//
		Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geom
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
			float4 _MainTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			[maxvertexcount(6)]
				void geom(triangle v2f IN[3], inout TriangleStream<v2f> triStream)
			{
				//v2f o;

				////法線ベクトルの計算(ライティングで使用)
				//float3 vecA = IN[1].vertex - IN[0].vertex;
				//float3 vecB = IN[2].vertex - IN[0].vertex;
				//float3 normal = cross(vecA, vecB);
				//normal = normalize(mul(normal, (float3x3) unity_WorldToObject));

				////ライティングの計算
				//float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				//o.light = max(0., dot(normal, lightDir));

				//o.uv = (IN[0].uv + IN[1].uv + IN[2].uv) / 3;

				////メッシュ作成
				for (int i = 0; i < 3; i++)
				{
					triStream.Append(IN[i]);
				}
				// tristream.RestartStrip();
				for (int i = 0; i < 3; i++)
				{
					v2f o = IN[i];

					o.vertex.y += 10.0f;

					triStream.Append(o);
				}
				//tristream.RestartStrip();//さらに他の三角メッシュを作成する時に必要
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
			// apply fog
			UNITY_APPLY_FOG(i.fogCoord, col);
			return col;
			}


		ENDCG
	}
	}
}
