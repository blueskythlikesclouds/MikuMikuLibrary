#version 330

uniform sampler2D uTexture;

in vec2 fTexCoord;

out vec4 oColor;

void main()
{
    oColor = texture( uTexture, fTexCoord );
}