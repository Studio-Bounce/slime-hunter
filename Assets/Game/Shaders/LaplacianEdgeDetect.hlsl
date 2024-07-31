#ifndef SOBEL_EDGE_DETECTION_INCLUDED
#define SOBEL_EDGE_DETECTION_INCLUDED



float Laplacian(float ks) {
    float acc = 0.0;
    float centerWeight = ks * ks * 4 - 1;

    for (float y = -ks; y <= ks, y++) {
        for (float x = -ks; x <= ks, x++) {
        
        }
    }

}


void TextureEdgeDetect(float2 Scale, Texture2D NormalsTex, SamplerState SS,  out float Matte)
{
    float xV[] = [-Scale.x, 0 Scale.x];
    float yV[] = [-Scale.y, 0, Scale.y];

    float3 samples[] = [0,0,0,0,0,0,0,0,0]

    for (int i = 0; i < 9; i++){
        xI = i / 3;
        yi  = i % 3;
        samples[i] = Tex.Sample(SS, float2(xV[xI], yV[yI]))
    }

}

#endif