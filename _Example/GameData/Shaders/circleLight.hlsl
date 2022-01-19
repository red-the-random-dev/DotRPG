sampler s0;
texture canvas;
sampler fin = sampler_state
{
    Texture = <canvas>;
    MagFilter = LINEAR; 
	MinFilter = LINEAR; 
	MipFilter = LINEAR; 
	AddressU = WRAP; 
	AddressV = WRAP;
};
float2 sourceLocation;
float2 topLeft;
float range;
float distResize;
float4 sourceColor;

float4 CircLight(float2 coords: TEXCOORD0) : COLOR0
{
    float4 color = tex2D(fin, coords);
    float d = distance((coords / distResize)+topLeft, sourceLocation);
    float br = (range - d) / range;
    if (br > 1.0) br = 1.0f;
    if (br < 0.0) br = 0.0f;
    float r = sourceColor.r*br*sourceColor.a;
    if (color.r > r) r = color.r;
    float g = sourceColor.g*br*sourceColor.a;
    if (color.g > g) g = color.g;
    float b = sourceColor.b*br*sourceColor.a;
    if (color.b > b) b = color.b;
    return float4(r, g, b, 1.0f);
}

technique Light
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 CircLight();
    }
}