#version 330

layout ( location = 0 ) in vec2 aPosition;

out vec2 fTexCoord;
out vec2 fTexCoord0;
out vec2 fTexCoord1;
out vec2 fTexCoord2;
out vec2 fTexCoord3;

uniform vec4 uResolution;

void main()
{
    fTexCoord = aPosition * 0.5 + 0.5;
    fTexCoord0 = fTexCoord + vec2( -1.0, -1.0 ) * uResolution.zw;
    fTexCoord1 = fTexCoord + vec2( -1.0, 1.0 ) * uResolution.zw;
    fTexCoord2 = fTexCoord + vec2( 1.0, -1.0 ) * uResolution.zw;
    fTexCoord3 = fTexCoord + vec2( 1.0, 1.0 ) * uResolution.zw;
    gl_Position = vec4( aPosition, 0.0, 1.0 );
}
