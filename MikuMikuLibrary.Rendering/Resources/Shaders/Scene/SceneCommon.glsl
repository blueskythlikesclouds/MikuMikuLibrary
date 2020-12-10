#version 330

#ifndef SCENE_COMMON_GLSL_INCLUDED
#define SCENE_COMMON_GLSL_INCLUDED

#include "SceneDefinitions.glsl"

const float SPEC_COEF = 1.0 / ( 1.0 - cos( 0.3141592653589793 ) );
const float LUCE_COEF = 1.0 / ( 1.0 - cos( 0.7853981633974483 ) );
const float ESM_COEF = ( 80 * 9.95 ) * 2 * sqrt( 2 );
const float SSS_COEF = 0.6;

// Passed at 0x1405E4750
/*
const float GLASS_EYE_TABLE[ 48 ] =
{
    5.0, 5.0, 0.5, 0.5, 2.5, 2.5, 0.5, 0.5, 1.0, 1.45, 0.02, 0.85, 1.0, 1.2, 0.45, 1.0, 1.2, 
    0.25, 0.5, 0.6, 0.4, -0.2, 1.0, 1.0, 1.0, 42.5, 0.02, 0.024, 0.009, 0.02, 0.024, 0.005, 
    0.01, 0.012, 0.008, -0.004, 1.0, 1.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0
};
*/

float computeLucency( vec3 normal, vec3 lightDir, vec3 viewDir )
{
    vec4 luce;

    luce.x = saturate( dot( -viewDir, lightDir ) );
    luce.w = pow( luce.x, 8.0 );
    luce.x = luce.x + luce.w;
    luce.y = dot( normal, lightDir );
    luce.y = luce.y * 1.0 + 1.0;
    luce.y = saturate( luce.y * luce.y );
    luce.x = luce.x * luce.y;

    return luce.x * uFresnelCoefficientAndShininess.z;
}

vec3 computeAnisoTangent( vec3 normal, vec3 tangent, vec3 bitangent, vec2 texCoord )
{
    if ( ( uShaderFlags & SHADER_FLAGS_ANISO_DIRECTION_NORMAL ) != 0 )
        return normal;

    if ( ( uShaderFlags & SHADER_FLAGS_ANISO_DIRECTION_U ) != 0 )
        return tangent;    
        
    if ( ( uShaderFlags & SHADER_FLAGS_ANISO_DIRECTION_V ) != 0 )
        return bitangent;

    // SHADER_FLAGS_ANISO_DIRECTION_RADIAL

    // I literally don't know how to decipher this
    // Thanks AM2

    vec4 _tmp0, tmp, aniso_tangent, o_aniso_tangent;

    tmp.xyz = normal;
    _tmp0.y = bitangent.x;
    _tmp0.z = tmp.x;
    bitangent.x = tangent.y;
    tmp.x = tangent.z;
    tangent.yz = _tmp0.yz;
    _tmp0.z = tmp.y;
    tmp.y = bitangent.z;
    bitangent.z = _tmp0.z;
    aniso_tangent.xy = texCoord * 2.0 + -1.0;
    aniso_tangent.z = 0.01;
    o_aniso_tangent.x = dot( tangent, aniso_tangent.xyz );
    o_aniso_tangent.y = dot( bitangent, aniso_tangent.xyz );
    o_aniso_tangent.z = dot( tmp.xyz, aniso_tangent.xyz );

    return o_aniso_tangent.xyz;
}

vec3 computeShadow( vec3 normal, vec4 shadowMapCoord, vec3 lightDir )
{
    vec3 shadowMapCoordNormalized = shadowMapCoord.xyz / shadowMapCoord.w;

    float shadow = texture( uShadowMapTexture, shadowMapCoordNormalized.xy ).x;
    shadow -= shadowMapCoordNormalized.z;
    shadow *= ESM_COEF;
    shadow *= uEmission.w;
    shadow = saturate( exp( shadow ) );

    float factor = dot( lightDir, normal );
    factor = saturate( factor + 1 );
    factor = factor * factor;
    factor = factor * factor;

    return vec3( shadow, vec2( min( shadow, factor ) ) );
}

vec3 computeNormal( vec3 normal, vec3 tangent, vec3 bitangent, vec2 texCoord )
{
    vec4 tmp0, tmp1;

    tmp0 = texture( uNormalTexture, texCoord.xy );
    tmp0.xy = tmp0.xy * 2.0 - 1.0;
    tmp0.zw = tmp0.xy * tmp0.xy;
    tmp0.zw = tmp0.zw * tmp0.xy;
    tmp0 = tmp0 * vec4( 1.5, 1.5, 2.0, 2.0 );
    tmp0.xy = tmp0.xy + tmp0.zw;

    tmp1.xyz = tangent * tmp0.x;
    tmp1.xyz = bitangent * tmp0.y + tmp1.xyz;
    tmp1.xyz = tmp1.xyz + normal;

    return normalize( tmp1.xyz );
}

vec3 computeDiffuseIBL( vec3 normal, float shadow )
{
    return mix( texture( uDiffuseIBLShadowedTexture, normal ), texture( uDiffuseIBLTexture, normal ), shadow ).rgb;
}

vec3 computeSpecularIBL( vec3 refDir, float shadow )
{
    vec3 specularIBLRough = mix(
        texture( uSpecularIBLRoughShadowedTexture, refDir ),
        texture( uSpecularIBLRoughTexture, refDir ), shadow ).rgb;  
        
    vec3 specularIBLShiny = mix(
        texture( uSpecularIBLShinyShadowedTexture, refDir ),
        texture( uSpecularIBLShinyTexture, refDir ), shadow ).rgb;

    float factor = uFresnelCoefficientAndShininess.w / 128.0;

    return mix( specularIBLRough, specularIBLShiny, factor );
}

#endif