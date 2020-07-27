Shader "UnitBasic"
{
	Properties
	{
		_MainTex ("Color (RGB)", 2D) = "white" {}
		_Normal ("Normal (RGB)", 2D) = "white" {}
		_Spec ("Specular (RGB)", 2D) = "white" {}
		_Ambi ("Ambient (RGB)", 2D) = "white" {}
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf SimpleSpecular

		sampler2D _MainTex;
		sampler2D _Normal;
		sampler2D _Spec;
		sampler2D _Ambi;

		half4 LightingSimpleSpecular (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		{
			half3 h = normalize (lightDir + viewDir);
			
			half diff = max (0, dot (s.Normal, lightDir));

			half3 ambi = max(s.Specular * atten * 0.3, s.Emission);
			
			half4 c;
			c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * ambi);
			c.a = s.Alpha;
			return c;
		}


		struct Input
		{
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Normal = UnpackNormal (tex2D (_Normal, IN.uv_MainTex));
			o.Emission = tex2D (_Ambi, IN.uv_MainTex).rgb;
			o.Specular = tex2D (_Spec, IN.uv_MainTex).r;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
