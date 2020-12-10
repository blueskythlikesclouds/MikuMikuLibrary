#version 330

layout ( location = 0 ) out float oLuma;

in vec2 fTexCoord0;
in vec2 fTexCoord1;
in vec2 fTexCoord2;
in vec2 fTexCoord3;
in vec2 fTexCoord4;
in vec2 fTexCoord5;
in vec2 fTexCoord6;
in vec2 fTexCoord7;

uniform sampler2D uTexture;

void main()
{
    float l0 = texture( uTexture, fTexCoord0 ).w;
    float l1 = texture( uTexture, fTexCoord1 ).w;
    float l2 = texture( uTexture, fTexCoord2 ).w;
    float l3 = texture( uTexture, fTexCoord3 ).w;
    float l4 = texture( uTexture, fTexCoord4 ).w;
    float l5 = texture( uTexture, fTexCoord5 ).w;
    float l6 = texture( uTexture, fTexCoord6 ).w;
    float l7 = texture( uTexture, fTexCoord7 ).w;
    oLuma = ( l0 + l1 + l2 + l3 + l4 + l5 + l6 + l7 ) / 8;
}
