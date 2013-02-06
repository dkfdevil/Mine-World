float4x4 World;
float4x4 View;
float4x4 Projection;
texture myTexture;
bool FogEnabled;
float4 FogColor;
float FogStart;
float FogEnd;

float4x4 WorldInverseTranspose;
float4 AmbientColor;
float3 Direction;
float4 DiffuseColor;

// TODO: add effect parameters here.
sampler2D mySampler = sampler_state {
    Texture = (myTexture);
    MinFilter = Point;
    MagFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float3 Normal : NORMAL;
	

    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float4 Color : COLOR0;
	float Depth : DEPTH;

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

struct VertexShaderInputGui
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutputGui
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
	float4 temppos;

	temppos.x = input.Position.x;
	temppos.y = input.Position.y;
	temppos.z = input.Position.z;
	temppos.w = input.Position.w;

    float4 worldPosition = mul(temppos, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.TexCoord = input.TexCoord;
	output.Depth = output.Position.z;

	float4 normal = mul(input.Normal, WorldInverseTranspose);
	float lightIntensity = dot(input.Normal, Direction);
	output.Color = saturate(DiffuseColor * lightIntensity);

    // TODO: add your vertex shader code here.

    return output;
}

VertexShaderOutputGui VertexShaderFunctionGui(VertexShaderInputGui input)
{
	VertexShaderOutputGui output;

	float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.TexCoord = input.TexCoord;

	return output;
}

float4 PixelShaderFunctionGui(VertexShaderOutputGui input) : COLOR0
{
	float4 output;
	output = tex2D(mySampler, input.TexCoord);
	if(output.a == 0)
	{
		discard;
	}
	return output;
}

float4 PixelShaderFunctionTransparent(VertexShaderOutput input) : COLOR0
{
	// TODO: add your pixel shader code here.
	float4 output;
	output = tex2D(mySampler, input.TexCoord);
	if(output.a > 0)
	{
		discard;
	}

    return output;
}

float4 PixelShaderFunctionTranslucent(VertexShaderOutput input) : COLOR0
{
    // TODO: add your pixel shader code here.
	float4 output;
	float alpha;
	output = tex2D(mySampler, input.TexCoord);
	if(output.a == 1.0 || output.a == 0.0 || input.Depth > FogEnd)
	{
		discard;
	}
	alpha = output.a;
	output = saturate(output * (input.Color + AmbientColor));
	output.a = alpha;

    return output;
}

float4 PixelShaderFunctionOpaque(VertexShaderOutput input) : COLOR0
{
    // TODO: add your pixel shader code here.
	float4 output;
	output = tex2D(mySampler, input.TexCoord);
	if(output.a < 1.0)
	{
		discard;
	}
	output = saturate(output * (input.Color + AmbientColor));

	output=lerp(output,FogColor,saturate((input.Depth - FogStart) / (FogEnd - FogStart)));

    return output;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.
		ZENABLE = TRUE;
        ZWRITEENABLE = TRUE;
        CULLMODE = CCW;

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunctionOpaque();
    }
	pass Pass2
    {
        // TODO: set renderstates here.
		ZENABLE = TRUE;
        ZWRITEENABLE = TRUE;
        CULLMODE = CCW;

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunctionTranslucent();
    }
}

technique Technique2
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunctionGui();
		PixelShader = compile ps_3_0 PixelShaderFunctionGui();
	}
}
