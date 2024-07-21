Shader "Custom/IndicatorSpellRing"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Power ("Power", Float) = 1
        _PowerScale ("PowerScale", Float) = 1


        // _Bias ("FresnelBias", Float) = 1;
        // _Scale ("FresnelScale", Float) = 1;
        // _Power ("FresnelPower", Float) = 1;

    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZTest Always
            ZWrite Off

            HLSLPROGRAM
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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Color;
            float _Power;
            float _PowerScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float fresnel()
            {
                return 0;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _Color;

                float2 center = float2(0.5, 0.5);
                float dist = length(center - i.uv);
                col.a *= step(dist, 0.5);
                float fade = pow(dist*2, _Power*_PowerScale);
                col *= fade;

                return col;
            }
            ENDHLSL
        }
    }
}
