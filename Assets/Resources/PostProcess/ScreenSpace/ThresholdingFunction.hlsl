#ifndef EDGE_DETECTION_THRESHOLDING_INCLUDED
#define EDGE_DETECTION_THRESHOLDING_INCLUDED

void EdgeDetectThresholds_float(float4 InMatte, float DepthThreshold, float NormalThreshold, float Feather, out float2 OutMatte)
{
    float nDotV = saturate(InMatte.a);
    float saf = saturate(nDotV * 3 - 0.5);
    
    float dM = smoothstep(0, 1, InMatte.g);
    dM = saf * dM;

    float nH = min(NormalThreshold + Feather, 1.0);
    float nL = max(NormalThreshold - Feather, 0.0);
    float dH = min(DepthThreshold + Feather, 1.0);
    float dL = max(DepthThreshold - Feather, 0.0);


    float nC = smoothstep(nL, nH, InMatte.r);
    float dC = smoothstep(dL, dH, dM);

    OutMatte = float2(nC, dC);
}

#endif