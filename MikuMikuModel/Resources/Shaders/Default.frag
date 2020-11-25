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

uniform sampler2D uDiffuseTexture;
uniform sampler2D uAmbientTexture;
uniform sampler2D uNormalTexture;
uniform sampler2D uSpecularTexture;
uniform samplerCube uReflectionTexture;

uniform vec4 uDiffuseColor;
uniform vec4 uAmbientColor;
uniform vec4 uSpecularColor;
uniform float uShininess;
uniform int uAnisoDirection;

uniform vec3 uViewPosition;
uniform vec3 uLightPosition;

const vec4 GAMMA = vec4( vec3( 2.2 ), 1 );
const vec4 INVERSE_GAMMA = 1.0 / GAMMA;
const float ALPHA_THRESHOLD = 0.5;

void main()
{
    vec3 viewDirection = normalize( uViewPosition - fPosition );
    vec3 lightDirection = normalize( uLightPosition - fPosition );
    vec3 halfwayDirection = normalize( viewDirection + lightDirection );

    vec4 diffuseColor = uDiffuseColor;
    vec4 specularColor = uSpecularColor;
    vec3 ambientColor = uAmbientColor.rgb;

    if ( uHasDiffuseTexture && uHasTexCoord0 )
        diffuseColor *= pow( texture( uDiffuseTexture, fTexCoord0 ), GAMMA );

    if ( uHasColor0 )
        diffuseColor *= fColor0;

    if ( diffuseColor.a < ALPHA_THRESHOLD )
        discard;

    if ( uHasSpecularTexture && uHasTexCoord0 )
        specularColor *= texture( uSpecularTexture, fTexCoord0 );

    if ( uHasAmbientTexture && uHasTexCoord1 )
        ambientColor *= texture( uAmbientTexture, fTexCoord1 ).rgb;

    vec3 directLighting = vec3( 0 );

    vec3 normal = normalize( fNormal );
    if ( uHasNormal )
    {
        if ( uHasTangent && uHasNormalTexture && uHasTexCoord0 )
        {
            mat3 tangentToWorldMatrix = mat3( fTangent, fBitangent, fNormal );
            
            normal = texture( uNormalTexture, fTexCoord0 ).xyz * 2 - 1;
            normal.z = sqrt( 1 - normal.x * normal.x - normal.y * normal.y );
            normal = normalize( tangentToWorldMatrix * normal );
        }

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
            directLighting += pow( max( 0, dot( normal, halfwayDirection ) ), uShininess ) * specularColor.rgb;
        }
    }

    directLighting *= max( 0, dot( normal, lightDirection ) );

    vec3 indirectLighting = ambientColor * diffuseColor.rgb;

    if ( uHasNormal && uHasReflectionTexture )
        indirectLighting += texture( uReflectionTexture, reflect( -viewDirection, normal ) ).rgb * specularColor.rgb * specularColor.w;

    oColor = pow( vec4( directLighting + indirectLighting, diffuseColor.a ), INVERSE_GAMMA );
}