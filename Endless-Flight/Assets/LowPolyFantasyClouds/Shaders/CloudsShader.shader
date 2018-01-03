// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "RandomGames/CloudsShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Thickness ("Thickness", Range(0,1)) = 0.5
	}
	SubShader {
		 Tags {"Queue" = "Transparent" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200
		
		CGPROGRAM
		#pragma surface surf SimpleFakeSubsurfaceScattering alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Thickness;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)
	

        half4 LightingSimpleFakeSubsurfaceScattering(SurfaceOutput s, half3 lightDir, half atten) {
            half NdotL = max(dot (s.Normal, lightDir),0);
			half NdotLInv = max(dot (-s.Normal, lightDir),0);
            half4 c;
            c.rgb = s.Albedo * _LightColor0.rgb * ((NdotL + NdotLInv * (1 - _Thickness))* atten);
            c.a = s.Alpha;
            return c;
        }

		void surf (Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Alpha = c.a * _Color.a;
		}
		ENDCG

		UsePass "Standard/SHADOWCASTER"
	}
	FallBack "Diffuse"
}
