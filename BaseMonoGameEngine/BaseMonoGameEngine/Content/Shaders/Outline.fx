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

	if (currentPixel.a == 0.0f)
	{
		float2 uvPix = float2(1 / sheetSize.x, 1 / sheetSize.y);

		float4 colorRight = tex2D(s0, input.TextureCoordinates + float2(uvPix.x, 0));
		float4 colorDown = tex2D(s0, input.TextureCoordinates + float2(0, uvPix.y));
		float4 colorUp = tex2D(s0, input.TextureCoordinates + float2(0, -uvPix.y));
		float4 colorLeft = tex2D(s0, input.TextureCoordinates + float2(-uvPix.x, 0));

		if (colorRight.a > 0 || colorDown.a > 0 || colorLeft.a > 0 || colorUp.a > 0)
		{
			output = outlineColor;
		}
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