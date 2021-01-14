Shader "Shaders/DecorsRoad"
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
		//light manager a faire ! debut puis dynamique en fonction de la distance avec joueur
		Pass
		{
		//Blend One One
			Tags
			{
				"LightMode" = "ForwardBase"
				//"PassFlags" = "OnlyDirectional"
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
				float4 worldPos : WORLD_POS;
				float4 viewPos : VIEW_POS;
				float3 worldNormal : NORMAL;
				SHADOW_COORDS(2)
			};

			float4 _Color;
			float _AmbientLevel;
			float _Glossiness;
			float4 _SpecularColor;
			float _ShadowStrenght;
			float _ShadowPercentColor;
			float3 _ColorShadow;
			//donner la taille max
			uniform float4 vector_col[128];
			uniform float4 vector_pos[128];
			uniform float4 vector_dir[128];
			uniform float4 vector_opt[128];
			uniform float vector_lenght = 0;

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.viewPos = mul(UNITY_MATRIX_MV, v.vertex);
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

				float4 light = lightIntensity * _LightColor0;
				

				//dir / point / spot light

				//V2
				//vector_opt x => intensity y => to 

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

					//point
					/*if (vector_pos[ibis].w <= 0.0f) {
						
						light_sec +=  (diff * vector_col[ibis] * attenuation);
					}
					//spot light
					else{*/
					if(vector_pos[ibis].w > 0.0f){
						float angle = vector_pos[ibis].w/2.0f;


						float theta = dot(light_dir, normalize(-vector_dir[ibis].xyz));
						//float epsilon = vector_opt[ibis].x - vector_opt[ibis].y;
						//float intensi = clamp((theta - vector_opt[ibis].Y) / epsilon, 0.0f, 1.0f);
						/*if (theta > vector_opt[ibis].x) {
							light_sec += (diff * vector_col[ibis] * attenuation);
						}*/
						/*if (theta < vector_opt[ibis].x) {
							diff = 0.0f;
						}*/

						
						//float3 light_dir = normalize(vertexToLightSource);

						//
						/*float rangeFade = dot(light_dir, light_dir) * vector_opt[ibis].x;
						rangeFade = saturate(1.0f - rangeFade * rangeFade);
						rangeFade *= rangeFade;

						float spotFade = dot(vector_dir[ibis].xyz, light_dir);
						spotFade = saturate(spotFade * vector_opt[ibis].z + vector_opt[ibis].w);
						spotFade *= spotFade;

						float distanceSqr = max(dot(light_dir, light_dir),0.00001f);
						//light_sec += (vector_col[ibis] * ((spotFade * spotFade)/distanceSqr));



						float theta = dot(light_dir, normalize(-vector_dir[ibis].xyz));
						if (theta < angle) {
							attenuation = 1.0f; // (distance * distance) ;
						}*/
					}
					light_sec += (diff * vector_col[ibis] * attenuation);
					//light_sec += vector_col[ibis] * attenuation;
				}

				//V1
				/*float r, g, b;
				r = g = b = 0.0f;
				float4 light_sec = float4(r, g, b, 1.0f);
				for (int ibis = 0; ibis < 8; ibis++) {
					//float3 pos = mul(UNITY_MATRIX_IT_MV,unity_LightPosition[ibis].xyz);
					//pos = mul(UNITY_MATRIX_M, pos);
					
					float3 pos = mul(unity_CameraToWorld, unity_LightPosition[ibis]).xyz;
					
					//float3 pos = unity_LightPosition[ibis].xyz;
					//float3 vertexToLightSource = pos.xyz - i.worldPos.xyz;
					float3 vertexToLightSource = pos.xyz - i.worldPos.xyz;

					float distance = length(vertexToLightSource);
					float attenuation = 0.0f;
					float4 _color_light = unity_LightColor[ibis];
					//float attenuation = 1.0 / (distance * distance);
					//unity_LightAtten[ibis];
					//x is cos sportAngle/2.0f phi y 1/cos(spotlight/4) theta z=> quadratic attenuation

					//while => pour arrete si array plus petit
					if (unity_LightColor[ibis].r != unity_LightColor[ibis].g != unity_LightColor[ibis].b != 0.0f) {
						if (unity_LightAtten[ibis].x != -1) {//spot
							float angle = acos(unity_LightAtten[ibis].x) * 2.0f;//in radius
							float theta = dot(unity_SpotDirection[ibis], normalize(-unity_SpotDirection[ibis]));
							if (theta < angle) {
								_color_light = float4(0.0f, 0.0f, 0.0f, 0.0f);
								attenuation = 1.0f;
							}
						}
						else {//point
							attenuation = 1.0 / (distance * distance);//unity_LightAtten[ibis].z;
						}
						light_sec += _color_light * attenuation;
					}
					//else if(ibis == 1 && (unity_LightColor[ibis].r == unity_LightColor[ibis].g == unity_LightColor[ibis].b == 0.0f)) {
					//	light_sec = float4(0.0f, 1.0f, 0.0f, 1.0f);
					//}

					//light_sec += unity_LightColor[ibis];// * attenuation;

					//float3 lightDirection = normalize(vertexToLightSource);
				}*/


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

				_Color *= (_AmbientColor + light + light_sec + specular);
				//_Color *= (_AmbientColor + light_sec);

				//if (vector_lenght)
				//	_Color = float4(0.0f, 1.0f, 0.0f, 1.0f);

				return _Color;
			}
			ENDCG
		}


		/*Pass
		{
			//Blend One One

			Tags
			{
				"LightMode" = "ForwardBase"//Add"
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwd//add

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
				float4 worldPos : WORLD_POS;
				float4 viewPos : VIEW_POS;
				float3 worldNormal : NORMAL;
				SHADOW_COORDS(2)
			};

			float4 _Color;
			float _AmbientLevel;
			float _Glossiness;
			float4 _SpecularColor;
			float _ShadowStrenght;
			float _ShadowPercentColor;
			float3 _ColorShadow;
			//donner la taille max
			uniform float4 vector_col[8];
			uniform float4 vector_pos[8];
			uniform float4 vector_dir[8];
			uniform float4 vector_opt[8];
			uniform float vector_lenght;

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.viewPos = mul(UNITY_MATRIX_MV, v.vertex);
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

				float4 light = lightIntensity * _LightColor0;


				//dir / point / spot light

				//V2
				//vector_opt x => intensity y => to 

				float r, g, b;
				r = g = b = 0.0f;
				float4 light_sec = float4(r, g, b, 1.0f);

				for (int ibis = 0; ibis < vector_lenght; ibis++) {
					float attenuation = 0.0f;
					float3 vertexToLightSource = vector_pos[ibis].xyz - i.worldPos.xyz;
					float distance = length(vertexToLightSource);

					//point
					if (vector_pos[ibis].w == -1.0f) {

						//float distance = length(vertexToLightSource);
						attenuation = 1.0 / (distance * distance);
						light_sec += vector_col[ibis] * attenuation;
					}
					//spot light
					else {
						float angle = vector_pos[ibis].w/2.0f;

						float3 light_dir = normalize(vertexToLightSource);

						//
						float rangeFade = dot(light_dir, light_dir) * vector_opt[ibis].x;
						rangeFade = saturate(1.0f - rangeFade * rangeFade);
						rangeFade *= rangeFade;

						float spotFade = dot(vector_dir[ibis].xyz, light_dir);
						spotFade = saturate(spotFade * vector_opt[ibis].z + vector_opt[ibis].w);
						spotFade *= spotFade;

						float distanceSqr = max(dot(light_dir, light_dir),0.00001f);
						light_sec += (vector_col[ibis] * ((spotFade * spotFade)/distanceSqr));



						float theta = dot(light_dir, normalize(-vector_dir[ibis].xyz));
						if (theta < angle) {
							attenuation = 1.0f; // (distance * distance) ;
						}
					}
					//light_sec += vector_col[ibis] * attenuation;
				}

				//V1
				float r, g, b;
				r = g = b = 0.0f;
				float4 light_sec = float4(r, g, b, 1.0f);
				for (int ibis = 0; ibis < 8; ibis++) {
					//float3 pos = mul(UNITY_MATRIX_IT_MV,unity_LightPosition[ibis].xyz);
					//pos = mul(UNITY_MATRIX_M, pos);

					float3 pos = mul(unity_CameraToWorld, unity_LightPosition[ibis]).xyz;

					//float3 pos = unity_LightPosition[ibis].xyz;
					//float3 vertexToLightSource = pos.xyz - i.worldPos.xyz;
					float3 vertexToLightSource = pos.xyz - i.worldPos.xyz;

					float distance = length(vertexToLightSource);
					float attenuation = 0.0f;
					float4 _color_light = unity_LightColor[ibis];
					//float attenuation = 1.0 / (distance * distance);
					//unity_LightAtten[ibis];
					//x is cos sportAngle/2.0f phi y 1/cos(spotlight/4) theta z=> quadratic attenuation

					//while => pour arrete si array plus petit
					if (unity_LightColor[ibis].r != unity_LightColor[ibis].g != unity_LightColor[ibis].b != 0.0f) {
						if (unity_LightAtten[ibis].x != -1) {//spot
							float angle = acos(unity_LightAtten[ibis].x) * 2.0f;//in radius
							float theta = dot(unity_SpotDirection[ibis], normalize(-unity_SpotDirection[ibis]));
							if (theta < angle) {
								_color_light = float4(0.0f, 0.0f, 0.0f, 0.0f);
								attenuation = 1.0f;
							}
						}
						else {//point
							attenuation = 1.0 / (distance * distance);//unity_LightAtten[ibis].z;
						}
						light_sec += _color_light * attenuation;
					}
					//else if(ibis == 1 && (unity_LightColor[ibis].r == unity_LightColor[ibis].g == unity_LightColor[ibis].b == 0.0f)) {
					//	light_sec = float4(0.0f, 1.0f, 0.0f, 1.0f);
					//}

					//light_sec += unity_LightColor[ibis];// * attenuation;

					//float3 lightDirection = normalize(vertexToLightSource);
				}


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

				_Color *= (_AmbientColor + light_sec);

				return _Color;
			}
			ENDCG
		}*/



		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
}
