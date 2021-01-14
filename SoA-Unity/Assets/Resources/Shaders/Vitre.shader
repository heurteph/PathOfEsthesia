Shader "Shaders/Vitre"
{
	Properties
	{
		[Header(Shading)]
		_Color("Color", Color) = (1,1,1,1)
		[HDR]
		_AmbientColor("Ambient",Color) = (0.4,0.4,0.4,1)
	}

		CGINCLUDE
		#include "UnityCG.cginc"
		#include "Autolight.cginc"
		
		uniform float4 _Color;
		float4 _AmbientColor;


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
			float3 worldNormal : NORMAL;
			uint id : TEXCOORD2;
		};

		vertexOutput vert(vertexInput v)
		{
			vertexOutput o;

			
			o.vertex = UnityObjectToClipPos(v.vertex);
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
				#pragma target 4.6

				#include "Lighting.cginc"


				float4 frag(vertexOutput i) : SV_Target{

					float3 viewDir = normalize(i.viewDir);
					float3 normal = normalize(i.worldNormal);
					float NdotL = dot(_WorldSpaceLightPos0, normal);
					float lightIntensity = smoothstep(0, 0.01, NdotL);

					float4 light = lightIntensity * _LightColor0;
					

					float4 res = _Color * (_AmbientColor + light);
					res.a = _Color.a;
					return res;
				}

				ENDCG
			}
		}
}
