Shader "Shaders/AfficheShader"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		[HDR]
		_ContourColor("Contour Color",Color) = (1.0,1.0,0.0,1.0)
		_Size_Contour("Contour Size",Float) = 1.0
		_Transparency("Transparence",Range(0,1)) = 0.5

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
			#pragma multi_compile DIRECTIONAL POINT
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
			float4 _ContourColor;
			float _Size_Contour;
			float4 _MainTex_ST;
			
			float _Transparency;
			float _LightIntensity;

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
				float3 viewDir = normalize(i.viewDir);


				if (i.uv.x <= _Size_Contour) {
					sample += (1.0f - (i.uv.x/_Size_Contour)) * _ContourColor;
				}
				else if (i.uv.x >= (1.0f - _Size_Contour)) {
					sample += (1.0f - ( (1.0f - i.uv.x) / _Size_Contour)) * _ContourColor;
				}
				if (i.uv.y <= _Size_Contour) {
					sample += (1.0f - (i.uv.y / _Size_Contour)) * _ContourColor;
				}
				else if (i.uv.y >= (1.0f - _Size_Contour)) {
					sample += (1.0f - ((1.0f - i.uv.y) / _Size_Contour)) * _ContourColor;
				}

				float3 normal = normalize(i.worldNormal);
				float NdotL = dot(_WorldSpaceLightPos0, normal);

				float shadow = SHADOW_ATTENUATION(i);
				float lightIntensity = smoothstep(0, 0.01, NdotL * shadow);

				
				float3 diffuseReflection = _LightIntensity * float3(1.0f, 1.0f, 1.0f);


				float4 light = lightIntensity * _LightColor0;
				light += float4(diffuseReflection, 1.0f);



				sample *= light;
				sample.a = _Transparency;
				return sample;
			}
			ENDCG
		}
		//after a Pass UsePass => grabs a pass from different shader and inserts into our shader
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
}
