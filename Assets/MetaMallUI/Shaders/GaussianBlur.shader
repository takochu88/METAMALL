Shader "MetaMall/GaussianBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

    TEXTURE2D(_MainTex);
    SAMPLER(sampler_MainTex);
    float4 _MainTex_TexelSize;

    half _Weights[10];
    float2 _Offset;

    struct Attributes
    {
        float4 positionOS : POSITION;
        float2 uv : TEXCOORD0;
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 uv : TEXCOORD0;
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;
        output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
        output.uv = input.uv;
        return output;
    }

    half4 Frag(Varyings input) : SV_Target
    {
        half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * _Weights[0];

        for (int j = 1; j < 10; j++)
        {
            col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + _Offset * j) * _Weights[j];
            col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv - _Offset * j) * _Weights[j];
        }

        return col;
    }
    ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            Name "GAUSSIAN_BLUR"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            ENDHLSL
        }
    }
}
