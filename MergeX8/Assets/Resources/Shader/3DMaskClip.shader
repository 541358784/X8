Shader "Custom/DynamicClipZone" {
    Properties {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1) 
        _ClipCenter ("Clip Center", Float) = 0
        _ClipWidth ("Clip Width", Float) = 1
    }
    SubShader {
        Tags { 
//            "RenderType"="Transparent" 
//            "Queue"="Transparent"
                  "RenderType"="Opaque"  // 改为不透明
        "Queue"="Geometry"     // 使用默认几何队列
        }
        LOD 100

        Pass {
            //Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On 
            //ZWrite Off                     
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color; 
            float _ClipCenter;
            float _ClipWidth;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float distToCenter = abs(i.worldPos.x - _ClipCenter);
                
                clip(distToCenter - _ClipWidth/2);
                
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                return col;
            }
            ENDCG
        }
    }
}