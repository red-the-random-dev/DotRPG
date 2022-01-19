sampler s0;
texture level;
sampler s1 = sampler_state
{
    Texture = <level>;
    MagFilter = LINEAR; 
	MinFilter = LINEAR; 
	MipFilter = LINEAR; 
	AddressU = WRAP; 
	AddressV = WRAP;
};
float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0
{
    float4 color = tex2D(s0, coords);
    float4 light = tex2D(s1, coords);
    return color * light;
}

technique LightFinal
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}