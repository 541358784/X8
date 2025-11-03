// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "DisBase"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		_DissolveTex("DissolveTex", 2D) = "white" {}
		_Dissolve("Dissolve", Range( 0 , 1)) = 0
		[HideInInspector]_MinXUV("_MinXUV", Float) = 0
		[HideInInspector]_MaxXUV("_MaxXUV", Float) = 1
		[HideInInspector]_MinYUV("_MinYUV", Float) = 0
		[HideInInspector]_MaxYUV("_MaxYUV", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		LOD 0

		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		
		
		Pass
		{
		CGPROGRAM
			
			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"
			

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				
			};
			
			uniform fixed4 _Color;
			uniform float _EnableExternalAlpha;
			uniform sampler2D _MainTex;
			uniform sampler2D _AlphaTex;
			uniform float4 _MainTex_ST;
			uniform float _Dissolve;
			uniform sampler2D _DissolveTex;
			uniform float _MinXUV;
			uniform float _MaxXUV;
			uniform float _MinYUV;
			uniform float _MaxYUV;

			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				
				IN.vertex.xyz +=  float3(0,0,0);
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
				// get the color from an external texture (usecase: Alpha support for ETC1 on android)
				fixed4 alpha = tex2D (_AlphaTex, uv);
				color.a = lerp (color.a, alpha.r, _EnableExternalAlpha);
#endif //ETC1_EXTERNAL_ALPHA

				return color;
			}
			
			fixed4 frag(v2f IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float2 uv_MainTex = IN.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
				float2 appendResult62 = (float2(( ( IN.texcoord.xy.x - _MinXUV ) / ( _MaxXUV - _MinXUV ) ) , ( ( IN.texcoord.xy.y - _MinYUV ) / ( _MaxYUV - _MinYUV ) )));
				float temp_output_16_0 = saturate( ( ( tex2D( _DissolveTex, appendResult62 ).r - _Dissolve ) / _Dissolve ) );

				float ifLocalVar64 = temp_output_16_0;
				float stepV=step(0.01,_Dissolve);
				ifLocalVar64=(1-stepV)+ifLocalVar64*stepV;
				// if( _Dissolve == 0.0 )
				// ifLocalVar64 = 1.0;
				// else
				// ifLocalVar64 = (temp_output_16_0+temp_output_16_1);
				float4 appendResult32 = (float4(tex2DNode1.rgb , saturate( ( tex2DNode1.a * ifLocalVar64 ) )));
				
				fixed4 c = appendResult32;
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18800
-1901;-4;1920;1013;2678.533;827.7977;1.652555;True;False
Node;AmplifyShaderEditor.RangedFloatNode;52;-1619.397,176.7877;Inherit;False;Property;_MinYUV;_MinYUV;4;1;[HideInInspector];Create;True;0;0;0;False;0;False;0;0.3803711;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;-1577.574,348.2525;Inherit;False;Property;_MaxYUV;_MaxYUV;5;1;[HideInInspector];Create;True;0;0;0;False;0;False;1;0.4301758;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-1604.562,-14.55774;Inherit;False;Property;_MinXUV;_MinXUV;2;1;[HideInInspector];Create;True;0;0;0;False;0;False;0;0.8920898;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;54;-1904.73,-98.82605;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;51;-1604.563,-93.29148;Inherit;False;Property;_MaxXUV;_MaxXUV;3;1;[HideInInspector];Create;True;0;0;0;False;0;False;1;0.9907227;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;61;-1342.954,367.9692;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;56;-1410.212,-225.0562;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;59;-1394.406,146.0859;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;58;-1383.048,-23.72707;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;57;-1197.697,-183.5121;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;60;-1196.64,149.3016;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;62;-1019.275,-68.78168;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-887.6627,325.5257;Inherit;True;Property;_Dissolve;Dissolve;1;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-892.3007,47.67676;Inherit;True;Property;_DissolveTex;DissolveTex;0;0;Create;True;0;0;0;False;0;False;-1;21c4ec6af8d1ff74da283dc9ea007b1e;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;18;-490.4301,73.93842;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;19;-231.8277,173.7646;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;44;-769.8501,-436.386;Inherit;False;0;0;_MainTex;Shader;False;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;16;-7.546356,174.4803;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-120.202,412.0364;Inherit;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-108.202,505.0364;Inherit;False;Constant;_Float1;Float 1;7;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-410.1989,-344.2927;Inherit;True;Property;_MainTex;MainTex;3;0;Fetch;True;0;0;0;False;0;False;-1;b7daacc056d04e8428e965d4166ca2b7;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ConditionalIfNode;64;213.798,355.0364;Inherit;True;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;422.4804,-63.2018;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;47;630.1387,-41.2767;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;32;817.1129,-140.1032;Inherit;True;COLOR;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;67;1187.63,-145.0722;Float;False;True;-1;2;ASEMaterialInspector;0;6;Dis;0f8ba0101102bb14ebf021ddadce9b49;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;True;3;1;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;True;2;False;-1;False;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;61;0;53;0
WireConnection;61;1;52;0
WireConnection;56;0;54;1
WireConnection;56;1;50;0
WireConnection;59;0;54;2
WireConnection;59;1;52;0
WireConnection;58;0;51;0
WireConnection;58;1;50;0
WireConnection;57;0;56;0
WireConnection;57;1;58;0
WireConnection;60;0;59;0
WireConnection;60;1;61;0
WireConnection;62;0;57;0
WireConnection;62;1;60;0
WireConnection;2;1;62;0
WireConnection;18;0;2;1
WireConnection;18;1;6;0
WireConnection;19;0;18;0
WireConnection;19;1;6;0
WireConnection;16;0;19;0
WireConnection;1;0;44;0
WireConnection;64;0;6;0
WireConnection;64;1;65;0
WireConnection;64;2;16;0
WireConnection;64;3;66;0
WireConnection;64;4;16;0
WireConnection;36;0;1;4
WireConnection;36;1;64;0
WireConnection;47;0;36;0
WireConnection;32;0;1;0
WireConnection;32;3;47;0
WireConnection;67;0;32;0
ASEEND*/
//CHKSM=021A2A7ED5750F8EA1B32B37A5AFA73F5E830364