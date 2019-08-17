#version 330

out vec4 oColor;

in vec3 fPosition;
in vec3 fNormal;
in vec3 fTangent;
in vec3 fBitangent;
in vec2 fTexCoord1;
in vec2 fTexCoord2;
in vec4 fColor;

uniform bool uHasNormal;
uniform bool uHasTexCoord1;
uniform bool uHasTexCoord2;
uniform bool uHasColor;
uniform bool uHasTangent;

uniform bool uHasDiffuseTexture;
uniform bool uHasAmbientTexture;
uniform bool uHasNormalTexture;
uniform bool uHasSpecularTexture;
uniform bool uHasReflectionTexture;
uniform bool uHasTangentTexture;

uniform sampler2D uDiffuseTexture;
uniform sampler2D uAmbientTexture;
uniform sampler2D uNormalTexture;
uniform sampler2D uSpecularTexture;
uniform samplerCube uReflectionTexture;
uniform sampler2D uTangentTexture;

uniform vec4 uDiffuseColor;
uniform vec4 uSpecularColor;
uniform float uSpecularPower;

uniform bool uUseAniso;

uniform vec3 uViewPosition;
uniform vec3 uLightPosition;

const vec4 GAMMA = vec4( vec3( 2.2 ), 1 );
const vec4 INVERSE_GAMMA = 1.0 / GAMMA;
const float ALPHA_THRESHOLD = 0.1;

void main()
{
    vec3 viewDirection = normalize( uViewPosition - fPosition );
    vec3 lightDirection = normalize( uLightPosition - fPosition );
    vec3 halfwayDirection = normalize( viewDirection + lightDirection );

    vec4 diffuseColor = uDiffuseColor;
    vec4 specularColor = uSpecularColor;

    vec3 normal = fNormal;
    mat3 tbn = transpose( mat3( fTangent, fBitangent, fNormal ) );

    if ( uHasDiffuseTexture && uHasTexCoord1 )
        diffuseColor *= pow( texture( uDiffuseTexture, fTexCoord1 ), GAMMA );

    if ( uHasColor )
        diffuseColor *= fColor;

    if ( diffuseColor.a < ALPHA_THRESHOLD )
        discard;

    if ( uHasNormal )
    {
        if ( uHasTangent && uHasNormalTexture && uHasTexCoord1 )
        {
            normal = texture( uNormalTexture, fTexCoord1 ).xyz * 2 - 1;
            normal.z = sqrt( 1 - normal.x * normal.x - normal.y * normal.y );
            normal *= tbn;
            normal = normalize( normal );
        }

        diffuseColor.xyz *= mix( 0.25, 1, dot( normal, lightDirection ) );
    }

    if ( uHasNormal )
    {
        if ( uHasTangentTexture && uHasTexCoord1 )
            specularColor *= texture( uTangentTexture, fTexCoord1 );

        else if ( uHasSpecularTexture && uHasTexCoord1 )
            specularColor *= texture( uSpecularTexture, fTexCoord1 );

        if ( dot( normal, lightDirection ) <= 0 )
        {
            specularColor = vec4( 0 );
        }
        else if ( uUseAniso && uHasTangent && uHasTexCoord1 )
        {
            float dotTH = dot( fBitangent, halfwayDirection );
            float sinTH = sqrt( 1 - dotTH * dotTH );
            float dirAtten = smoothstep( -1, 0, dotTH );
            specularColor.xyz *= dirAtten * pow( sinTH, uSpecularPower );
        }
        else
        {
            specularColor.xyz *= pow( dot( normal, halfwayDirection ), uSpecularPower );
        }
    }

    vec4 color = vec4( ( diffuseColor + specularColor ).xyz, diffuseColor.w );

    if ( uHasReflectionTexture )
        color.xyz += pow( texture( uReflectionTexture, reflect( -viewDirection, normal ) ), GAMMA ).xyz * specularColor.w * 0.05;

    if ( uHasAmbientTexture && uHasTexCoord2 )
        color *= pow( texture( uAmbientTexture, fTexCoord2 ), GAMMA );

    oColor = pow( color, INVERSE_GAMMA );
}