// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Study/BumpSpecular" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpTex ("Bump (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags {
			"AmbientType" = "NULL"
//			"LightMode" = "Pixel"
//			"RenderType"="Transparent"
//			"Queue"="Transparent"
		}
//		Blend SrcAlpha OneMinusSrcAlpha
//		ZWrite Off

		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _BumpTex;

			struct Input {
				float4 vertex;
				float3 normal;
				float4 texcoord;
	//			float4 texcoord1;
				float4 tangent;
	//			float4 color;
			};

			struct v2f {
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
//				float3 normal : TEXCOORD1;
				float3 lightDir : TEXCOORD2;
				float3 halfVector : TEXCOORD3;
			};

			v2f vert (Input inp) {
				v2f outp;
				outp.position = UnityObjectToClipPos(inp.vertex);
				outp.uv = inp.texcoord.xy;
				
				float3 binormal = cross(inp.normal, inp.tangent.xyz) *inp.tangent.w;
				float3x3 TangentSpace = float3x3(inp.tangent.xyz, binormal, inp.normal );

//				outp.lightDir = normalize(ObjSpaceLightDir(inp.vertex));
				float3 lightDir = ObjSpaceLightDir(inp.vertex);
				outp.lightDir = mul(TangentSpace, lightDir.xyz);

				float3 eyeDir = ObjSpaceViewDir(inp.vertex);
//				eyeDir = mul(TangentSpace, eyeDirq);
				outp.halfVector = normalize(lightDir + eyeDir);
				outp.halfVector = mul(TangentSpace, outp.halfVector);
				return outp;
			}

			half4 frag (v2f inp) : COLOR {
//				inp.normal = normalize(inp.normal);
//				inp.lightDir = normalize(inp.lightDir);
				inp.halfVector = normalize(inp.halfVector);
				half3 normal = UnpackNormal(tex2D( _BumpTex, inp.uv)).rgb;
//				half3 normal = tex2D( _BumpTex, inp.uv).xyz * 2.0 - 1.0;
				float diffuseAttn = saturate(dot(inp.lightDir, normal));
				float specularAttn = pow( saturate(dot(normal, inp.halfVector)), 32);
				
//				return (tex2D (_MainTex, inp.uv) * diffuseAttn) + specularAttn;
				float4 tex = tex2D (_MainTex, inp.uv);
				float4 tex2 = tex * diffuseAttn + specularAttn*0.7;
				return float4(tex2.rgb, tex.a);
			}
			ENDCG
		}
	}
	FallBack "Transparent/Diffuse"
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