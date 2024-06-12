#ifndef LIGHTING_CEL_SHADED_INCLUDED
#define LIGHTING_CEL_SHADED_INCLUDED

/*
Largely following Robin Seibold's work: https://youtu.be/gw31oF9qITw?si=PSya53Mqmfvpd-uw
https://blog.unity.com/engine-platform/custom-lighting-in-shader-graph-expanding-your-graphs-in-2019

Using functions/structs from:
https://github.com/Unity-Technologies/Graphics/blob/master/Packages/com.unity.render-pipelines.universal/ShaderLibrary/RealtimeLights.hlsl
https://github.com/Unity-Technologies/Graphics/blob/master/Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl

Unity Open Project 
https://github.com/UnityTechnologies/open-project-1/blob/devlogs/1-toon-shading/UOP1_Project/Assets/Shaders/CustomHLSL/CustomLighting.hlsl
*/

// ******* //
// Structs //
// ******* //

struct SurfaceInfo {
    float3 normal;
    float3 view;
    float ndotv;
};

struct InputParams {
    float fresnelPow;
    float rimPos;
    float rimFeather;
    float diffPos;
    float diffWidth;
    float diffFeather;
};

struct LightComponents {
    float diffHigh;
    float diffLow;
    float fresnel;
    float3 diffuse;
};

// **************** //
// Helper Functions //
// **************** //

// URP Library functions do not work with shadergraph preview, so we short circuit
#ifndef SHADERGRAPH_PREVIEW

// Modified from Unity Open Project
// Retrieves Light struct for main lights, including correct shadow attenuation
Light FetchMainLight(float3 WorldPos) {    
    /* Some examples used the following, assuming its to support screen-space shadows
    #if SHADOWS_SCREEN
        float4 clipPos = TransformWorldToHClip(Position);
        float4 shadowCoord = ComputeScreenPos(clipPos);
    #else
        float4 shadowCoord = TransformWorldToShadowCoord(clipPos);
    #endif
    */

    float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);

    Light l = GetMainLight(shadowCoord);

	#if !defined(_MAIN_LIGHT_SHADOWS) || defined(_RECEIVE_SHADOWS_OFF)
		l.shadowAttenuation = 1.0h;
	#else
        ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
        float shadowStrength = GetMainLightShadowStrength();
        l.shadowAttenuation = SampleShadowmap(
            shadowCoord,
            TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture),
            shadowSamplingData,
            shadowStrength,
            false);
    #endif

    return l;
}



// Main lighting calculations
LightComponents CalculateCelShading(Light l, SurfaceInfo s, InputParams p) {
    LightComponents c;
    
    float3 attenuation = l.distanceAttenuation * l.shadowAttenuation;
    float lambertian = saturate(dot(s.normal, l.direction));
        


    // Diffuse Light
    float dhp = p.diffPos + p.diffWidth; // TODO: Move this crap outside of the loop
    float dlp = p.diffPos - p.diffWidth;
    float df = p.diffFeather;

    c.diffHigh = attenuation * smoothstep(dhp - df,dhp + df , lambertian);
    c.diffLow = attenuation * smoothstep(dlp - df,dlp + df , lambertian);
    c.diffuse = c.diffLow * l.color;

    // Fresnel
    c.fresnel = pow(s.ndotv * lambertian, p.fresnelPow);
    
    

    return c;   
}

#endif

// ************* //
// Main Function //
// ************* //

void LightingCelShaded_float(float3 Normal, float3 View, float3 WorldPos, float2 RimEdgeParam, float3 DiffuseEdgeParam, float FresnelPow,
  out float Rim, out float3 Diffuse, out float DiffuseHigh, out float DiffuseLow, out float Fresnel) {
    // URP Library functions do not work with shadergraph preview, so we short circuit
    #if defined(SHADERGRAPH_PREVIEW)
    Rim = 1.0;
    Diffuse = float3(0,0,0);
    DiffuseHigh = 1.0;
    DiffuseLow = 1.0;
    Fresnel = 1.0;
    #else

    // Setup surface info struct
    SurfaceInfo s;
    s.normal = normalize(Normal);
    s.view = normalize(View);
    s.ndotv = 1 - dot(s.view, s.normal);

    // Setup params struct
    InputParams p;
    p.fresnelPow = abs(FresnelPow);
    p.rimPos = RimEdgeParam.x;
    p.rimFeather = RimEdgeParam.y * 0.5f;
    p.diffPos = DiffuseEdgeParam.x;
    p.diffFeather = DiffuseEdgeParam.y * 0.5f;
    p.diffWidth = DiffuseEdgeParam.z * 0.5f;
    

    // Compute Main Light
    Light light = FetchMainLight(WorldPos);
    LightComponents c = CalculateCelShading(light, s, p);

    DiffuseHigh = c.diffHigh;
    DiffuseLow = c.diffLow;
    Diffuse = c.diffuse;
    Fresnel = c.fresnel;

    Rim = smoothstep(p.rimPos - p.rimFeather, p.rimPos + p.rimFeather, s.ndotv);
  
    // Compute Additional Lights
    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; i++){
        light = GetAdditionalLight(i, WorldPos, 1); // The third argument is to trigger the correct overload, its normally a shadowmap parameter
        c = CalculateCelShading(light, s, p);
        DiffuseHigh += c.diffHigh;
        DiffuseLow += c.diffLow;
        Fresnel += c.fresnel;
        Diffuse += c.diffuse;
    }   

    DiffuseHigh = saturate(DiffuseHigh);
    DiffuseLow = saturate(DiffuseLow);
    Fresnel = saturate(Fresnel);

    #endif
}
#endif