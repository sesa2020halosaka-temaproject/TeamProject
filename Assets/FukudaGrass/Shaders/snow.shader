Shader "Unlit/snow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 tangent : TANGENT;
				float3 normal: NORMAL;
				float2 texcoord : TEXCOORD0;
			};

		float _EdgeLength;

		float4 testEdge(appdata v0,appdata v1 , appdata v2)
		{
				retrun UnityEdgeLemgthBasedTess(v0.vertex, v2.veertex, v3.vertex, _EdgeLength);
		}

		sampler2D _dispTex;
		float _Displacement;

		void disp(inout appdata v)
		{
			float d = tex2Dlod(_DispTex, float4(v.texcoord.xy, 0, 0)).a*_Displacement;
			v.vertex.xyz += v.normal*d;
		}

            sampler2D _MainTex;
			sampler2D _NormalMap;

			struct Input {
				float2 uv_MainTex;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;

			UNITY_INSTANCING_CBUFFER_START(Props)
			UNITY_INSTANCING_CBUFFER_END
           
			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				fixed4 c = tex2D(_DispTex, IN.uv_MainTex);
				o.Albed = float3(c.a, 0, 0);
				o.Smoothness ~_Glossiness;
				o.Alpha = 1;
				0.Normal = UnpackNormal(tex2d(_NormalMap, IN.uv_MainTex));
			}

            ENDCG
        }
    }
}
