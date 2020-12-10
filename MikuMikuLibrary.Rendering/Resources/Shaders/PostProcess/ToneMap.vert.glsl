#version 330

#include "ToneMapDefinitions.glsl"

layout ( location = 0 ) in vec2 aPosition;

out vec2 fTexCoord;
out vec3 fExposure;

uniform sampler2D uExposureTexture;

void main()
{
    float tmp = texture( uExposureTexture, vec2( 0, 0 ) ).x;
    tmp = exp2( -tmp * 1.8 ) * 2.9 + 0.4;
    tmp = uExposure.w > 0.0 ? tmp * uExposure.z : uExposure.x;
    fExposure.x = tmp;
    fExposure.y = tmp * uExposure.y;
    fExposure.z = 1.0;
    fTexCoord = aPosition * 0.5 + 0.5;
    gl_Position = vec4( aPosition, 0.0, 1.0 );
}
