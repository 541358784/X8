// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/UnlitSpineLight"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        _MainTex("Main Tex", 2D) = "white"{}
        _Alpha("Alpha", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent"
        }

        Pass //增加一个Pass，开启深度写入，但不输出颜色，目的仅仅是为了把该模型的深度值写入深度缓冲中
        {
            ZWrite On
            ColorMask 0 //ColorMask用于设置颜色通道的写掩码（write mask），如果设为0，则该Pass不写入任何颜色通道，即不会输出任何颜色。
        }

        Pass
        {
            Tags
            {
                "LightMode" = "ForwardBase"
            }

            ZWrite Off //关闭深度写入，透明度混合中都应关闭深度写入
            Blend SrcAlpha OneMinusSrcAlpha //设置该Pass的混合模式，我们将源颜色（该片元着色器产生的颜色）的混合因子设为SrcAlpha，把目标颜色（已经存在于颜色缓冲中的颜色）的混合因子设为OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _Alpha;

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            v2f vert(a2v v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);

                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

                fixed4 texColor = tex2D(_MainTex, i.uv);

                fixed3 albedo = texColor.rgb * _Color.rgb;

                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

                fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(worldNormal, worldLightDir));

                return fixed4(ambient + diffuse, texColor.a * _Alpha); //返回需要设置透明通道值，只有使用Blend命令打开混合后，这里的设置才有意义，否则这些透明度并不会对片元的透明效果有任何影响
            }
            ENDCG
        }
    }
}