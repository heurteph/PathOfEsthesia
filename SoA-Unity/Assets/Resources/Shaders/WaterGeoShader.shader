Shader "Shaders/WaterGeoShader"
{
	Properties
	{
		[Header(Shading)]
		_Color("Color", Color) = (1,1,1,1)
		_TranslucentGain("Translucent Gain", Range(0,1)) = 0.5
		_Transparency("Transparency",Range(0,1)) = 1.0
		_Amplitude("Amplitude",Range(0,0.5)) = 0.08

		[HDR]
		_AmbientColor("Ambient",Color) = (0.4,0.4,0.4,1)
		[HDR]
		_SpecularColor("Spec Color",Color) = (0.9,0.9,0.9,1)
		_Glossiness("Glossiness",Float) = 32

		[HDR]
		_RimColor("Rim Color",Color) = (1,1,1,1)
		_RimAmount("Rim Amount",Range(0,1)) = 0.716
		_RimThreshold("Rim Threshold", Range(0,1)) = 0.1

		_WindDistoMap("Wind Disto Map", 2D) = "white"{}
		_WindFq("Wind Frequence",Vector) = (0.05,0.05,0,0)
		_WindStrength("Wind Strength",Float) = 1
	}

		CGINCLUDE
		#include "UnityCG.cginc"
		#include "Autolight.cginc"
		
		float4 _Color;
		float _WindStrength;
		float _Amplitude;
		
		float distance;
		int _DynamicRender;
		int _Transparency;

		float4 _AmbientColor;
		float _Glossiness;
		float4 _SpecularColor;
		float4 _RimColor;
		float _RimAmount;
		float _RimThreshold;



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
			float3 viewDir : TEXTCOORD1;
			float3 worldNormal : NORMAL;
			uint id : TEXCOORD2;
		};



		vertexOutput vert(vertexInput v)
		{
			vertexOutput o;

			
			//o.vertex = UnityObjectToClipPos(v.vertex);
			o.vertex = mul(unity_ObjectToWorld,v.vertex);
			o.worldNormal = UnityObjectToWorldNormal(v.normal);
			o.viewDir = WorldSpaceViewDir(v.vertex);

			o.tangent = v.tangent;
			o.id = v.id;
			return o;
		}

		//rand Trauma
		float rand(float3 myVector) {
			return frac(sin(dot(myVector, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
		}

		
		[maxvertexcount(3)]
		void geo(triangle vertexOutput IN[3], inout TriangleStream<vertexOutput> triStream) {
			
			float4 pos;

			float4 tmp;
			pos = IN[0].vertex;

			for (int i = 0; i < 3; i++) {
				pos = IN[i].vertex;
				pos.y += cos(_WindStrength*(_Time.y+IN[i].id))*_Amplitude + _Amplitude;
				IN[i].vertex = mul(UNITY_MATRIX_VP,pos);
				triStream.Append(IN[i]);
			}
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
				#pragma geometry geo
				#pragma fragment frag
				#pragma target 4.6

				#include "Lighting.cginc"


				float4 frag(vertexOutput i) : SV_Target{

					float3 viewDir = normalize(i.viewDir);

					float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);

					float3 normal = normalize(i.worldNormal);
					float NdotL = dot(_WorldSpaceLightPos0, normal);
					float NdotH = dot(normal, halfVector);
					float lightIntensity = smoothstep(0, 0.01, NdotL);

					float specularIntensity = pow(NdotH * lightIntensity, _Glossiness * _Glossiness);
					float specularIntensitySmooth = smoothstep(0.005, 0.01, specularIntensity);
					float4 specular = specularIntensitySmooth * _SpecularColor;

					float4 rimDot = 1 - dot(viewDir, normal);

					float rimIntensity = rimDot * pow(NdotL, _RimThreshold);
					rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);

					float4 rim = rimIntensity * _RimColor;

					float4 light = lightIntensity * _LightColor0;
					
					rim = float4(1.0f, 1.0f, 1.0f, 1.0f) - rim;
					rim.a = 1.0f;
					float4 res = _Color * (_AmbientColor + light + specular + rim);
					res.a = _Color.a;
					return res;
				}

				ENDCG
			}
		}
}
