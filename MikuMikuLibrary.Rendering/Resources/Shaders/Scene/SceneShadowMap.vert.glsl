#version 330

#include "SceneDefinitions.glsl"

layout ( location = 0 ) in vec3 aPosition;
layout ( location = 3 ) in vec2 aTexCoord0;

out vec2 fTexCoord;

void main()
{
    if ( ( uShaderFlags & SHADER_FLAGS_PUNCH_THROUGH ) != 0 )
        fTexCoord = ( uDiffuseTransformation * vec4( aTexCoord0.xy, 0, 0 ) ).xy;

    gl_Position = uProjection * uView * uModel * vec4( aPosition, 1.0 );
}