Shader "UI_Shader/Effect/distort_blend_SOFTMASK" {
	Properties {
		__Brightness("Brightness",Float) = 1
		_Contrast ("Contrast", Float ) = 1
		_MainColor ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Main Tex (A)", 2D) = "white" {}
		_MainPannerX  ("Main Panner X", Float) = 0
		_MainPannerY  ("Main Panner Y", Float) = 0
		_TurbulenceTex ("Turbulence Tex", 2D) = "white" {}
		_MaskTex ("Mask Tex", 2D) = "white" {}
		_DistortPower  ("Distort Power", Float) = 0
		_PowerX  ("Power X", range (0,1)) = 0
		_PowerY  ("Power Y", range (0,1)) = 0


		//_StencilWriteMask        ("Stencil Write Mask", Float) = 255
		//_StencilReadMask        ("Stencil Read Mask", Float) = 255
		_ColorMask         ("Color Mask", Float) = 15
		_SoftMask          ("Mask", 2D) = "white" {}

        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
	}

	SubShader
	{
		Tags {
			"Queue"="Transparent" 
			"RenderType"="Transparent" 
		}
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		Lighting Off
		ZWrite Off


		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "SoftMask.cginc"

	        #pragma multi_compile __ SOFTMASK_SIMPLE SOFTMASK_SLICED SOFTMASK_TILED


			struct appdata_t 
			{
				float4 vertex       : POSITION;
				float4 color        : COLOR;
				float4 vertexColor  : COLOR;
				float2 texcoord     : TEXCOORD0;
			};

			struct v2f {
				float4 vertex       : POSITION;
				float4 color        : COLOR;
				float4 vertexColor  : COLOR2;
				float2 uvmain       : TEXCOORD1;
				float2 uvnoise      : TEXCOORD2;
				float2 uvMask       : TEXCOORD3;
				SOFTMASK_COORDS(4)
			};

			float _Brightness;
			float _Contrast;
			float4 _MainColor;
			float _PowerX;
			float _PowerY;
			float _DistortPower;
			float4 _MainTex_ST;
			float4 _TurbulenceTex_ST;
			float4 _MaskTex_ST;
			float _Type;
			float _MainPannerX;
			float _MainPannerY;

			sampler2D _TurbulenceTex;
			sampler2D _MainTex;
			sampler2D _MaskTex;

			v2f vert (appdata_t v)
			
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertexColor = v.vertexColor;
				o.uvmain = TRANSFORM_TEX( v.texcoord, _MainTex );
				o.uvnoise = TRANSFORM_TEX( v.texcoord, _TurbulenceTex);
				o.uvMask = TRANSFORM_TEX(v.texcoord, _MaskTex);

				SOFTMASK_CALCULATE_COORDS(o, v.vertex);

				return o;
			}

			float4 frag( v2f i ) : COLOR
			{
				
				float4 offsetColor1 = tex2D(_TurbulenceTex, i.uvnoise + fmod(_Time.xz*_DistortPower,1));
				float4 offsetColor2 = tex2D(_TurbulenceTex, i.uvnoise + fmod(_Time.yx*_DistortPower,1));
				float4 mask = tex2D(_MaskTex, i.uvMask);
				float2 oldUV = i.uvmain;
				i.uvmain.x += ((offsetColor1.r + offsetColor2.r) - 1) * _PowerX;
				i.uvmain.y += ((offsetColor1.r + offsetColor2.r) - 1) * _PowerY;
				float2 newUV = i.uvmain;
				float2 resUV = lerp(oldUV,newUV,mask.xy);
				resUV.x += fmod(_MainPannerX*_Time.y,1);
				resUV.y += fmod(_MainPannerY*_Time.y,1);
				float4 _MainTex_var = tex2D(_MainTex,resUV);
				float2 maskUV = i.uvMask;
				float4 _MaskTex_var = tex2D(_MaskTex,maskUV);
				float3 emissive = _MainTex_var.rgb *_MaskTex_var.r;//(_MainColor.rgb*_Brightness*pow(_MainTex_var.rgb,_Contrast))*(_MainColor.a*_MainTex_var.a*i.vertexColor.a*_MaskTex_var.r);
				float3 finalColor = emissive;
				float finalAlpha = _MainColor.a*_MainTex_var.a*i.vertexColor.a*_MaskTex_var.r;

    			finalAlpha *= SOFTMASK_GET_MASK(i); 

				return fixed4(finalColor,finalAlpha);
				
			}
			ENDCG
		}
	}
}

