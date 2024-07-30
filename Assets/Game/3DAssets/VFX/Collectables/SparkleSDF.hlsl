#ifndef STAR_SDF_INCLUDED
#define STAR_SDF_INCLUDED



float sdStar(float2 p, float r, float w)
{

    // BASED UPON CODE:
    // The MIT License
    // Copyright © 2019 Inigo Quilez
    // https://www.shadertoy.com/view/3tSGDy


    p.x = max(abs(p.x), 0.0001);
    p.y = max(abs(p.y), 0.0001);

    float m = 4 + w*(2.0-4);
    float an = 3.1415927/4;
    float en = 3.1415927/m;
    float2  racs = r*float2(cos(an),sin(an));
    float2   ecs =   float2(cos(en),sin(en)); // ecs=vec2(0,1) and simplify, for regular polygon,
    
    // reduce to first sector
    float bn = atan(p.x/p.y) % (2.0*an) - an;
    p = length(p)*float2(cos(bn),abs(sin(bn)));

    // line sdf
    p -= racs;
    p += ecs*clamp( -dot(p,ecs), 0.0, racs.y/ecs.y);
    return length(p)*sign(p.x);
}

void StarFX_float(float2 Coords, float Radius, float Angle, out float Factor) 
{
    Factor = sdStar(Coords,Radius,Angle);
}

#endif