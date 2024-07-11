// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
///*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:Mobile/Diffuse,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:True,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33588,y:32738,varname:node_3138,prsc:2|emission-1595-OUT,olwid-114-OUT,olcol-9221-RGB;n:type:ShaderForge.SFN_NormalVector,id:6213,x:32372,y:33034,prsc:2,pt:False;n:type:ShaderForge.SFN_Transform,id:7285,x:32558,y:33034,varname:node_7285,prsc:2,tffrom:0,tfto:3|IN-6213-OUT;n:type:ShaderForge.SFN_ComponentMask,id:5866,x:32745,y:33034,varname:node_5866,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-7285-XYZ;n:type:ShaderForge.SFN_RemapRange,id:1920,x:32945,y:33034,varname:node_1920,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-5866-OUT;n:type:ShaderForge.SFN_Tex2d,id:4326,x:33169,y:33057,ptovrint:False,ptlb:MatCap,ptin:_MatCap,varname:node_4326,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:6299da88d04de1043b95dfd8ac5f72e3,ntxv:0,isnm:False|UVIN-1920-OUT;n:type:ShaderForge.SFN_Tex2d,id:3692,x:33098,y:32706,ptovrint:False,ptlb:Texture,ptin:_Texture,varname:node_3692,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:024c1ea47ca6afb4195b1ed866e16134,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1595,x:33324,y:32887,varname:node_1595,prsc:2|A-3692-RGB,B-4326-RGB;n:type:ShaderForge.SFN_ValueProperty,id:114,x:33324,y:33108,ptovrint:False,ptlb:OutlineWidth,ptin:_OutlineWidth,varname:node_114,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_Color,id:9221,x:33291,y:33328,ptovrint:False,ptlb:OutlineColor,ptin:_OutlineColor,varname:node_9221,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;proporder:4326-3692-114-9221;pass:END;sub:END;*/

Shader "UltraRealAssets/ComicEffects/Mobile/Mobile_Toon_Mat_Cap" {
    Properties {
        _MatCap ("MatCap", 2D) = "white" {}
        _Texture ("Texture", 2D) = "white" {}
        _OutlineWidth ("OutlineWidth", Float ) = 0.1
        _OutlineColor ("OutlineColor", Color) = (0.5,0.5,0.5,1)
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "Outline"
            Tags {
            }
            Cull Front
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 2.0
            uniform float _OutlineWidth;
            uniform float4 _OutlineColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos(float4(v.vertex.xyz + v.normal*_OutlineWidth,1) );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                return fixed4(_OutlineColor.rgb,0);
            }
            ENDCG
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 2.0
            uniform sampler2D _MatCap; uniform float4 _MatCap_ST;
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(i.uv0, _Texture));
                float2 node_1920 = (mul( UNITY_MATRIX_V, float4(i.normalDir,0) ).xyz.rgb.rg*0.5+0.5);
                float4 _MatCap_var = tex2D(_MatCap,TRANSFORM_TEX(node_1920, _MatCap));
                float3 emissive = (_Texture_var.rgb*_MatCap_var.rgb);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Mobile/Diffuse"
   // CustomEditor "ShaderForgeMaterialInspector"
}
