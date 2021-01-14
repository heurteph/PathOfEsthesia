Shader "Shaders/Light_Effect2"
{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_MaskTex("Mask texture", 2D) = "white" {}
		_maskBlend("Mask blending", Float) = 0.5
		_maskSize("Mask Size", Float) = 1
		_color("Color",Color) = (0.0,0.0,0.0,1.0)
		_Transparency("Transparency",Range(0.0,1.0)) = 1.0
		_Position("Position",Vector) = (0.0,0.0,0.0,0.0)
		_ecart("Ecart", Float) = 0.4
		_height("Hauteur",Float) = 10.0
		_PourcentHeight("Height display",Range(0.0,1.0)) = 1.0
	}
		SubShader{
			Pass{
			Tags
				{
					"Queue" = "Transparent"
					"RenderType" = "Transparent"
					"IgnoreProjector" = "True"
				}

			ZWrite Off
			Cull off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM	
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geo

			#include "UnityCG.cginc"


		shared float res = 0.0f;

		uniform sampler2D _MainTex;
		uniform sampler2D _MaskTex;
		uniform float type;
		uniform float time;
		uniform float height;
		uniform float width;
		uniform float4 _Position;
		uniform float _ecart;
		uniform float _height;
		uniform float4 _color;
		uniform float _PourcentHeight;
		uniform float height_scale;
		uniform float _Transparency;


		//pour composante Main_Text
		float4 _MainTex_ST;


		struct vertexInput {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float4 tangent : TANGENT;
			half2 uv : TEXCOORD0;
			uint id : SV_VertexID;
		};

		struct vertexOutput{
			float4 vertex : SV_POSITION;
			float3 normal : NORMAL;
			float4 tangent : TANGENT;
			half2 uv : TEXCOORD0;
			uint id : TEXCOORD1;
			bool up : TEXCOORD2;
		};

		struct geometryOutput{
			float4 pos : SV_POSITION;
		};


		fixed _maskBlend;
		fixed _maskSize;

		float rand(float3 myVector) {
			return frac(sin(dot(myVector, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
		}



		vertexOutput vert(vertexInput v)
		{
			vertexOutput o;
			if (v.vertex.y >= _height) {
				o.up = true;
			}
			else {
				o.up = false;
			}
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.id = v.id;
			o.normal = v.normal;
			o.tangent = v.tangent;
			o.uv = TRANSFORM_TEX(v.uv, _MainTex);

			return o;
		}

		
		[maxvertexcount(3)]
		void geo(triangle vertexOutput IN[3] : SV_POSITION, inout TriangleStream<vertexOutput> triStream) {
			geometryOutput o;
			vertexOutput vo;
			vo = IN[0];
			triStream.Append(vo);
			
			vo = IN[1];
			triStream.Append(vo);

			vo = IN[2];
			triStream.Append(vo);
		}


		fixed4 frag(vertexOutput im) : COLOR{
			float4 col = _color;
			
			float taille = (height_scale * 2.0f);

			float uv = taille - im.uv.y;
			

			
			int time = (int)_Time.w;
			float tmp = (tex2D(_MainTex, uv).x + tex2D(_MainTex, uv).y + tex2D(_MainTex, uv).z)/3.0f;
			float seuil = taille * _PourcentHeight;
			float alpha = im.uv.y / (taille - (taille - seuil));
			
			if (im.uv.y >= seuil) {
				discard;
			}
			
			col.a = (alpha * _Transparency);

			return col;		
		}
		ENDCG
	}
	}
}
