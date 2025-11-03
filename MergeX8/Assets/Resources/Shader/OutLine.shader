Shader "Custom/OutlinedLine"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,1,1,1)
//        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
//        _OutlineWidth ("Outline Width", float) = 0.1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Opaque" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            fixed4 _MainColor;
            // fixed4 _OutlineColor;
            // float _OutlineWidth;

            v2f vert (appdata_t v)
            {
                v2f o;
                // 将顶点位置传递到片段着色器
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = _MainColor;

                // // 计算当前片段距离像素中心的距离
                // float dist = length(i.pos.xy - float2(0.5, 0.5) * _ScreenParams.xy);
                //
                // // 如果距离小于_OutlineWidth，则选择_OutlineColor
                // if (dist > (0.5 - _OutlineWidth))
                // {
                //     color = _OutlineColor;
                // }

                return color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
