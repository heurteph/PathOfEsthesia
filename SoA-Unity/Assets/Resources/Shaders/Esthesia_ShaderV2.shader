Shader "Shaders/Esthesia_ShaderV2"
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
				float4 worldPos : WORLD_POS;
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

			//donner la taille max
			uniform float4 vector_col[16];
			uniform float4 vector_pos[16];
			uniform float4 vector_dir[16];
			uniform float4 vector_opt[16];
			uniform float vector_lenght = 0;

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
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
				
				
				float r, g, b;
				r = g = b = 0.0f;
				float4 light_sec = float4(r, g, b, 1.0f);

				for (int ibis = 0; ibis < vector_lenght; ibis++) {
					float attenuation = 0.0f;
					float3 vertexToLightSource = vector_pos[ibis].xyz - i.worldPos.xyz;
					float distance = length(vertexToLightSource);
					float3 light_dir = normalize(vertexToLightSource);
					float diff = max(dot(normal, light_dir), 0.0f);
					attenuation = (1.0 * vector_dir[ibis].w) / (distance * distance);

					if (vector_pos[ibis].w > 0.0f) {
						float angle = vector_pos[ibis].w / 2.0f;

						float theta = dot(light_dir, normalize(-vector_dir[ibis].xyz));
						if (theta < vector_opt[ibis].x) {
							diff = 0.0f;
						}
					}
					light_sec += ((diff > 0.0f ? 1.0f : 0.0f) * vector_col[ibis] * attenuation);

					//point
					/*if (vector_pos[ibis].w <= 0.0f) {
						float diff = max(dot(normal, light_dir), 0.0f);
						//float distance = length(vertexToLightSource);
						attenuation = (1.0 * vector_dir[ibis].w) / (distance * distance);
						light_sec += (diff * vector_col[ibis] * attenuation);
					}
					//spot light
					else {
						float angle = vector_pos[ibis].w / 2.0f;

						//
						float rangeFade = dot(light_dir, light_dir) * vector_opt[ibis].x;
						rangeFade = saturate(1.0f - rangeFade * rangeFade);
						rangeFade *= rangeFade;

						float spotFade = dot(vector_dir[ibis].xyz, light_dir);
						spotFade = saturate(spotFade * vector_opt[ibis].z + vector_opt[ibis].w);
						spotFade *= spotFade;

						float distanceSqr = max(dot(light_dir, light_dir), 0.00001f);
						light_sec += (vector_col[ibis] * ((spotFade * spotFade) / distanceSqr));



						float theta = dot(light_dir, normalize(-vector_dir[ibis].xyz));
						if (theta < angle) {
							attenuation = 1.0f; // (distance * distance) ;
						}
					}*/
					//light_sec += vector_col[ibis] * attenuation;
				}


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

				sample *= (_AmbientColor + light + light_sec);

				//if (vector_lenght == 1) {
				//	sample = float4(0.0f, 1.0f, 0.0f, 1.0f);
				//}

				return sample;
			}
			ENDCG
		}
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
}
