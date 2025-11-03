Shader "Color/FixColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Brightness("Brightness", Range(0,2)) = 1  
        _Saturation("Saturation", Range(0,2)) = 1  
        _Contrast("Contrast", Range(0,2)) = 1  
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed _Brightness;
            fixed _Saturation;
            fixed _Contrast;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target  
            {  
                fixed4 col = tex2D(_MainTex, i.uv);  
                //brigtness亮度
                fixed3 finalColor = col * _Brightness;  
                //saturation调整饱和度
                fixed gray = 0.2125 * col.r + 0.7154 * col.g + 0.0721 * col.b;  
                fixed3 grayColor = fixed3(gray, gray, gray);  
                finalColor = lerp(grayColor, finalColor, _Saturation);  
                //contrast调整对比度
                fixed3 avgColor = fixed3(0.5, 0.5, 0.5);    
                finalColor = lerp(avgColor, finalColor, _Contrast);
                fixed colorAlpha = col.a * 0.7;
                return fixed4(finalColor, colorAlpha );  
            }  
            ENDCG
        }
    }
}
