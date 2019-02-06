#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_5_0
	#define PS_SHADERMODEL ps_5_0
#endif

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

SamplerState s0;
float2 sheetSize;
float4 outlineColor;

float4 Outline(VertexShaderOutput input) : COLOR
{
	float4 currentPixel = tex2D(s0, input.TextureCoordinates) * input.Color;
	float4 output = currentPixel;
	float alpha = output.a;

	float2 uvPix = float2(1 / sheetSize.x, 1 / sheetSize.y);

	//Sample colors from all four directions around this pixel
	float4 colorRight = tex2D(s0, input.TextureCoordinates + float2(uvPix.x, 0));
	float4 colorDown = tex2D(s0, input.TextureCoordinates + float2(0, uvPix.y));
	float4 colorUp = tex2D(s0, input.TextureCoordinates + float2(0, -uvPix.y));
	float4 colorLeft = tex2D(s0, input.TextureCoordinates + float2(-uvPix.x, 0));

	//Get the max alpha value for all surrounding pixels
	alpha = max(alpha, colorRight.a);
	alpha = max(alpha, colorDown.a);
	alpha = max(alpha, colorUp.a);
	alpha = max(alpha, colorLeft.a);

	output.a = alpha;

	//If the alpha in any direction is found to be greater than the current pixel's alpha,
	//then we're on the edge, so set the color
	if (alpha > currentPixel.a)
	{
		output = outlineColor;
	}

	return output;
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL Outline();
	}
};