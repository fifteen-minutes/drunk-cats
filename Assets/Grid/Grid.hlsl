#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"


float _GridThickness;
float _CellSize;
float2 _GridXInWorldSpace;
float2 _GridYInWorldSpace;
bool _DebugShowGridSpaceCoordinates;

struct Attributes
{
    float4 positionOS : POSITION;                 
};

struct Interpolators
{
    float4 positionCS : SV_POSITION;
    float3 positionWS : TEXCOORD0;
};            

Interpolators Vert(Attributes input)
{
    Interpolators output;
    VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS);
    output.positionCS = TransformWorldToHClip(positionInputs.positionWS);
    output.positionWS = positionInputs.positionWS;
    return output;
}

half4 Frag(Interpolators input) : SV_Target
{
    float3 positionWS = input.positionWS;
    float gridSpaceDeterminant = _GridXInWorldSpace.x * _GridYInWorldSpace.y - _GridYInWorldSpace.x * _GridXInWorldSpace.y;
    float2x2 gridSpaceMatrixInverse = {
        _GridYInWorldSpace.y, -_GridYInWorldSpace.x,
        -_GridXInWorldSpace.y, _GridXInWorldSpace.x
    };
    gridSpaceMatrixInverse /= gridSpaceDeterminant;
    float2 positionGS = mul(gridSpaceMatrixInverse, positionWS.xy);

    half4 outputColor;
    float xDistanceToGridLineGS = min(frac(positionGS.x), abs(1 - frac(positionGS.x)));
    float yDistanceToGridLineGS = min(frac(positionGS.y), abs(1 - frac(positionGS.y)));
    if (min(xDistanceToGridLineGS, yDistanceToGridLineGS) <= _GridThickness)
    {
        outputColor = half4(1, 1, 1, 1);;
    }
    else if (_DebugShowGridSpaceCoordinates)
    {
        outputColor = float4(positionGS, 0, 1);
    }
    else
    {
        outputColor = half4(0, 0, 0, 0);
    }
    return outputColor;
}