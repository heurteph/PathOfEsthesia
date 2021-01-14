Shader "Shaders/Afficheur"
{
	Properties
	{
		_Color("Color", Color) = (0.5, 0.65, 1, 1)
		_AmbientLevel("Ambient Level",Range(0.0,1.0)) = 0.4
		_Glossiness("Glossiness",Float) = 32
		_ShadowPercentColor("Shadow Pourcent Color",Range(0.0,0.5)) = 0.01
		_ShadowStrenght("Shadow Strenght",Range(0.0,1.0)) = 0.5
		_ColorShadow("Color of Shadow",Color) = (1.0,1.0,1.0)
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
				float3 viewDir : TEXTCOORD1;
				float3 worldNormal : NORMAL;
				float4 worldPos : WORLD_POS;
				SHADOW_COORDS(2)
			};

			float4 _Color;
			float _AmbientLevel;
			float _Glossiness;
			float4 _SpecularColor;
			float _ShadowStrenght;
			float _ShadowPercentColor;
			float3 _ColorShadow;
			float4 _LightPos;
			float4 _LightColor;
			float _LightIntensity;
			float _LightRange;

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.viewDir = WorldSpaceViewDir(v.vertex);
				TRANSFER_SHADOW(o);
				return o;
			}


			float4 frag(v2f i) : SV_Target
			{
				float4 _AmbientColor;
				float3 viewDir = normalize(i.viewDir);

				float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);

				float3 normal = normalize(i.worldNormal);
				float NdotL = dot(_WorldSpaceLightPos0, normal);
				float NdotH = dot(normal, halfVector);

				float shadow = SHADOW_ATTENUATION(i);
				
				float lightIntensity = smoothstep(0, 0.01, NdotL * shadow);

				float specularIntensity = pow(NdotH * lightIntensity, _Glossiness * _Glossiness);

				float4 specular = specularIntensity * _SpecularColor;

				if (specularIntensity < 0.5) {
					specular = float4(0.0f, 0.0f, 0.0f, 0.0f);
				}
				else {
					specular = float4(0.2f, 0.2f, 0.2f, 1.0f);
				}

				float3 vertexToLightSource = _LightPos.xyz - i.worldPos.xyz;
				float distance = length(vertexToLightSource) * _LightRange;
				float attenuation = 1.0 / (distance * distance);
				float3 lightDirection = normalize(vertexToLightSource);

				float3 diffuseReflection = _LightIntensity * attenuation * _LightColor.rgb * max(0.0, dot(normal, lightDirection));

				float4 light = lightIntensity * _LightColor0;
				
				if (lightIntensity < 0.5) {
					light = float4(_ShadowPercentColor, _ShadowPercentColor, _ShadowPercentColor, 1.0f) * _LightColor0;
					light.rgb *= float3(0.62f, 0.81f, 1.0f);
					float tmp = max(_AmbientLevel - _ShadowStrenght, 0.0f);
					_AmbientColor = float4(tmp, tmp, tmp, 1.0f);
				}
				else {
					_AmbientColor = float4(_AmbientLevel, _AmbientLevel, _AmbientLevel,1.0f);
				}

				_AmbientColor *= UNITY_LIGHTMODEL_AMBIENT;

				_Color *= (_AmbientColor + light + specular + float4(diffuseReflection.r, diffuseReflection.g, diffuseReflection.b,1.0f));

				return _Color;
			}
			ENDCG
		}
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
}
