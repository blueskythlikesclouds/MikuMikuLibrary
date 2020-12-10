#version 330

layout ( location = 0 ) out float oLuma;

in vec2 fTexCoord;

uniform sampler2D uTexture;

void main()
{
    float l = 0.0;

    l += texture( uTexture, vec2( 0.03125, 0.25 ) ).r;
    l += texture( uTexture, vec2( 0.09375, 0.25 ) ).r;
    l += texture( uTexture, vec2( 0.15625, 0.25 ) ).r;
    l += texture( uTexture, vec2( 0.21875, 0.25 ) ).r;
    l += texture( uTexture, vec2( 0.28125, 0.25 ) ).r;
    l += texture( uTexture, vec2( 0.34375, 0.25 ) ).r;
    l += texture( uTexture, vec2( 0.40625, 0.25 ) ).r;
    l += texture( uTexture, vec2( 0.46875, 0.25 ) ).r;
    l += texture( uTexture, vec2( 0.53125, 0.25 ) ).r;
    l += texture( uTexture, vec2( 0.59375, 0.25 ) ).r;
    l += texture( uTexture, vec2( 0.65625, 0.25 ) ).r;
    l += texture( uTexture, vec2( 0.71875, 0.25 ) ).r;
    l += texture( uTexture, vec2( 0.78125, 0.25 ) ).r;
    l += texture( uTexture, vec2( 0.84375, 0.25 ) ).r;
    l += texture( uTexture, vec2( 0.90625, 0.25 ) ).r;
    l += texture( uTexture, vec2( 0.96875, 0.25 ) ).r;

    oLuma = l / 16;
}
