#ifndef CALC_MATTES_INCLUDED
#define CALC_MATTES_INCLUDED

void CalcMattes_float(float In, float Width, float Position, float Feather, out float3 Matte)
{
    float hw = Width * 0.5;
    float hf = Feather * 0.5;

    float tt = saturate(Position + hw + hf);
    float tb = saturate(Position + hw - hf);
    float bt = saturate(Position - hw + hf);
    float bb = saturate(Position - hw - hf);


    Matte.x = smoothstep(tb, tt, In);
    Matte.y = smoothstep(bb, bt, In);
    Matte.z = saturate(Matte.y - Matte.x);
}



#endif