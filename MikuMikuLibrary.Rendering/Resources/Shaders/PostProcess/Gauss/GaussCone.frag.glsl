#version 330

layout ( location = 0 ) out vec4 oColor;

in vec2 fTexCoord0;
in vec2 fTexCoord1;
in vec2 fTexCoord2;
in vec2 fTexCoord3;

uniform sampler2D uTexture;

void main()
{
    vec4 c0 = texture( uTexture, fTexCoord0 );
    vec4 c1 = texture( uTexture, fTexCoord1 );
    vec4 c2 = texture( uTexture, fTexCoord2 );
    vec4 c3 = texture( uTexture, fTexCoord3 );

    oColor = ( c0 + c1 + c2 + c3 ) / 4;
}
