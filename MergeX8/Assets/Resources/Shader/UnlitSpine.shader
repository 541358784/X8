Shader "Unlit/UnlitSpine"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Scale ("scale", Float) = 1
        _Alpha ("alpha", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent"
               "Queue" = "Transparent"
               "IgnoreProjector"="True"
             }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Scale;
            float _Alpha;

            v2f vert (appdata v)
            {
                v2f o;
                // float3 viewDir = mul((float3x3)unity_CameraToWorld, float3(0,0,1));
                // float4 viewModel = normalize(mul(unity_WorldToObject,float4(viewDir,0)));
                // float3 viewScale=viewModel.xyz*dot(viewModel.xyz,v.vertex.xyz);
                // float3 other=v.vertex.xyz-viewScale;
                // float3 final=viewScale*_Scale+other;
                o.vertex = UnityObjectToClipPos(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return float4(col.xyz,col.w*_Alpha);
            }
            ENDCG
        }
    }
}