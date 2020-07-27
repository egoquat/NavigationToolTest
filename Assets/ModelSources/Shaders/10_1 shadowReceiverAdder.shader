// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Study/ShadowReceiverAdder" {
	Properties {
		_ShadowMap ("depthTexture", 2D) = "white" {}
	}
	SubShader {
		Tags { 
			"RenderType"="Transparent"
			"Queue" = "Transparent"
		}
//		LOD 5000
		Blend SrcAlpha OneMinusSrcAlpha
		Offset 0, -1
		ZWrite Off
		
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			
//			sampler2D _MainTex;
			sampler2D _ShadowMap;
//			float4x4 _Object2Light;
			uniform float4x4 depthcam_viewproj_matrix;
			uniform float4x4 depthcam_view_matrix;
//			float4 _ScreenParams;

			struct Input {
				float4 vertex;
	//			float3 normal;
				float4 texcoord;
	//			float4 texcoord1;
	//			float4 tangent;
	//			float4 color;
			};

			struct v2f {
				float4 position : POSITION;
//				float2 uv : TEXCOORD0;
				float2 depth : TEXCOORD1;
				float4 shadowCrd : TEXCOORD2;
//3				float4 projPos : TEXCOORD3;
			};

			v2f vert (Input inp) {
				v2f outp;
				outp.position = UnityObjectToClipPos(inp.vertex);
//				outp.uv = inp.texcoord.xy;
//3				outp.projPos = ComputeScreenPos(outp.position);
//3				outp.projPos.z = -mul( UNITY_MATRIX_MV, inp.vertex ).z;		//COMPUTE_EYEDEPTH(outp.projPos.z);

				float4 pos = mul(unity_ObjectToWorld, inp.vertex);
				float4 sPos = mul(depthcam_viewproj_matrix, pos);
				outp.depth = sPos.zw;

//				sPos.z += 10;
				outp.shadowCrd.x = 0.5 * sPos.w + (0.5 + 0.5/256) * sPos.x;
				outp.shadowCrd.y = 0.5 * sPos.w - (0.5 + 0.5/256) * sPos.y;
				outp.shadowCrd.z = sPos.z;
				outp.shadowCrd.w = sPos.w;
				

				return outp;
			}

			half4 frag (v2f inp) : COLOR {
//3				float shadowMap = LinearEyeDepth (tex2Dproj(_ShadowMap, UNITY_PROJ_COORD(inp.projPos)).r);

				float shadowMap = tex2Dproj(_ShadowMap, inp.shadowCrd);
				float shadow = (shadowMap > (inp.depth.x / inp.depth.y));
/*
				float screen = (1/2048.0)*inp.shadowCrd.w;
				shadowMap = tex2Dproj(_ShadowMap, inp.shadowCrd+float4( screen, 0, 0, 0));
				shadow += (shadowMap > (inp.depth.x / inp.depth.y));
				shadowMap = tex2Dproj(_ShadowMap, inp.shadowCrd+float4(-screen, 0, 0, 0));
				shadow += (shadowMap > (inp.depth.x / inp.depth.y));
				shadowMap = tex2Dproj(_ShadowMap, inp.shadowCrd+float4( 0, screen, 0, 0));
				shadow += (shadowMap > (inp.depth.x / inp.depth.y));
				shadowMap = tex2Dproj(_ShadowMap, inp.shadowCrd+float4( 0,-screen, 0, 0));
				shadow += (shadowMap > (inp.depth.x / inp.depth.y));
				shadow *= 0.2;
*/
//				shadow += (inp.depth.y <= 0);

//				float x = inp.shadowCrd.x / inp.shadowCrd.w;
//				float y = inp.shadowCrd.y / inp.shadowCrd.w;
//				shadow -= (x<0 || x>1 || y<0 || y>1);
				shadow -= (inp.shadowCrd.x<0 || inp.shadowCrd.x>inp.shadowCrd.w || inp.shadowCrd.y<0 || inp.shadowCrd.y>inp.shadowCrd.w);//out of shadow cam

				shadow = clamp(shadow, 0.6, 1);
				
//				return tex2D (_MainTex, inp.uv) * shadow;
				return float4(0,0,0,1-shadow);
			}
			ENDCG
		}
	} 
	FallBack Off
}

/*
	Built-in matrices
	Matrices (float4x4) supported:
		UNITY_MATRIX_MVP 
		Current model*view*projection matrix 
		UNITY_MATRIX_MV 
		Current model*view matrix 
		UNITY_MATRIX_P 
		Current projection matrix 
		UNITY_MATRIX_T_MV 
		Transpose of model*view matrix 
		UNITY_MATRIX_IT_MV 
		Inverse transpose of model*view matrix 
		UNITY_MATRIX_TEXTURE0 to UNITY_MATRIX_TEXTURE3 
		Texture transformation matrices 

	Built-in vectors
	Vectors (float4) supported: 
		UNITY_LIGHTMODEL_AMBIENT 
		Current ambient color. 
		
		
		
		
	Using them in programmable shaders requires including UnityCG.cginc file. 

	Transformations
		float4x4 UNITY_MATRIX_MVP 
		Current model*view*projection matrix 
		float4x4 UNITY_MATRIX_MV 
		Current model*view matrix 
		float4x4 UNITY_MATRIX_P 
		Current projection matrix 
		float4x4 UNITY_MATRIX_T_MV 
		Transpose of model*view matrix 
		float4x4 UNITY_MATRIX_IT_MV 
		Inverse transpose of model*view matrix 
		float4x4 UNITY_MATRIX_TEXTURE0 to UNITY_MATRIX_TEXTURE3 
		Texture transformation matrices 
		float4x4 _Object2World 
		Current model matrix 
		float4x4 _World2Object 
		Inverse of current world matrix 
		float3 _WorldSpaceCameraPos 
		World space position of the camera 
		float4 unity_Scale 
		xyz components unused; .w contains scale for uniformly scaled objects. 
	Lighting
	In plain ShaderLab, you access the following properties by appending zero at the end: e.g. the light's model*light color is _ModelLightColor0. In Cg shaders, they are exposed as arrays with a single element, so the same in Cg is _ModelLightColor[0]. 

	Name Type Value 
		_ModelLightColor float4 Material's Main * Light color 
		_SpecularLightColor float4 Material's Specular * Light color 
		_ObjectSpaceLightPos float4 Light's position in object space. w component is 0 for directional lights, 1 for other lights 
		_Light2World float4x4 Light to World space matrix 
		_World2Light float4x4 World to Light space matrix 
		_Object2Light float4x4 Object to Light space matrix 

	Various
		float4 _Time : Time (t/20, t, t*2, t*3), use to animate things inside the shaders 
		float4 _SinTime : Sine of time: (t/8, t/4, t/2, t) 
		float4 _CosTime : Cosine of time: (t/8, t/4, t/2, t) 
		float4 _ProjectionParams : 
			x is 1.0 or -1.0, negative if currently rendering with a flipped projection matrix 
			y is camera's near plane 
			z is camera's far plane 
			w is 1/FarPlane. 
		float4 _ScreenParams : 
			x is current render target width in pixels 
			y is current render target height in pixels 
			z is 1.0 + 1.0/width 
			w is 1.0 + 1.0/height 
*/