#ifndef CUT_DISC_SDF_INCLUDED
#define CUT_DISC_SDF_INCLUDED



float sdCutDisk(float2 p,float r,float h )
{
    // ADAPTED FROM:
    // The MIT License
    // Copyright © 2019 Inigo Quilez
    // https://iquilezles.org/articles/distfunctions2d/

    float w = sqrt(r*r-h*h); // constant for any given shape
    p.x = abs(p.x);
    float s = max( (h-r)*p.x*p.x+w*w*(h+r-2.0*p.y), h*p.x-w*p.y );
    return (s<0.0) ? length(p)-r :
           (p.x<w) ? h - p.y     :
                     length(p-float2(w,h));
}


void generateBlobMattes_float( in float2 p, in float r, in float h, in float Feather, in float Edge, in float Width, out float3 mattes )
{
    float sdf = sdCutDisk(p, r, h);

    float oL = saturate(Edge + Width - Feather);
    float oH = saturate(Edge + Width + Feather);
    float iL = saturate(Edge - Width - Feather);
    float iH = saturate(Edge - Width + Feather);

    float outerMask = smoothstep(oH, oL, sdf);
    float innerMask = smoothstep(iH, iL, sdf);

    // outerMask = saturate(innerMask - outerMask);
    mattes = float3(outerMask, innerMask, sdf);
}

#endif