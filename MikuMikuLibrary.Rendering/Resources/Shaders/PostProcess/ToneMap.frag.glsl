#version 330

#include "ToneMapDefinitions.glsl"

#define YCC_EXPONENT

layout ( location = 0 ) out vec4 oColor;

in vec2 fTexCoord;
in vec3 fExposure;

const vec3 TO_YBR = vec3( 0.30, 0.59, 0.11 );
const vec3 TO_RGB = vec3( -0.508475, 1.0, -0.186441 );

uniform sampler2D uColorTexture;
uniform sampler2D uGlareTexture;

#ifdef YCC_EXPONENT
uniform sampler2D uLookupTexture;
#endif

void main()
{
    vec4 color = vec4( 0 );

    vec4 c0 = texture( uColorTexture, fTexCoord );
    vec4 c1 = texture( uGlareTexture, fTexCoord );
    color.rgb = mix( c0.rgb, c0.rgb + c1.rgb, fExposure.z > 0.0 );

#ifdef YCC_EXPONENT
    color.y = dot( color.rgb, TO_YBR );
    color.rb -= color.y;
    color.yw = texture( uLookupTexture, vec2( color.y * fExposure.y, 0 ) ).xy;
    color.rb *= fExposure.x * color.w;
    color.w = dot( color.rgb, TO_RGB );
    color.rb += color.y;
    color.g = color.w;
#endif

#ifdef RGB_LINEAR
    color.rgb = min( color.rgb * 0.48 * fExposure.x, 0.96 );
#endif

#ifdef RGB_LINEAR2
    color.rgb = min( color.rgb * 0.25 * fExposure.x, 0.80 );
#endif

    color.w = c0.w;

    color.rgb = color.rgb * uToneScale.rgb + uToneOffset.rgb;

#ifdef FADE
    bvec3 tmp = equal( vec3( uFadeFunc ) - vec3( 0.0, 1.0, 2.0 ), vec3( 0.0, 0.0, 0.0 ) );

    color.rgb = mix( color.rgb, uFadeColor.rgb, tmp.x * uFadeColor.a );
    color.rgb = mix( color.rgb, color.rgb * uFadeColor.rgb, tmp.y );
    color.rgb = mix( color.rgb, color.rgb + uFadeColor.rgb, tmp.z );
#endif

    oColor = color;
}
