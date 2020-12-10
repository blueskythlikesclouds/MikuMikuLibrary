#version 330

layout ( location = 0 ) in vec2 aPosition;

out vec2 fTexCoord0;
out vec2 fTexCoord1;
out vec2 fTexCoord2;
out vec2 fTexCoord3;
out vec2 fTexCoord4;
out vec2 fTexCoord5;
out vec2 fTexCoord6;
out vec2 fTexCoord7;

uniform vec4 uResolution;

void main()
{
    vec2 texCoord = aPosition * 0.5 + 0.5;
    fTexCoord0 = texCoord + vec2( -1.5, -0.6 ) * uResolution.zw;
    fTexCoord1 = texCoord + vec2( -0.5, -0.6 ) * uResolution.zw;
    fTexCoord2 = texCoord + vec2( 0.5, -0.6 ) * uResolution.zw;
    fTexCoord3 = texCoord + vec2( 1.5, -0.6 ) * uResolution.zw;
    fTexCoord4 = texCoord + vec2( -1.5, 0.6 ) * uResolution.zw;
    fTexCoord5 = texCoord + vec2( -0.5, 0.6 ) * uResolution.zw;
    fTexCoord6 = texCoord + vec2( 0.5, 0.6 ) * uResolution.zw;
    fTexCoord7 = texCoord + vec2( 1.5, 0.6 ) * uResolution.zw;
    gl_Position = vec4( aPosition, 0.0, 1.0 );
}
