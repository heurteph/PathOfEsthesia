Shader "Shaders/VerreOpaque"
{
	Properties
	{
		[Header(Shading)]
		_Color("Color", Color) = (1,1,1,1)
		_TranslucentGain("Translucent Gain", Range(0,1)) = 0.5
		_CoefTaille("Taille",Range(1,10)) = 1.0
		_Scale("Scale",float) = 0.1
		_CoefAttenuation("Attenuation", Range(1.0,10.0)) = 1.0
		_Transparency("Transparency",Range(0.0,1.0)) = 1.0
		
		[HDR]
		_AmbientColor("Ambient",Color) = (0.4,0.4,0.4,1)
		[HDR]
		_SpecularColor("Spec Color",Color) = (0.9,0.9,0.9,1)

		_Glossiness("Glossiness",Float) = 32

		[HDR]
		_RimColor("Rim Color",Color) = (1,1,1,1)
		_RimAmount("Rim Amount",Range(0,1)) = 0.716
		_RimThreshold("Rim Threshold", Range(0,1)) = 0.1
	}

		CGINCLUDE
		#include "UnityCG.cginc"
		#include "Autolight.cginc"
		
		float4 _Color;
		float _CoefTaille;
		float _Scale;
		float _CoefAttenuation;
		
		float4 _Light_Pos;
		float _Transparency;
		float _Intensity;

		//Toon var
		float4 _AmbientColor;
		float _Glossiness;
		float4 _SpecularColor;
		float4 _RimColor;
		float _RimAmount;
		float _RimThreshold;


		static const float repartition[5] = { 0.227027, 0.1945946, 0.1216216, 0.054054, 0.016216 };


		struct vertexInput
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float4 tangent : TANGENT;
			float4 uv : TEXCOORD0;
			uint id : SV_VertexID;
		};

		struct vertexOutput
		{
			float4 vertex : SV_POSITION;
			float4 tangent : TANGENT;
			float2 uv : TEXCOORD0;
			float3 viewDir : TEXTCOORD1;
			float4 reel_pos : POS;
			float3 worldNormal : NORMAL;
			uint id : TEXCOORD2;
		};

		vertexOutput vert(vertexInput v)
		{
			vertexOutput o;

			
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.worldNormal = UnityObjectToWorldNormal(v.normal);
			o.reel_pos = mul(unity_ObjectToWorld, v.vertex);
			o.viewDir = WorldSpaceViewDir(v.vertex);
			

			o.tangent = v.tangent;
			o.id = v.id;
			return o;
		}

		//rand Trauma
		float rand(float3 myVector) {
			return frac(sin(dot(myVector, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
		}



		ENDCG

			SubShader{
			Pass
			{
				Tags
				{
					"LightMode" = "ForwardBase"
					"PassFlags" = "OnlyDirectional"
					"Queue" = "Transparent"
					"RenderType" = "Transparent"
				}
				Blend SrcAlpha OneMinusSrcAlpha
				Cull off
				ZWrite off
				
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fwdbase
				#pragma target 4.6

				#include "Lighting.cginc"


				float4 frag(vertexOutput i) : SV_Target{
					float3 viewDir = normalize(i.viewDir);

					float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);

					float3 normal = normalize(i.worldNormal);
					float NdotL = dot(_WorldSpaceLightPos0, normal);
					float NdotH = dot(normal, halfVector);
					float lightIntensity = smoothstep(0, 0.01, NdotL);


					//Point Light calcul
					float3 vertexToLightSource = _Light_Pos.xyz - i.reel_pos.xyz;
					float distance = length(vertexToLightSource);
					float attenuation = 1.0 / (pow(distance, _CoefAttenuation));
					float3 lightDirection = normalize(vertexToLightSource);

					float3 diffuseReflection = _Intensity * attenuation * float3(1.0f, 1.0f, 1.0f);

					float4 light = float4(diffuseReflection, 1.0f);
					

					float4 res = _Color * light;
					res.a = _Transparency;
					return res;
				}

				ENDCG
			}
			UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
		}
}
