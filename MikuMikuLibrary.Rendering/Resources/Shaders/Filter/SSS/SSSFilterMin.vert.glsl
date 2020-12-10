#version 330

layout ( location = 0 ) in vec2 aPosition;

out vec2 fTexCoord;
out vec2 fTexCoord0;
out vec2 fTexCoord1;
out vec2 fTexCoord2;
out vec2 fTexCoord3;

uniform vec2 uResolution;

void main()
{
    fTexCoord = aPosition.xy * 0.5 + 0.5;

    fTexCoord0 = vec2( -2, 0 ) * uResolution.xy + fTexCoord;
    fTexCoord1 = vec2( +2, 0 ) * uResolution.xy + fTexCoord;
    fTexCoord2 = vec2( 0, -2 ) * uResolution.xy + fTexCoord;
    fTexCoord3 = vec2( 0, +2 ) * uResolution.xy + fTexCoord;

    gl_Position = vec4( aPosition, 0.0, 1.0 );
}
