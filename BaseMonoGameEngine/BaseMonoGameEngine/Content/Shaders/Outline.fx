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

SamplerState s0 { AddressU = Clamp; AddressV = Clamp; };
float2 texelSize;
float4 outlineColor;

float4 Outline(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(s0, input.TextureCoordinates);

	if (color.a == 0)
	{
		float2 coord = input.TextureCoordinates;

		float4 colorUp = tex2D(s0, coord - float2(0, texelSize.y));
		float4 colorDown = tex2D(s0, coord + float2(0, texelSize.y));
		float4 colorLeft = tex2D(s0, coord - float2(texelSize.x, 0));
		float4 colorRight = tex2D(s0, coord + float2(texelSize.x, 0));

		if (colorUp.a != 0 || colorDown.a != 0 || colorLeft.a != 0 || colorRight.a != 0)
		{
			color.rgba = outlineColor;
		}
	}

	return color;
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL Outline();
	}
};