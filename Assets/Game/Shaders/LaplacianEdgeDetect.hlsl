#ifndef LAPLACIAN_EDGE_DETECTION_INCLUDED
#define LAPLACIAN_EDGE_DETECTION_INCLUDED


void LaplacianEdgeDetect_float(float2 UV, float2 TexelSize, float KernelSize, UnityTexture2D viewNormals, SamplerState ss, out float4 Matte)
{
    float ks = 5.0;

    float3 filteredNormals = float3(0.0, 0.0,0.0);
    float filteredDepth = 0.0;
    float centerWeight = ks * ks - 1;

    float2 PixelUVs;

    float x = 0;
    float y = 0;

    float ksf = floor(ks);
    float hks = floor(ks/2);
    float maxLoops = ksf * ksf;

    
    [unroll(1024)]
    for (float i = 0; i < maxLoops; i++) {
        x = floor(i % ks - hks);
        y = floor(floor(i / ks) - hks);

        if (x != 0.0 || y != 0.0){
                PixelUVs = UV + TexelSize * float2(x,y);


                // filteredNormals -= SAMPLE_TEXTURE2D(viewNormals.tex,viewNormals.samplerstate, PixelUVs).rgb;
                filteredNormals -= viewNormals.tex.SampleLevel(ss, PixelUVs, 0).rgb;
                filteredDepth -= SAMPLE_DEPTH_TEXTURE(ss,PixelUVs, 0);
            } else {
                filteredNormals += viewNormals.tex.SampleLevel(ss, UV, 0).rgb * centerWeight;
                filteredDepth += SAMPLE_DEPTH_TEXTURE(ss,UV, 0) * centerWeight;
            }
    }

    /*
    for ( y = -HalfKernelSize; y <= HalfKernelSize; y++) {
        for ( x = -HalfKernelSize; x <= HalfKernelSize; x++) {
            if (x != 0.0 || y != 0.0){
                PixelUVs = UV + TexelSize * float2(x,y);

                filteredNormals -= SAMPLE_TEXTURE2D(viewNormals.tex,viewNormals.samplerstate, PixelUVs).rgb;
                filteredDepth -= SHADERGRAPH_SAMPLE_SCENE_DEPTH(PixelUVs);
            } else {
                filteredNormals += SAMPLE_TEXTURE2D(viewNormals.tex,viewNormals.samplerstate, UV).rgb * centerWeight;
                filteredDepth += SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV);
            }
        }
    }
    */


    filteredDepth /= centerWeight;
    filteredNormals /= centerWeight;
    Matte = float4(filteredNormals, filteredDepth);

}

#endif