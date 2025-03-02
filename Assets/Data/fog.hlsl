#ifndef BOXINTERSECTION_
#define BOXINTERSECTION_

float2 rayBoxDst(float3 boundsMin, float3 boundsMax, float3 rayOrigin, float3 rayDir)
{
    float3 t0 = (boundsMin - rayOrigin) / rayDir;
    float3 t1 = (boundsMax - rayOrigin) / rayDir;
    float3 tmin = min(t0, t1);
    float3 tmax = max(t0, t1);

    float dstA = max(max(tmin.x, tmin.y), tmin.z);
    float dstB = min(tmax.x, min(tmax.y, tmax.z));

    //case 1: ray intersects box from outside(0 <= dstA <= dstB)
    // dstA is dst to nearest intersection, dstB dst to far intersection

    // Case 2: ray intersects box from instide (dstA < 0 < dstB)
    // dstA is the dst to intersection behind the ray, dstB is dst to forward intersection

    // Case 3: ray missed box (distA > dstB)

    float dstToBox = max(0, dstA);
    float dstInsideBox = max(0, dstB - dstToBox);

    float2 result = float2(dstToBox, dstInsideBox);
    return result;
}

void falloff_float(float value, float power, float multiplier, out float result)
{
    result = 1 + (1 / (-(multiplier * pow(value, power) + 1)));
}


void fogtransmittance_float(Texture2D<float4> weatherMap, SamplerState weatherSample, float3 boundsMin, float3 boundsMax, float3 rayOrigin, float3 rayDir, float numSteps,
float fogDensity, float weatherFogIntensity, float weatherDensityFalloff, float weatherDensityMaxHeight, float densityFalloff, float3 atmosphereColor, float3 weatherColor, 
out float alpha, out float3 color)
{
    // box ray intersection
    
    float2 rayBoxIntersection = rayBoxDst(boundsMin, boundsMax, rayOrigin, rayDir);
    float dstToBox = rayBoxIntersection.x;
    float dstInsideBox = rayBoxIntersection.y;
    
    // point of intersection with the container
    float3 entryPoint = rayOrigin + rayDir * dstToBox;
    
    // ray march through box
    float dstTravelled = 0;
    float stepSize = dstInsideBox / numSteps;
    float atmosphereTransmittance = 0;
    float weatherTransmittance = 0;
    
    while (dstTravelled < dstInsideBox)
    {
        float3 rayPos = entryPoint + rayDir * dstTravelled;
        
        //calculate atmospheric density
        float height01 = (rayPos.y) / boundsMax.y;
        float atmosphereDensity = exp(-height01 * densityFalloff) * (1 - height01);
       
        //calculate uvs for weather map
        float3 rayPosMap = -boundsMin + rayPos;
        float3 boundsMap = boundsMax - boundsMin;
        float2 uv = float2((boundsMap.x - rayPosMap.x) / boundsMap.x, (boundsMap.z - rayPosMap.z) / boundsMap.z);
        
        // sample weather 
        float weatherAmount = weatherMap.SampleLevel(weatherSample, uv, 0) * weatherFogIntensity;
        float weatherHeight = clamp((rayPos.y / weatherDensityMaxHeight), 0, 1);
        float weatherAtmosphericDensity = exp(-weatherHeight * weatherDensityFalloff) * (1 - weatherHeight);
        
        atmosphereTransmittance += atmosphereDensity * stepSize * fogDensity;
        weatherTransmittance += stepSize * weatherAmount * weatherAtmosphericDensity;
        dstTravelled += stepSize;
    }
    
    
    alpha = atmosphereTransmittance + weatherTransmittance;
    color = lerp(weatherColor, atmosphereColor, weatherTransmittance / (atmosphereTransmittance + weatherTransmittance + 0.001));
    
}

#endif
