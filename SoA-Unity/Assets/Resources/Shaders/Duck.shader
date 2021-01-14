Shader "Shaders/Duck"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		
		_ShadowPercentColor("Shadow Pourcent Color",Range(0.0,0.5)) = 0.01
		_ShadowStrenght("Shadow Strenght",Range(0.0,1.0)) = 0.5
		
		_AmbientLevel("Ambient Level",Range(0.0,1.0)) = 0.4

		_Amplitude("Amplitude",Range(0,10)) = 0.25
		_OffSetFactor("Factor OffSet",Range(0.0,0.5)) = 0.01
		_WindStrength("Wind Strength",Float) = 1
		
	}
		SubShader
	{
		Pass
		{
			Tags
			{
				"LightMode" = "ForwardBase"
				"PassFlags" = "OnlyDirectional"
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geo
			#pragma multi_compile_fwdbase

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 viewDir : TEXTCOORD1;
				float3 worldNormal : NORMAL;
				SHADOW_COORDS(2)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float _AmbientLevel;
			float _ShadowPercentColor;
			float _ShadowStrenght;
			float _OffSetFactor;
			float _WindStrength;
			float _Amplitude;
			float _Id;

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = mul(unity_ObjectToWorld, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.viewDir = WorldSpaceViewDir(v.vertex);
				TRANSFER_SHADOW(o);
				return o;
			}

			[maxvertexcount(3)]
			void geo(triangle v2f IN[3], inout TriangleStream<v2f> triStream) {
				float4 pos;

				float2 offSet = float2(cos(_Id + _Time.y)*_OffSetFactor, sin(_Id + _Time.y)*_OffSetFactor);

				for (int i = 0; i < 3; i++) {
					pos = IN[i].pos;
					pos.x += offSet.x;
					pos.z += offSet.y;
					pos.y += cos(_WindStrength*(_Time.y + _Id))*_Amplitude + _Amplitude;
					IN[i].worldNormal = UnityObjectToWorldNormal(IN[i].worldNormal);
					IN[i].pos = mul(UNITY_MATRIX_VP, pos);
					TRANSFER_SHADOW(IN[i]);
					triStream.Append(IN[i]);
				}
			}


			float4 frag(v2f i) : SV_Target
			{
				float4 sample = tex2D(_MainTex, i.uv);
				float4 flat = sample;
				float4 _AmbientColor;
				float3 viewDir = normalize(i.viewDir);

				float3 normal = normalize(i.worldNormal);
				float NdotL = dot(_WorldSpaceLightPos0, normal);
				float shadow = SHADOW_ATTENUATION(i);


				float lightIntensity = smoothstep(0, 0.01, NdotL * shadow);


				float4 light = lightIntensity * _LightColor0;


				if (lightIntensity < 0.5) {
					light = float4(_ShadowPercentColor, _ShadowPercentColor, _ShadowPercentColor, 1.0f) * _LightColor0;
					float tmp = max((_AmbientLevel - _ShadowStrenght), 0.0f);
					_AmbientColor = float4(tmp, tmp, tmp, 1.0f);
				}
				else {
					_AmbientColor = float4(_AmbientLevel, _AmbientLevel, _AmbientLevel, _AmbientLevel);
				}


				_AmbientColor *= UNITY_LIGHTMODEL_AMBIENT;

				sample *= (_AmbientColor + light);

				return sample;
			}
			ENDCG
		}
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
}
