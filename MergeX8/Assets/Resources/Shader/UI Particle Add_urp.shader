Shader "UI Extensions/Particles/Additive_urp"
{
    Properties
    {
        _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
        _MainTex ("Particle Texture", 2D) = "white" {}
        _InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15
    }

    Category
    {
        Tags
        {
            "RenderPipeline" = "UniversalRenderPipeline" 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane" 
            "CanUseSpriteAtlas"="True"
        }
        Blend SrcAlpha One
        ColorMask RGB
        Cull Off Lighting Off ZWrite Off

        SubShader
        {

            Stencil
            {
                Ref [_Stencil]
                Comp [_StencilComp]
                Pass [_StencilOp]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
            }

            Pass
            {

                HLSLPROGRAM
                #pragma prefer_hlslcc gles
			    #pragma exclude_renderers d3d11_9x
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_particles
                #pragma multi_compile_fog

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"                

                TEXTURE2D(_MainTex);
                SAMPLER(sampler_MainTex);
                                
                half4 _TintColor;

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    half4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    half4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                    float fogCoord : TEXCOORD1;
                };

                float4 _MainTex_ST;

                v2f vert(appdata_t IN)
                {
                    v2f v;
                    v.vertex = TransformObjectToHClip(IN.vertex);                                        
                    v.color = IN.color;
                    v.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                    v.fogCoord = ComputeFogFactor(v.vertex.z);                    
                    return v;
                }                
                                               
                half4 frag(v2f IN) : SV_Target
                {                    
                    half4 col = 2.0f * IN.color * _TintColor * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.texcoord);
                    col.rgb = MixFog(col, IN.fogCoord);                    
                    return col;
                }
                ENDHLSL
            }
        }
        
        fallback "UI Extensions/Particles/Additive"
    }
}