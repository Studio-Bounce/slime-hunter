
#ifndef LAPLACIAN_EDGE_DETECTION_INCLUDED
#define LAPLACIAN_EDGE_DETECTION_INCLUDED

float LinearEyeDepth( float z ){
    return 1.0 / (_ZBufferParams.z * z + _ZBufferParams.w);
}

void LaplacianEdgeDetect_float(float2 UV, float2 TexelSize, float KernelSize, UnityTexture2D viewNormals, SamplerState ss, out float4 Matte)
{
    float ks = 7.0;

    float3 filteredNormals = float3(0.0, 0.0,0.0);
    float filteredDepth = 0.0;
    float centerWeight = ks * ks - 1;

    float2 PixelUVs;

    float x = 0;
    float y = 0;

    float ksf = floor(ks);
    float hks = floor(ks/2);
    float maxLoops = ksf * ksf;

    float3 sN;
    float sD;
    
    [unroll(1024)]
    for (float i = 0; i < maxLoops; i++) {
        x = floor(i % ks - hks);
        y = floor(floor(i / ks) - hks);

        PixelUVs = UV + TexelSize * float2(x,y);
        sN = viewNormals.tex.SampleLevel(ss, PixelUVs, 0).rgb;
        sD = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(PixelUVs));

        if (x != 0.0 || y != 0.0) {
            filteredNormals -= sN;
            filteredDepth -= sD;
        } else {
            filteredNormals += sN * centerWeight;
            filteredDepth += sD * centerWeight;
        }
    }

    filteredDepth /= centerWeight;
    filteredNormals /= centerWeight;
    Matte = float4(filteredNormals, filteredDepth);
}

#endif