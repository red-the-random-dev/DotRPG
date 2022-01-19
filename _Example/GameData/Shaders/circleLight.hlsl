sampler s0;
float2 source = float2(0, 0);
float2 topLeft = float2(0, 0);
float range = 64.0f;
float distResize = 1.0f;
float4 source_c = float4(1, 1, 1, 1);

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    return float4(1, 0, 0, 1);
    float4 color = tex2D(s0, coords);
    float2 a = (coords / distResize) + topLeft;
    float d = distance(a, source);
    float br = (range - d) / range;
    if (br > 1.0) br = 1.0f;
    if (br < 0.0) br = 0.0f;
    float r = source_c.r*br* source_c.a;
    if (color.r > r) r = color.r;
    float g = source_c.g*br* source_c.a;
    if (color.g > g) g = color.g;
    float b = source_c.b*br* source_c.a;
    if (color.b > b) b = color.b;
    return float4(r, g, b, 1.0f);
}

technique Light
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}