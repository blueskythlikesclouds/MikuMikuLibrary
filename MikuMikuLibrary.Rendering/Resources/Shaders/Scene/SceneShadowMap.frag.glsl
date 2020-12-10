#version 330

#include "SceneDefinitions.glsl"

in vec2 fTexCoord;

void main()
{
    if ( ( uShaderFlags & SHADER_FLAGS_PUNCH_THROUGH ) == 0 )
        return;

    float transparency = uDiffuse.a;

    if ( ( uShaderFlags & SHADER_FLAGS_TRANSPARENCY ) != 0 )
        transparency = texture( uTransparencyTexture, fTexCoord ).a;

    else if ( ( uShaderFlags & SHADER_FLAGS_DIFFUSE ) != 0 )
        transparency = texture( uDiffuseTexture, fTexCoord ).a;

    if ( transparency < 0.5 )
        discard;
}