#version 330

layout ( location = 0 ) out vec4 oColor;

in vec2 fTexCoord0;
in vec2 fTexCoord1;
in vec2 fTexCoord2;
in vec2 fTexCoord3;

uniform sampler2D uTexture;

const vec3 EXT_COL = vec3( 1.1, 1.1, 1.1 );
const vec3 TO_YBR = vec3( 0.35, 0.45, 0.2 );

void main()
{
    vec4 c0 = texture( uTexture, fTexCoord0 );
    vec4 c1 = texture( uTexture, fTexCoord1 );
    vec4 c2 = texture( uTexture, fTexCoord2 );
    vec4 c3 = texture( uTexture, fTexCoord3 );

    vec4 color = ( c0 + c1 + c2 + c3 ) / 4;
    color.w = dot( color.rgb, TO_YBR );
    color.rgb = max( c0.rgb, c1.rgb );
    color.rgb = max( color.rgb, c2.rgb );
    color.rgb = max( color.rgb, c3.rgb );
    color.rgb -= EXT_COL;
    color.rgb = max( color.rgb, vec3( 0.0 ) );
    oColor = color;
}
