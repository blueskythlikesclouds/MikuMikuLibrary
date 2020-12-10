#version 330

#ifndef ITEM_GLSL_INCLUDED
#define ITEM_GLSL_INCLUDED

#ifndef SCENE_ATTRIBUTES_GLSL_INCLUDED

#define ATTRIB_TOKEN in
#include "../SceneAttributes.glsl"

#endif

#include "../SceneCommon.glsl"

vec4 ITEM()
{
    vec4 _tmp0, _tmp1, _tmp2;
    vec4 normal, eye, ref;
    vec4 org_normal, org_eye;
    vec4 lc, spec, diff, env;
    vec4 spec_ratio;
    vec4 col0;
    vec4 tmp;
    vec4 ndote;

    if ( ( uShaderFlags & SHADER_FLAGS_DIFFUSE ) != 0 )
        col0 = texture( uDiffuseTexture, fTexCoord.xy );

    else
        col0 = uDiffuse;

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

    org_normal = normal;
    org_eye = eye;

    normal.x = dot( uIBL.IBLSpace[ 0 ].xyz, org_normal.xyz );
    normal.y = dot( uIBL.IBLSpace[ 1 ].xyz, org_normal.xyz );
    normal.z = dot( uIBL.IBLSpace[ 2 ].xyz, org_normal.xyz );

    eye.x = dot( uIBL.IBLSpace[ 0 ].xyz, org_eye.xyz );
    eye.y = dot( uIBL.IBLSpace[ 1 ].xyz, org_eye.xyz );
    eye.z = dot( uIBL.IBLSpace[ 2 ].xyz, org_eye.xyz );

    tmp.x = dot( org_eye.xyz, org_normal.xyz );
    tmp.x = tmp.x * 2.0;
    tmp.xyz = tmp.x * org_normal.xyz + -org_eye.xyz;
    ref.x = dot( uIBL.IBLSpace[ 0 ].xyz, tmp.xyz );
    ref.y = dot( uIBL.IBLSpace[ 1 ].xyz, tmp.xyz );
    ref.z = dot( uIBL.IBLSpace[ 2 ].xyz, tmp.xyz );

    vec4 a_tex_shadow0 = fShadowMapCoord / fShadowMapCoord.w;

    _tmp0 = texture( uShadowMapTexture, a_tex_shadow0.xy );
    _tmp0.x = _tmp0.x - a_tex_shadow0.z;
    _tmp0.x = _tmp0.x * ESM_COEF;
    _tmp0.x = _tmp0.x * uEmission.w;
    _tmp0.x = saturate( exp( _tmp0.x ) );
    _tmp0.y = dot( uCharaLight.Direction, org_normal.xyz );
    _tmp0.y = saturate( _tmp0.y + 1 );
    _tmp0.y = _tmp0.y * _tmp0.y;
    _tmp0.y = _tmp0.y * _tmp0.y;
    lc.yz = vec2( min( _tmp0.x, _tmp0.y ) );
    lc.x = _tmp0.x;

    tmp.w = saturate( dot( normal.xyz, eye.xyz ) );
    ndote.x = tmp.w;
    tmp.x = 1.0 - tmp.w;
    tmp.x = pow( tmp.x, 5.0 );
    tmp.y = lc.z * 0.7 + 0.3;
    tmp.x = tmp.x * tmp.y;
    spec_ratio.x = tmp.x * uFresnelCoefficientAndShininess.x + uFresnelCoefficientAndShininess.y;
    spec_ratio.w = uFresnelCoefficientAndShininess.x * 10.0;
    spec_ratio.w = spec_ratio.w * tmp.x + 1;
    spec_ratio = spec_ratio.xxxw * uSpecular;

    diff = texture( uDiffuseIBLTexture, normal.xyz );
    tmp = texture( uDiffuseIBLShadowedTexture, normal.xyz );
    diff = mix( tmp, diff, lc.y );

    diff.xyz = diff.xyz * uCharaLight.Diffuse.xyz/* + a_color0.w*/;
    diff.xyz = diff.xyz + uCharaLight.Ambient.xyz;
    diff.xyz = diff.xyz + uEmission.xyz;
    diff.xyz = diff.xyz;
    diff.xyz = diff.xyz * col0.xyz;

    spec = texture( uSpecularIBLShinyTexture, ref.xyz );
    _tmp2 = texture( uSpecularIBLRoughTexture, ref.xyz );
    spec.xyz = mix( _tmp2, spec, uFresnelCoefficientAndShininess.w / 128 ).xyz;
    tmp = min( spec, 3.0 );
    spec.xyz = mix( tmp, spec, lc.z ).xyz;
    spec.xyz = spec.xyz * uCharaLight.Specular.xyz;

    if ( ( uShaderFlags & SHADER_FLAGS_SPECULAR ) != 0 )
    {
        tmp = texture( uSpecularTexture, fTexCoord.xy );
        spec_ratio = spec_ratio * tmp;
    }

    diff.xyz = diff.xyz * 0.96;
    diff.xyz = spec.xyz * spec_ratio.xyz + diff.xyz;

    if ( ( uShaderFlags & SHADER_FLAGS_ENVIRONMENT ) != 0 )
    {
        env = texture( uEnvironmentTexture, ref.xyz );
        env.w = lc.z * 0.5 + 0.5;
        env.w = env.w * uCharaLight.Specular.w;
        env.xyz = env.xyz * env.w;
        diff.xyz = env.xyz * spec_ratio.w + diff.xyz;
    }

    diff.xyz = diff.xyz;

    return vec4( diff.xyz, col0.w );
}

#endif