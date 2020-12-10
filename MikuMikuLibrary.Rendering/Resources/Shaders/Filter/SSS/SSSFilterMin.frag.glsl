#version 330

#include "../../ArbCommon.glsl"

uniform sampler2D uTexture;

in vec2 fTexCoord;
in vec2 fTexCoord0;
in vec2 fTexCoord1;
in vec2 fTexCoord2;
in vec2 fTexCoord3;

out vec4 oColor;

void main()
{
    /*
    vec4 color0 = texture( uTexture, fTexCoord );

    if ( color0.a == 1.0 )
    {
        oColor = color0;
        return;
    }

    vec4 color1 = texture( uTexture, fTexCoord0 );

    if ( color1.a > color0.a ) )
        color0 = color1;   
        
    color1 = texture( uTexture, fTexCoord1 );

    if ( color1.a > color0.a )
        color0 = color1;
        
    color1 = texture( uTexture, fTexCoord2 );

    if ( color1.a > color0.a )
        color0 = color1;   
        
    color1 = texture( uTexture, fTexCoord3 );

    if ( color1.a > color0.a )
        color0 = color1;

    oColor = color0;
    */

    vec4 tmp, col0, col1, cc0;

    col0 = texture(uTexture, fTexCoord.xy);
    oColor = col0;
    tmp.w = col0.w - 1;
    cc0.w = GetCC(tmp.w);
    if (BCCEQ(cc0.w))
        return;
    col1 = texture(uTexture, fTexCoord0.xy);
    tmp.w = col0.w - col1.w;
    cc0.w = GetCC(tmp.w);
    col0 = CCLT(cc0.w, col1, col0);
    col1 = texture(uTexture, fTexCoord1.xy);
    tmp.w = col0.w - col1.w;
    cc0.w = GetCC(tmp.w);
    col0 = CCLT(cc0.w, col1, col0);
    col1 = texture(uTexture, fTexCoord2.xy);
    tmp.w = col0.w - col1.w;
    cc0.w = GetCC(tmp.w);
    col0 = CCLT(cc0.w, col1, col0);
    col1 = texture(uTexture, fTexCoord3.xy);
    tmp.w = col0.w - col1.w;
    cc0.w = GetCC(tmp.w);
    col0 = CCLT(cc0.w, col1, col0);
    oColor = col0;
}