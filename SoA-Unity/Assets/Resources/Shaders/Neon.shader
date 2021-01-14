Shader "Shaders/Neon"
{
	Properties
	{
		[Header(Shading)]
		_Color("Color", Color) = (1,1,1,1)
		_TranslucentGain("Translucent Gain", Range(0,1)) = 0.5
		_Transparency("Transparency",Range(0.0,1.0)) = 1.0



		_On("Neon variation activate",Int) = 0
		_MinIntensity("Minimum intensity reach",Range(0.0,1.0)) = 0.0
	}

		CGINCLUDE
		#include "UnityCG.cginc"
		#include "Autolight.cginc"
		
		#define BLADE_SEGMENTS 3
		float4 _Color;
		
		float _Transparency;
		float _Intensity;

		int _On;
		float _MinIntensity;

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
					float3 normal = normalize(i.worldNormal);
					float NdotL = dot(_WorldSpaceLightPos0, normal);
					float lightIntensity = smoothstep(0, 0.01, NdotL);



					float intensity = max(_MinIntensity, _Intensity * (_On != 0 ? (abs(_SinTime.y)) : 1.0f));



					float3 diffuseReflection = intensity * float3(1.0f, 1.0f, 1.0f);

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
