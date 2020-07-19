#version 330

out vec4 oColor;

in vec3 fPosition;
in vec3 fColor;

uniform vec4 uBackColor;

void main()
{
    oColor = vec4( mix( uBackColor.rgb, fColor, clamp( ( 45.0 + fPosition.z ) / 45.0, 0.0, 1.0 ) ), 1.0 );
}