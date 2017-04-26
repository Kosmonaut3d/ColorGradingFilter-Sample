//Trail shader, TheKosmonaut 2017 ( kosmonaut3d@googlemail.com )

////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  VARIABLES
////////////////////////////////////////////////////////////////////////////////////////////////////////////

//Needed for pixel offset
float2 Resolution = float2(1280, 800);

#define NUMBEROFSHIPS 61
float2 Positions[NUMBEROFSHIPS];
float2 LastPositions[NUMBEROFSHIPS];
float3 Colors[NUMBEROFSHIPS];

Texture2D Tex;

////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  STRUCTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////

struct VertexShaderFSQInput
{
	float2 Position : POSITION0;
};

struct VertexShaderFSQOutput
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
}; 

////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  FUNCTIONS
////////////////////////////////////////////////////////////////////////////////////////////////////////////

	////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//  VERTEX SHADER
	////////////////////////////////////////////////////////////////////////////////////////////////////////////

VertexShaderFSQOutput VertexShaderFSQFunction(VertexShaderFSQInput input)
{
	VertexShaderFSQOutput output;

	output.Position = float4(input.Position.xy, 1, 1);
	output.TexCoord = input.Position.xy * 0.5f + 0.5f;
	output.TexCoord.y = 1 - output.TexCoord.y;

	return output;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  PIXEL SHADER
////////////////////////////////////////////////////////////////////////////////////////////////////////////

float distanceSquared(float2 x, float2 y)
{
	return dot(x - y, x - y);
}

float4 PixelShaderTrail(VertexShaderFSQOutput input) : COLOR0
{
	const float size = 7;

	const float size2 = size*size;

	for (int i = NUMBEROFSHIPS - 1; i >= 0; i--)
	{
		//Line between LastPosition and Position
		//get base
		float2 baseVector = Positions[i] - LastPositions[i];
		float2 baseNormal = normalize(float2(-baseVector.y, baseVector.x));
		
		float2 pixelVector = input.Position.xy - LastPositions[i];

		float lengthBaseVector = length(baseVector);
		//convert to base
		float2 resultVector = float2(dot(baseVector/lengthBaseVector, pixelVector) / lengthBaseVector, dot(baseNormal, pixelVector));

		if (resultVector.x > 0  && resultVector.x < 1 && abs(resultVector.y) < size)
			return float4(Colors[i], 0);

		//If not on line, check if we hit the spheres at the end points.

		if (distanceSquared(input.Position.xy, Positions[i]) < size2)
		{
			return float4(Colors[i], 0);
		}

		if (distanceSquared(input.Position.xy, LastPositions[i]) < size2)
		{
			return float4(Colors[i], 0);
		}
	}

	return saturate(Tex.Load(int3(input.Position.xy, 0)) - 1.0f/255.0f);
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  TECHNIQUES
////////////////////////////////////////////////////////////////////////////////////////////////////////////

technique Trails
{
    pass Pass1
    {
		VertexShader = compile vs_4_0 VertexShaderFSQFunction();
        PixelShader = compile ps_4_0 PixelShaderTrail();
    }
}
