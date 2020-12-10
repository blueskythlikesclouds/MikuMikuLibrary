#version 330

#ifndef HAIR_GLSL_INCLUDED
#define HAIR_GLSL_INCLUDED

#ifndef SCENE_ATTRIBUTES_GLSL_INCLUDED

#define ATTRIB_TOKEN in
#include "../SceneAttributes.glsl"

#endif

#include "../SceneCommon.glsl"

vec4 HAIR()
{
    vec4 _tmp0, _tmp1, _tmp2;
    vec4 normal;
    vec4 rot_normal;
    vec4 lc, spec, diff, luce;
    vec4 col0;
    vec4 tmp;
    vec4 aniso_tangent, eye;
    vec4 aniso_coef;
    vec4 ndote;

    if ( ( uShaderFlags & SHADER_FLAGS_NORMAL ) != 0 )
    {
         tmp = texture( uNormalTexture, fTexCoord.xy );
         tmp.xy = tmp.xy * 2.0 + -1.0;
         tmp.zw = tmp.xy * tmp.xy;
         tmp.zw = tmp.zw * tmp.xy;
         tmp = tmp * vec4( 1.5, 1.5, 2.0, 2.0 );
         tmp.xy = tmp.xy + tmp.zw;
         normal.xyz = fTangent * tmp.x;
         normal.xyz = fBitangent * tmp.y + normal.xyz;
         normal.xyz = normal.xyz + fNormal;
         normal.xyz = normalize( normal.xyz );
    }

    else
    {
        normal.xyz = normalize( fNormal );
    }

    eye.xyz = normalize( uViewPosition - fPosition );

    rot_normal.x = dot( uIBL.IBLSpace[ 0 ].xyz, normal.xyz );
    rot_normal.y = dot( uIBL.IBLSpace[ 1 ].xyz, normal.xyz );
    rot_normal.z = dot( uIBL.IBLSpace[ 2 ].xyz, normal.xyz );

    vec4 a_tex_shadow0 = fShadowMapCoord / fShadowMapCoord.w;

    _tmp0 = texture( uShadowMapTexture, a_tex_shadow0.xy );
    _tmp0.x = _tmp0.x - a_tex_shadow0.z;
    _tmp0.x = _tmp0.x * ESM_COEF;
    _tmp0.x = _tmp0.x * uEmission.w;
    _tmp0.x = saturate( exp( _tmp0.x ) );
    _tmp0.y = dot( uCharaLight.Direction, normal.xyz );
    _tmp0.y = saturate( _tmp0.y + 1 );
    _tmp0.y = _tmp0.y * _tmp0.y;
    _tmp0.y = _tmp0.y * _tmp0.y;
    lc.yz = vec2( min( _tmp0.x, _tmp0.y ) );
    lc.x = _tmp0.x;

    tmp.x = saturate( dot( normal.xyz, eye.xyz ) );
    ndote.x = tmp.x;
    tmp.x = tmp.x * -tmp.x + 1.0;
    tmp.x = pow( tmp.x, 8.0 );
    tmp.x = tmp.x * fAnisoTangentAndLucency.w;
    luce.xyz = ( uIBL.FrontLight * uCharaLight.Specular.xyz * LUCE_COEF ) * tmp.x;
    luce.xyz = luce.xyz * lc.z;
    luce.xyz = luce.xyz * 0;

    diff = texture( uDiffuseIBLTexture, rot_normal.xyz );
    tmp = texture( uDiffuseIBLShadowedTexture, rot_normal.xyz );
    diff = mix( tmp, diff, lc.y );

    if ( ( uShaderFlags & SHADER_FLAGS_DIFFUSE ) != 0 )
        col0 = texture( uDiffuseTexture, fTexCoord.xy );

    else
        col0 = uDiffuse;

    col0.xyz = col0.xyz;
    diff.xyz = diff.xyz * uCharaLight.Diffuse.xyz/* + a_color0.w*/;
    diff.xyz = diff.xyz + uCharaLight.Ambient.xyz;
    diff.xyz = diff.xyz;
    diff.xyz = diff.xyz * col0.xyz;
    aniso_tangent.xyz = normalize( fAnisoTangentAndLucency.xyz );
    aniso_tangent.w = dot( aniso_tangent.xyz, normal.xyz );
    aniso_tangent.xyz = -aniso_tangent.w * normal.xyz + aniso_tangent.xyz;
    aniso_tangent.xyz = normalize( aniso_tangent.xyz );
    tmp.x = dot( aniso_tangent.xyz, uCharaLight.Direction );
    tmp.y = dot( aniso_tangent.xyz, eye.xyz );
    tmp.w = -tmp.x;
    tmp = tmp.xyxw * tmp.xyyy + vec4( -1.01, -1.01, 0, 0 );
    aniso_coef.x = 1 / sqrt( -tmp.x );
    aniso_coef.y = 1 / sqrt( -tmp.y );
    tmp.xy = -tmp.xy * aniso_coef.xy;
    tmp.yz = saturate( tmp.x * tmp.y + -tmp.zw );
    tmp.y = pow( tmp.y, uFresnelCoefficientAndShininess.w );
    tmp.z = pow( tmp.z, uFresnelCoefficientAndShininess.w );
    tmp.x = tmp.x * tmp.x;
    aniso_coef.yz = vec2( dot( normal.xyz, uCharaLight.Direction ) );
    aniso_coef.yz = saturate( aniso_coef * vec4( 0, 0.7, -0.7, 0 ) + vec4( 0, 0.3, 0.3, 0 ) ).yz;
    tmp.yz = tmp.yz * aniso_coef.yz;
    aniso_coef = tmp * vec4( 0.25, 0.18, 0.05, 0 ) + vec4( 0.75, 0, 0, 0 );
    spec = ( vec4( uIBL.FrontLight, 1 ) * uCharaLight.Specular * SPEC_COEF ) * aniso_coef.y;
    tmp = ( vec4( uIBL.BackLight, 1 ) * uCharaLight.Specular * SPEC_COEF ) * aniso_coef.z;
    spec = spec * lc.z + tmp;
    spec.xyz = spec.xyz;

    if ( ( uShaderFlags & SHADER_FLAGS_SPECULAR ) != 0 )
    {
        tmp = texture( uSpecularTexture, fTexCoord.xy );
        spec = spec * tmp;
    }

    diff = diff * aniso_coef.x;
    diff = luce * 0.5 + diff;
    diff.xyz = spec.xyz * uSpecular.xyz + diff.xyz;
    diff.xyz = diff.xyz;

    if ( ( uShaderFlags & SHADER_FLAGS_TRANSPARENCY ) != 0 )
    {
        tmp = texture( uTransparencyTexture, fTexCoord.xy );
        diff.w = tmp.x;
    }

    return vec4( diff.xyz, col0.w );
}

#endif