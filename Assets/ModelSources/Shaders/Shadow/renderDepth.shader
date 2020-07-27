// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Render Depth" {
	SubShader {
		Tags { "RenderType"="Opaque" }
		Pass {
			Fog { Mode Off }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform float4x4 depthcam_viewproj_matrix;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 depth : TEXCOORD0;
			};

			v2f vert (appdata_base v) {
				v2f outp;
				
//				outp.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				float4 pos = mul(unity_ObjectToWorld, v.vertex);
				outp.pos = mul(depthcam_viewproj_matrix, pos);
				
				outp.depth = outp.pos.zw;		//UNITY_TRANSFER_DEPTH(o.depth);
				return outp;
			}

			half4 frag(v2f inp) : COLOR {
				return inp.depth.x/inp.depth.y;	//UNITY_OUTPUT_DEPTH(i.depth);
			}
			ENDCG
		}
	}
}