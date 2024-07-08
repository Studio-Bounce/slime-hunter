Shader "Custom/IndicatorRing"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Radius ("Radius", Float) = 0.25
        _Thickness ("Thickness", Float) = 0.05
        _Feathering ("Feathering", Float) = 0.01

        _ThicknessScale ("ThicknessScale", Float) = 1
        _FeatheringScale ("FeatheringScale", Float) = 1

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
            float _Radius;
            float _Thickness;
            float _Feathering;

            float _ThicknessScale;
            float _FeatheringScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float thickness = (_Thickness/4)*_ThicknessScale;
                float feathering = _Feathering*_FeatheringScale;

                float2 center = float2(0.5, 0.5);
                float sdf = length(center - i.uv) - (_Radius-(thickness+feathering));
                sdf = abs(sdf);
                sdf = 1-smoothstep(thickness, thickness+feathering, sdf);
                sdf = sdf*sdf*sdf;

                fixed4 col = _Color;
                col.a *= sdf;

                return col;
            }
            ENDHLSL
        }
    }
}
