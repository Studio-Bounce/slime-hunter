#ifndef EDGE_DETECTION_THRESHOLDING_INCLUDED
#define EDGE_DETECTION_THRESHOLDING_INCLUDED

void EdgeDetectThresholds_float(float DepthMatte, float NormalMatte, float DepthThreshold, 
float NormalThreshold, float Feather, out float2 OutMatte)
{
    float nH = min(NormalThreshold + Feather, 1.0);
    float nL = max(NormalThreshold - Feather, 0.0);
    float dH = min(DepthThreshold + Feather, 1.0);
    float dL = max(DepthThreshold - Feather, 0.0);

    float nC = smoothstep(nL, nH, NormalMatte);
    float dC = smoothstep(dL, dH, DepthMatte);

    OutMatte = float2(nC, dC);
}

#endif