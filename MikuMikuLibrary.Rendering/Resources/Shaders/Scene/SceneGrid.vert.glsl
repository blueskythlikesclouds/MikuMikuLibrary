#version 330

#include "SceneDefinitions.glsl"

layout ( location = 0 ) in vec4 aPosition;

out vec3 fPosition;
out vec3 fColor;

uniform vec3 uInnerColor;
uniform vec3 uOuterColor;
uniform vec3 uXColor;
uniform vec3 uZColor;

void main()
{
    fPosition = ( uView * vec4( aPosition.xyz, 1 ) ).xyz;

    switch ( int( aPosition.w ) )
    {
        case 0:
            fColor = uZColor;
            break;

        case 1:
            fColor = uXColor;
            break;

        case 2:
            fColor = uInnerColor;
            break;

        default:
            fColor = uOuterColor;
            break;
    }

    gl_Position = uProjection * vec4( fPosition, 1.0 );
}