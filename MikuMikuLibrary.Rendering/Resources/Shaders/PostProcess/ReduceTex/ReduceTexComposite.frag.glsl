#version 330

layout ( location = 0 ) out vec4 oColor;

in vec2 fTexCoord;
in vec2 fTexCoord0;
in vec2 fTexCoord1;
in vec2 fTexCoord2;
in vec2 fTexCoord3;

uniform sampler2D uTexture0;
uniform sampler2D uTexture1;
uniform sampler2D uTexture2;
uniform sampler2D uTexture3;

const vec4 COL = vec4( 0.15, 0.25, 0.25, 0.25 );

void main()
{
    vec4 c0 = texture( uTexture3, fTexCoord0 );
    vec4 c1 = texture( uTexture3, fTexCoord1 );
    vec4 c2 = texture( uTexture3, fTexCoord2 );
    vec4 c3 = texture( uTexture3, fTexCoord3 );

    vec4 color = ( c0 + c1 + c2 + c3 ) * 0.25 * COL.w;
    c0 = texture( uTexture0, fTexCoord ) * COL.x;
    c1 = texture( uTexture1, fTexCoord ) * COL.y;
    c2 = texture( uTexture2, fTexCoord ) * COL.z;

    oColor = color + c0 + c1 + c2;
}
