#version 330

in vec2 fTexCoord;

uniform vec2 uResolution;
uniform vec2 uDirection;

uniform sampler2D uTexture;

out vec4 oColor;

void main()
{
    // TODO
    oColor = texture( uTexture, fTexCoord );
}