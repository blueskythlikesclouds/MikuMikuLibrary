#version 330

layout ( location = 0 ) out float oLuma;

in vec2 fTexCoord;

uniform sampler2D uTexture;

const vec3 CENTER_COEF = vec3( 0.8, 1.0, 1.2 );

void main()
{
    float l = 0.0;
    float s = 0.0;

    l += texture( uTexture, vec2( 0.125, 0.125 ) ).r * CENTER_COEF.x; s += CENTER_COEF.x;
    l += texture( uTexture, vec2( 0.375, 0.125 ) ).r * CENTER_COEF.y; s += CENTER_COEF.y;
    l += texture( uTexture, vec2( 0.625, 0.125 ) ).r * CENTER_COEF.y; s += CENTER_COEF.y;
    l += texture( uTexture, vec2( 0.875, 0.125 ) ).r * CENTER_COEF.x; s += CENTER_COEF.x;
    l += texture( uTexture, vec2( 0.125, 0.375 ) ).r * CENTER_COEF.y; s += CENTER_COEF.y;
    l += texture( uTexture, vec2( 0.375, 0.375 ) ).r * CENTER_COEF.z; s += CENTER_COEF.z;
    l += texture( uTexture, vec2( 0.625, 0.375 ) ).r * CENTER_COEF.z; s += CENTER_COEF.z;
    l += texture( uTexture, vec2( 0.875, 0.375 ) ).r * CENTER_COEF.y; s += CENTER_COEF.y;
    l += texture( uTexture, vec2( 0.125, 0.625 ) ).r * CENTER_COEF.y; s += CENTER_COEF.y;
    l += texture( uTexture, vec2( 0.375, 0.625 ) ).r * CENTER_COEF.z; s += CENTER_COEF.z;
    l += texture( uTexture, vec2( 0.625, 0.625 ) ).r * CENTER_COEF.z; s += CENTER_COEF.z;
    l += texture( uTexture, vec2( 0.875, 0.625 ) ).r * CENTER_COEF.y; s += CENTER_COEF.y;
    l += texture( uTexture, vec2( 0.125, 0.875 ) ).r * CENTER_COEF.x; s += CENTER_COEF.x;
    l += texture( uTexture, vec2( 0.375, 0.875 ) ).r * CENTER_COEF.y; s += CENTER_COEF.y;
    l += texture( uTexture, vec2( 0.625, 0.875 ) ).r * CENTER_COEF.y; s += CENTER_COEF.y;
    l += texture( uTexture, vec2( 0.875, 0.875 ) ).r * CENTER_COEF.x; s += CENTER_COEF.x;

    oLuma = l / s;
}