Shader "Shaders/Esthesia_Shader"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}

		_ShadowPercentColor("Shadow Pourcent Color",Range(0.0,0.5)) = 0.01
		_ShadowStrenght("Shadow Strenght",Range(0.0,1.0)) = 0.5
		_ColorShadow("Color of Shadow",Color) = (1.0,1.0,1.0)

		_AmbientLevel("Ambient Level",Range(0.0,1.0)) = 0.4

		_Glossiness("Glossiness",Float) = 32
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
			float _AmbientLevel;
			float _ShadowPercentColor;
			float _ShadowStrenght;
			float3 _ColorShadow;

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.viewDir = WorldSpaceViewDir(v.vertex);
				TRANSFER_SHADOW(o);
				return o;
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
					light.rgb *= float3(0.62f, 0.81f, 1.0f);
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
