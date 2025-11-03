Shader "Custom/Hole"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Fade("fade", float) = 20
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 100

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma multi_compile __ CIRCLE RECT
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 lp : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            float4 _Centers[4];
            float _Rs[4];
            int _Count;
            float _Fade;

            float Hole(float2 pos, float r)
            {
                return length(pos) - r;
            }

            float rectangle(float2 pos, float2 hs)
            {
                float2 d = abs(pos) - hs;
                float od = length(max(d, 0));
                float idst = min(max(d.x, d.y), 0);
                return od + idst;
            }


            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.lp = v.vertex;

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                 #ifdef CIRCLE
                 for (int k = 0; k < _Count; ++k)
                 {
                     float d = Hole(i.lp - _Centers[k].xy, _Rs[k]);
                     float a = smoothstep(-_Fade, _Fade, d);
                     col.a *= a;
                 }
                #elif RECT
                for (int k = 0; k < _Count; ++k)
                {
                    float4 center = _Centers[k];
                    float d = rectangle(i.lp - center.xy, center.zw);                 
                    float a = smoothstep(-_Fade, _Fade, d);
                    col.a *= a;
                }                
                #endif

                return col;
            }
            ENDCG
        }
    }
}