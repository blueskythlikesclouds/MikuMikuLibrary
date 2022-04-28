#version 330

out vec4 oColor;

in vec3 fPosition;
in vec3 fNormal;
in vec3 fTangent;
in vec3 fBitangent;
in vec2 fTexCoord0;
in vec2 fTexCoord1;
in vec4 fColor0;

uniform bool uHasNormal;
uniform bool uHasTexCoord0;
uniform bool uHasTexCoord1;
uniform bool uHasColor0;
uniform bool uHasTangent;

uniform bool uHasDiffuseTexture;
uniform bool uHasAmbientTexture;
uniform bool uHasNormalTexture;
uniform bool uHasSpecularTexture;
uniform bool uHasReflectionTexture;
uniform bool uHasToonCurveTexture;

uniform sampler2D uDiffuseTexture;
uniform sampler2D uAmbientTexture;
uniform sampler2D uNormalTexture;
uniform sampler2D uSpecularTexture;
uniform samplerCube uReflectionTexture;
uniform sampler2D uToonCurveTexture;

uniform vec4 uDiffuseColor;
uniform vec4 uAmbientColor;
uniform vec4 uSpecularColor;
uniform float uShininess;
uniform int uAnisoDirection;
uniform bool uPunchThrough;

uniform vec3 uViewPosition;
uniform vec3 uLightPosition;

const float ALPHA_THRESHOLD = 0.5;

vec2 yccLookup(float x)
{
    float v9 = 1.5;
    float samples = 32;
    float scale = 1.0 / samples;
    float i = x * 16 * samples;
    float v11 = exp( -i * scale );
    float v10 = pow( 1.0 - v11, v9 );
    v11 = v10 * 2.0 - 1.0;
    v11 *= v11;
    v11 *= v11;
    v11 *= v11;
    v11 *= v11;

    return vec2( v10, v10 * ( samples / i ) * ( 1.0 - v11 ) );
}

vec3 yccToneMap( vec3 c )
{
    float exposure = 2.0;

    vec4 color;
    color.rgb = c;

    color.y = dot( color.rgb, vec3( 0.30, 0.59, 0.11 ) );
    color.rb -= color.y;
    color.yw = yccLookup( color.y * exposure * 0.0625 );
    color.rb *= exposure * color.w;
    color.w = dot( color.rgb, vec3( -0.508475, 1.0, -0.186441 ) );
    color.rb += color.y;
    color.g = color.w;

    return color.rgb;
}

void standard()
{
    vec3 viewDirection = normalize( uViewPosition - fPosition );
    vec3 lightDirection = normalize( uLightPosition - fPosition );
    vec3 halfwayDirection = normalize( viewDirection + lightDirection );

    vec4 diffuseColor = uDiffuseColor;
    vec4 specularColor = uSpecularColor;
    vec3 ambientColor = uAmbientColor.rgb;

    specularColor.rgb *= 2.0;

    if ( uHasDiffuseTexture && uHasTexCoord0 )
        diffuseColor *= texture( uDiffuseTexture, fTexCoord0 );

    if ( uHasColor0 )
        diffuseColor *= fColor0;
        
    if ( uPunchThrough && diffuseColor.a < ALPHA_THRESHOLD )
        discard;

    if ( uHasSpecularTexture && uHasTexCoord0 )
        specularColor *= texture( uSpecularTexture, fTexCoord0 );

    if ( uHasAmbientTexture && uHasTexCoord1 )
        diffuseColor.rgb *= texture( uAmbientTexture, fTexCoord1 ).rgb;

    float fresnel = 0;
    vec3 directLighting = vec3( 0 );

    vec3 normal = normalize( fNormal );
    if ( uHasNormal )
    {
        if ( uHasTangent && uHasNormalTexture && uHasTexCoord0 )
        {
            normal = texture( uNormalTexture, fTexCoord0 ).xyz * 2 - 1;
            normal.z = sqrt( 1 - normal.x * normal.x - normal.y * normal.y );
            normal = normalize( fTangent * normal.x + fBitangent * normal.y + fNormal * normal.z );
        }

        fresnel = pow( 1 - clamp( dot( normal, viewDirection ), 0, 1 ), 5 );

        directLighting += diffuseColor.rgb;
        
        if ( uAnisoDirection > 0 && uAnisoDirection < 3 && uHasTangent && uHasTexCoord0 )
        {
            float dotTH = dot( uAnisoDirection == 2 ? normalize( fBitangent ) : normalize( fTangent ), halfwayDirection );
            float sinTH = sqrt( 1 - dotTH * dotTH );
            float dirAtten = smoothstep( -1, 0, dotTH );
            directLighting += ( dirAtten * pow( sinTH, uShininess ) ) * specularColor.rgb;
        }
        else
        {
            directLighting += pow( clamp( dot( normal, halfwayDirection ), 0, 1 ), uShininess ) * specularColor.rgb;
        }
    }

    directLighting *= clamp( dot( normal, lightDirection ), 0, 1 );

    vec3 indirectLighting = mix( ambientColor, vec3( 1 ), fresnel ) * diffuseColor.rgb;

    if ( uHasNormal && uHasReflectionTexture )
        indirectLighting += texture( uReflectionTexture, reflect( -viewDirection, normal ) ).rgb * mix( specularColor.w * ( dot( normal, lightDirection ) * 0.5 + 0.5 ), 1, fresnel );

    oColor = vec4( yccToneMap( directLighting + indirectLighting ), diffuseColor.a );
}

void chara()
{
    vec4 diffuse = texture( uDiffuseTexture, fTexCoord0 );

    if ( uPunchThrough && diffuse.a < ALPHA_THRESHOLD )
        discard;

    vec4 specular = texture( uSpecularTexture, fTexCoord0 );

    vec3 normal = normalize( fNormal );
    vec3 viewDirection = normalize( uViewPosition - fPosition );
    vec3 lightDirection = normalize( uLightPosition - fPosition );
    vec3 halfwayDirection = normalize( viewDirection + lightDirection );

    if ( uAnisoDirection > 0 )
        halfwayDirection = normalize( viewDirection + vec3( 0, 1, 0 ) );

    float nDotV = dot( normal, viewDirection );
    float nDotL = dot( normal, lightDirection );
    float nDotH = dot( normal, halfwayDirection );

    vec3 diffuseToonCurve = texture( uToonCurveTexture, vec2( nDotL * 0.5 + 0.5, 0.875 ) ).rgb;
    vec3 specularToonCurve = texture( uToonCurveTexture, vec2( clamp( nDotH, 0, 1 ), 0.625 ) ).rgb;
    vec3 fresnelToonCurve = texture( uToonCurveTexture, vec2( clamp( 1 - nDotV, 0, 1 ), 0.375 ) ).rgb;

    vec3 diffuseLighting = diffuse.rgb * diffuse.rgb * diffuseToonCurve.rgb;
    vec3 specularLighting = specular.rgb * specular.rgb * specularToonCurve.rgb * 1.551;

    if ( uAnisoDirection == 0 )
        specularLighting *= ( 1 - pow( clamp( 1 - nDotL, 0, 1 ), 8 ) );
    
    vec3 fresnelLighting = fresnelToonCurve * specular.a * 0.431;

    oColor = vec4( pow( clamp( diffuseLighting + specularLighting + fresnelLighting, 0, 1 ), vec3( 0.625 ) ), diffuse.a );
}

void main()
{
    if ( uHasToonCurveTexture )
        chara();

    else
        standard();
}