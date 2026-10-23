Shader "Underwater/OceanVegetation"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            Tags { "LightMode"="Universal2D" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            struct Attributes { float3 positionOS:POSITION; float2 uv:TEXCOORD0; float4 color:COLOR; };
            struct Varyings { float4 positionCS:SV_POSITION; float2 uv:TEXCOORD0; float4 color:COLOR; };
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS=TransformObjectToHClip(input.positionOS);
                output.uv=input.uv; output.color=input.color;
                return output;
            }
            half4 frag(Varyings input):SV_Target
            {
                half4 pixel=SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,input.uv);
                // Middle.png incluye azul opaco entre las plantas. Se oculta SOLO al
                // renderizar; el PNG y su importador originales permanecen intactos.
                half3 background=SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,float2(0.003,0.29)).rgb;
                pixel.a*=smoothstep(0.008,0.035,distance(pixel.rgb,background));
                return pixel*input.color;
            }
            ENDHLSL
        }
    }
}
