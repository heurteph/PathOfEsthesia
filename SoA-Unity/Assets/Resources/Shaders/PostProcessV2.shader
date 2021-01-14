Shader "Shaders/PostProcessV2"
{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_MaskTex("Mask texture", 2D) = "white" {}
		_maskBlend("Mask blending", Float) = 0.5
		_maskSize("Mask Size", Float) = 1
	}
		SubShader{
			Pass{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				shared float res = 0.0f;	
				uniform sampler2D _MainTex;
				uniform float4 _MainTex_ST;
				uniform float4 _MainTex_TexelSize;

				uniform sampler2D _MaskTex;
				uniform float _CoefBlur;
				uniform float type;
				uniform float time;
				uniform float height;
				uniform float width;
				uniform float life;
				uniform float _Radius;
				uniform float4 _OffsetColor;

				uniform float _Radius_Head_Min;
				uniform float _Radius_Head_Max;
				uniform float4 _Position_Head;
				uniform float4 _Color_Sense;


				uniform bool _StateBlur;
				uniform bool _StateChromatique;
				uniform bool _StateFeedBack;
				uniform bool _VignettePleine;

				uniform float _LerpEffect;


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
					float2 head_pos : TEXTCOORD1;
				};


				fixed _maskBlend;
				fixed _maskSize;

				float rand(float3 myVector) {
					return frac(sin(dot(myVector, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
				}

				float3 greyScale(float3 vec) {
					float value = (vec.r + vec.g + vec.b) / 3.0f;
					return float3(value, value, value);
				}

				static float2 offset = float2(1.0f / width, 1.0f / height);

				static const float gaussian[] = { 0.227027, 0.1945946, 0.1216216, 0.054054, 0.016216 };

				static half2 offsets[9] = { half2(-offset.x , offset.y), half2(0.0f, offset.y), half2(offset.x, offset.y),
				half2(-offset.x, 0.0f), half2(0.0f, 0.0f), half2(offset.x , 0.0f),
				half2(-offset.x, -offset.y), half2(0.0f, -offset.y), half2(offset.x, -offset.y) };


				static float kernel[] = { 1.0f,1.0f,1.0f,1.0f,-8.0f,1.0f,1.0f,1.0f,1.0f };

				static float kernel2[] = { -1.0f,-1.0f,-1.0f,-1.0f,8.0f,-1.0f,-1.0f,-1.0f,-1.0f };

				static float kernelCell[] = { -1.0f,-2.0f,-1.0f,0.0f,0.0f,0.0f,1.0f,2.0f,1.0f };

				static float kernelBlur[] = { 1.0f / 16.0f, 2.0f / 16.0f, 1.0f / 16.0f, 2.0f / 16.0f, 4.0f / 16.0f, 2.0f / 16.0f, 1.0f / 16.0f, 2.0f / 16.0f, 1.0f / 16.0f };

				static float kernelBlurDynamic[] = { 1.0f, 2.0f, 1.0f, 2.0f, 4.0f, 2.0f, 1.0f, 2.0f, 1.0f };

				//v2f_img vert(float4 pos : POSITION,	float2 uv : TEXCOORD0)
				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					

					/*float2 pos = float2(0.0, 0.0);
					if (_Position_Head.x < 1.0) {
						pos.x = _Position_Head.x;
					}
					else {
						pos.x = _Position_Head.x / _ScreenParams.x;
					}

					if (_Position_Head.y < 1.0) {
						pos.y = _Position_Head.y;
					}
					else {
						pos.y = _Position_Head.y / _ScreenParams.y;
					}

					o.head_pos = pos;*/

					
					float4 pos = float4(_Position_Head.x, _Position_Head.y, _Position_Head.z, 1.0f);
					//pos = mul(UNITY_MATRIX_VP, pos);//WorldSpaceViewDir(_Position_Head));
					pos.x /= pos.w;
					pos.y /= pos.w;

					o.head_pos.xy = _Position_Head.xy;//((pos.xy  * 0.5f) + 0.5f);// *_ScreenParams.xy;

					//o.head_pos.x /= _ScreenParams.x;//(2.0 * width);
					//o.head_pos.x += 0.5f;
					//o.head_pos.y /= _ScreenParams.y;//(2.0 * height);
					//o.head_pos.y += 0.5f;
					return o;
				}



				//fixed4 frag(v2f_img im) : COLOR{
				fixed4 frag(v2f im) : COLOR{
					fixed4 mask = tex2D(_MaskTex, im.uv * _maskSize);
					fixed4 base = tex2D(_MainTex, im.uv);
					int i;
					int j;
					float4 col = float4(0.0f, 0.0f, 0.0f, 1.0f);
					float4 sampleTex[9];

					if (_StateBlur){
						for (i = 0; i < 9; i++) {
							float2 uv = im.uv;
							uv.x += offsets[i].x;
							uv.y += offsets[i].y;
							if (_StateChromatique) {
								sampleTex[i].r = tex2D(_MainTex, float2(uv.x + (_OffsetColor.x / width), uv.y)).r;
								sampleTex[i].g = tex2D(_MainTex, float2(uv.x + (_OffsetColor.y / width), uv.y)).g;
								sampleTex[i].b = tex2D(_MainTex, float2(uv.x - (_OffsetColor.z / width), uv.y)).b;
								sampleTex[i].a = 1.0f;
							}
							else {
								sampleTex[i] = tex2D(_MainTex, uv);
							}
						}

						for (i = 0; i < 9; i++) {
							col += (sampleTex[i] * kernelBlurDynamic[i] / _CoefBlur);
						}
					}
					else {
						if (_StateChromatique) {
							col.r = tex2D(_MainTex, float2(im.uv.x + (_OffsetColor.x / width), im.uv.y)).r;
							col.g = tex2D(_MainTex, float2(im.uv.x + (_OffsetColor.y / width), im.uv.y)).g;
							col.b = tex2D(_MainTex, float2(im.uv.x + (_OffsetColor.z / width), im.uv.y)).b;
						}
						else
							col = base;
					}

					float2 centre = float2(0.5f, 0.5f);

					float dist = length(centre - im.uv);
					if (dist >= _Radius) {
						if(_VignettePleine)
							col = float4(0.0f, 0.0f, 0.0f, 1.0f);
						else
							col = lerp(col, float4(0.0f, 0.0f, 0.0f, 1.0f), dist);
					}

					col = lerp(base, col, _LerpEffect);

					//if(abs(im.uv.x - im.head_pos.x) <= 10.0 )
					if (_StateFeedBack) {
						float x = im.uv.x * _ScreenParams.x;
						float y = im.uv.y * _ScreenParams.y;
						float len = length(im.head_pos - float2(x, y));
						if (len >= _Radius_Head_Min && len <= _Radius_Head_Max) {
							float4 tmp = float4(_Color_Sense.r, _Color_Sense.g, _Color_Sense.b, _Position_Head.w*0.6f);
							col = lerp(tmp, col, abs(cos(_Time.z + (len - _Radius_Head_Min) / (_Radius_Head_Max - _Radius_Head_Min))));
						}
					}

					return col;//lerp(base, col, _LerpEffect);
				}
			ENDCG
		}
	}
}
