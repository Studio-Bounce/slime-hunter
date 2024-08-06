#ifndef LAPLACIAN_EDGE_DETECTION_INCLUDED
#define LAPLACIAN_EDGE_DETECTION_INCLUDED

float LinearEyeDepth( float z ){
    return 1.0 / (_ZBufferParams.z * z + _ZBufferParams.w);
}

void LaplacianEdgeDetect_float(float2 UV, float2 TexelSize, UnityTexture2D viewNormals, SamplerState ss, float3 ViewPos, out float4 Matte)
{
    float hks = 2.0;
    float ks = hks * 2 + 1;

    float3 filteredNormals = float3(0.0, 0.0,0.0);
    float filteredDepth = 0.0;
    float centerWeight = 0.0;

    float2 PixelUVs;

    float x = 0;
    float y = 0;

    float hksSQ = hks * hks;
    float maxLoops = ks * ks;

    float3 sN;
    float sD;
    float mult = -1;
    
    [unroll(1024)]
    for (float i = 0; i < maxLoops; i++) {
        x = floor(i % ks) - hks;
        y = floor(i / ks) - hks;

        if(x*x+ y*y > hksSQ){
            continue;
        }

        centerWeight++;

        PixelUVs = UV + TexelSize * float2(x,y);
        filteredNormals -= viewNormals.tex.SampleLevel(ss, PixelUVs, 0).rgb;
        filteredDepth -= LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(PixelUVs));
    }

    float4 nsam = viewNormals.tex.SampleLevel(ss, PixelUVs, 0);
    filteredNormals += nsam.rgb * centerWeight;
    filteredDepth += LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(PixelUVs)) * centerWeight;

    centerWeight--;    
    centerWeight = 1.0 / centerWeight;

    filteredDepth *= centerWeight;
    filteredNormals *= centerWeight;

    float nDotV = dot(nsam.rgb * 2.0 - 1.0, float3(0.0, 0.0,1.0));

    Matte = float4(max(max(filteredNormals.x,filteredNormals.y),filteredNormals.z), filteredDepth, nsam.a, nDotV);
}

#endif