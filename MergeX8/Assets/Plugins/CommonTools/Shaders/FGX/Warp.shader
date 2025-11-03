// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:33750,y:32746,varname:node_4013,prsc:2|alpha-1379-OUT,refract-5891-OUT;n:type:ShaderForge.SFN_Tex2d,id:2808,x:32078,y:32663,ptovrint:False,ptlb:node_2808,ptin:_node_2808,varname:node_2808,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:227f30b4ce437fb42919a6778dce8100,ntxv:0,isnm:False|UVIN-7906-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:3630,x:32078,y:32912,ptovrint:False,ptlb:node_3630,ptin:_node_3630,varname:node_3630,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:0afb609ae59a6994c8a91429c785a903,ntxv:0,isnm:False|UVIN-7052-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:2757,x:32839,y:32789,ptovrint:False,ptlb:node_2757,ptin:_node_2757,varname:node_2757,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e60087f7f9b4cf247819ed63033fe604,ntxv:0,isnm:False|UVIN-80-OUT;n:type:ShaderForge.SFN_Panner,id:7906,x:31885,y:32663,varname:node_7906,prsc:2,spu:1,spv:1|UVIN-1549-UVOUT;n:type:ShaderForge.SFN_Rotator,id:1549,x:31683,y:32663,varname:node_1549,prsc:2|UVIN-5016-UVOUT,SPD-2958-OUT;n:type:ShaderForge.SFN_TexCoord,id:5016,x:31454,y:32663,varname:node_5016,prsc:2,uv:0;n:type:ShaderForge.SFN_Vector1,id:2958,x:31498,y:32828,varname:node_2958,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Panner,id:7052,x:31901,y:32912,varname:node_7052,prsc:2,spu:1,spv:1|UVIN-6284-UVOUT;n:type:ShaderForge.SFN_Rotator,id:6284,x:31709,y:32912,varname:node_6284,prsc:2|UVIN-6113-UVOUT,SPD-2717-OUT;n:type:ShaderForge.SFN_TexCoord,id:6113,x:31443,y:32914,varname:node_6113,prsc:2,uv:0;n:type:ShaderForge.SFN_Vector1,id:2717,x:31443,y:33086,varname:node_2717,prsc:2,v1:-0.3;n:type:ShaderForge.SFN_Multiply,id:1521,x:32262,y:32789,varname:node_1521,prsc:2|A-2808-B,B-3630-B;n:type:ShaderForge.SFN_Add,id:80,x:32476,y:32789,varname:node_80,prsc:2|A-1521-OUT,B-9061-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:4164,x:31989,y:33103,varname:node_4164,prsc:2,uv:0;n:type:ShaderForge.SFN_Rotator,id:9061,x:32313,y:32977,varname:node_9061,prsc:2|UVIN-4164-UVOUT,SPD-7011-OUT;n:type:ShaderForge.SFN_Vector1,id:7011,x:32132,y:33147,varname:node_7011,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Tex2d,id:1954,x:32839,y:33022,ptovrint:False,ptlb:node_1954,ptin:_node_1954,varname:node_1954,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:20c1dbb8509b32045b7a238433915690,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2491,x:33076,y:32887,varname:node_2491,prsc:2|A-2757-G,B-1954-B;n:type:ShaderForge.SFN_Multiply,id:5891,x:33375,y:33008,varname:node_5891,prsc:2|A-2491-OUT,B-3420-OUT,C-3503-OUT,D-3436-B;n:type:ShaderForge.SFN_Vector2,id:3420,x:33076,y:33038,varname:node_3420,prsc:2,v1:0.25,v2:0.25;n:type:ShaderForge.SFN_ValueProperty,id:3503,x:33076,y:33198,ptovrint:False,ptlb:node_3503,ptin:_node_3503,varname:node_3503,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_VertexColor,id:3436,x:33076,y:33308,varname:node_3436,prsc:2;n:type:ShaderForge.SFN_Vector1,id:1379,x:33484,y:32916,varname:node_1379,prsc:2,v1:0;proporder:2808-3630-2757-1954-3503;pass:END;sub:END;*/

Shader "GFX/Distortion" {
    Properties {
        _node_2808 ("node_2808", 2D) = "white" {}
        _node_3630 ("node_3630", 2D) = "white" {}
        _node_2757 ("node_2757", 2D) = "white" {}
        _node_1954 ("node_1954", 2D) = "white" {}
        _node_3503 ("node_3503", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform float4 _TimeEditor;
            uniform sampler2D _node_2808; uniform float4 _node_2808_ST;
            uniform sampler2D _node_3630; uniform float4 _node_3630_ST;
            uniform sampler2D _node_2757; uniform float4 _node_2757_ST;
            uniform sampler2D _node_1954; uniform float4 _node_1954_ST;
            uniform float _node_3503;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float4 node_8557 = _Time + _TimeEditor;
                float node_1549_ang = node_8557.g;
                float node_1549_spd = 0.3;
                float node_1549_cos = cos(node_1549_spd*node_1549_ang);
                float node_1549_sin = sin(node_1549_spd*node_1549_ang);
                float2 node_1549_piv = float2(0.5,0.5);
                float2 node_1549 = (mul(i.uv0-node_1549_piv,float2x2( node_1549_cos, -node_1549_sin, node_1549_sin, node_1549_cos))+node_1549_piv);
                float2 node_7906 = (node_1549+node_8557.g*float2(1,1));
                float4 _node_2808_var = tex2D(_node_2808,TRANSFORM_TEX(node_7906, _node_2808));
                float node_6284_ang = node_8557.g;
                float node_6284_spd = (-0.3);
                float node_6284_cos = cos(node_6284_spd*node_6284_ang);
                float node_6284_sin = sin(node_6284_spd*node_6284_ang);
                float2 node_6284_piv = float2(0.5,0.5);
                float2 node_6284 = (mul(i.uv0-node_6284_piv,float2x2( node_6284_cos, -node_6284_sin, node_6284_sin, node_6284_cos))+node_6284_piv);
                float2 node_7052 = (node_6284+node_8557.g*float2(1,1));
                float4 _node_3630_var = tex2D(_node_3630,TRANSFORM_TEX(node_7052, _node_3630));
                float node_9061_ang = node_8557.g;
                float node_9061_spd = 0.5;
                float node_9061_cos = cos(node_9061_spd*node_9061_ang);
                float node_9061_sin = sin(node_9061_spd*node_9061_ang);
                float2 node_9061_piv = float2(0.5,0.5);
                float2 node_9061 = (mul(i.uv0-node_9061_piv,float2x2( node_9061_cos, -node_9061_sin, node_9061_sin, node_9061_cos))+node_9061_piv);
                float2 node_80 = ((_node_2808_var.b*_node_3630_var.b)+node_9061);
                float4 _node_2757_var = tex2D(_node_2757,TRANSFORM_TEX(node_80, _node_2757));
                float4 _node_1954_var = tex2D(_node_1954,TRANSFORM_TEX(i.uv0, _node_1954));
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + ((_node_2757_var.g*_node_1954_var.b)*float2(0.25,0.25)*_node_3503*i.vertexColor.b);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
                float3 finalColor = 0;
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,0.0),1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
