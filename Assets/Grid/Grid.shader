Shader "Sprites/Default"
{
    Properties
    {
        [MainTexture] _MainTex ("Main Texture", 2D) = "white" {}
        _GridThickness ("Grid Thickness", Float) = 0.1
        _CellSize ("Cell Size", Float) = 1
        _GridXInWorldSpace ("Grid X In World Space", Vector) = (1, 0, 0, 0)
        _GridYInWorldSpace ("Grid Y In World Space", Vector) = (0, 1, 0, 0)
        [Toggle] _DebugShowGridSpaceCoordinates ("Debug Show Grid Space Coordinates", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
            "RenderPipeline" = "UniversalPipeline"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        HLSLPROGRAM
            #include "Grid.hlsl"
            #pragma vertex Vert
            #pragma fragment Frag
        ENDHLSL
        }
    }
}