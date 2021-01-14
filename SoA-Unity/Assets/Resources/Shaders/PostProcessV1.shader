Shader "Shaders/PostProcessV1"
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

				uniform bool _StateBlur;
				uniform bool _StateChromatique;
				uniform bool _VignettePleine;

				uniform float _LerpEffect;

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

				v2f_img vert(float4 pos : POSITION,	float2 uv : TEXCOORD0)
				{
					v2f_img o;
					o.pos = UnityObjectToClipPos(pos);
					o.uv = uv;
					return o;
				}



				fixed4 frag(v2f_img im) : COLOR{
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
								sampleTex[i].r = tex2D(_MainTex, float2(uv.x + (1.0f / width), uv.y)).r;
								sampleTex[i].g = tex2D(_MainTex, uv).g;
								sampleTex[i].b = tex2D(_MainTex, float2(uv.x - (1.0f / width), uv.y)).b;
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
							col.r = tex2D(_MainTex, float2(im.uv.x + (1.0f / width), im.uv.y)).r;
							col.g = tex2D(_MainTex, float2(im.uv.x, im.uv.y)).g;
							col.b = tex2D(_MainTex, float2(im.uv.x - (1.0f / width), im.uv.y)).b;
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

					return lerp(base, col, _LerpEffect);
				}
			ENDCG
		}
	}
}
