
#ifndef SPRITE_EDGE_HLSLINCLUDE_INCLUDED
#define SPRITE_EDGE_HLSLINCLUDE_INCLUDED

void SpriteEdge_float(
    UnityTexture2D maskTex,
    UnitySamplerState maskSampler,
    float2 uv,
    float2 texelSize,
    float border,
    float bloomIntensity, // Intensidad del bloom
    float bloomThreshold, // Umbral para activar el bloom
    out float edge)
{
    float2 t = texelSize * border;

    float a0 = maskTex.Sample(maskSampler, uv).a;

    float aR = maskTex.Sample(maskSampler, uv + float2(t.x, 0)).a;
    float aL = maskTex.Sample(maskSampler, uv + float2(-t.x, 0)).a;
    float aU = maskTex.Sample(maskSampler, uv + float2(0, t.y)).a;
    float aD = maskTex.Sample(maskSampler, uv + float2(0, -t.y)).a;
    
    float inside = a0 > 0 ? 1 : 0;
    float outsideAround = (aR <= 0 || aL <= 0 || aU <= 0 || aD <= 0) ? 1 : 0;
    
    float baseEdge = inside * outsideAround;
    
    // Bloom solo se aplica a píxeles que superen el umbral
    float bloom = a0 > bloomThreshold ? a0 * bloomIntensity : 0;
    
    edge = saturate(baseEdge + bloom);
}



void SpriteEdgeGlow_float(
    UnityTexture2D maskTex,
    UnitySamplerState maskSampler,
    float2 uv,
    float2 texelSize,
    float border,
    float glow,
    out float edge)
{
    float2 t = texelSize * border;

    float a0 = maskTex.Sample(maskSampler, uv).a;

    float aR = maskTex.Sample(maskSampler, uv + float2(t.x, 0)).a;
    float aL = maskTex.Sample(maskSampler, uv + float2(-t.x, 0)).a;
    float aU = maskTex.Sample(maskSampler, uv + float2(0, t.y)).a;
    float aD = maskTex.Sample(maskSampler, uv + float2(0, -t.y)).a;
    
    float inside = a0 > 0 ? 1 : 0;
    float outsideAround = (aR <= 0 || aL <= 0 || aU <= 0 || aD <= 0) ? 1 : 0;
    float baseEdge = inside * outsideAround;
    
    // Efecto glow optimizado
    if (glow > 0)
    {
        // Muestreo en forma de cruz para mejor rendimiento
        float glowSample = 0;
        
        // Radio del glow
        int samples = int(glow * 4.0);
        
        for (int i = 1; i <= samples; i++)
        {
            float sampleDist = float(i);
            float falloff = 1.0 - (sampleDist / float(samples));
            
            // Muestrear en las 4 direcciones principales
            float2 offsets[4] =
            {
                float2(sampleDist, 0),
                float2(-sampleDist, 0),
                float2(0, sampleDist),
                float2(0, -sampleDist)
            };
            
            for (int j = 0; j < 4; j++)
            {
                float sampleAlpha = maskTex.Sample(maskSampler, uv + offsets[j] * texelSize).a;
                glowSample += sampleAlpha * falloff * glow;
            }
        }
        
        // Añadir el efecto glow al borde
        edge = saturate(baseEdge + glowSample / float(samples * 4));
    }
    else
    {
        edge = baseEdge;
    }
}

#endif 