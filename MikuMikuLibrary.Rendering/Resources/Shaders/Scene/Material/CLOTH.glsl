#version 330

#ifndef CLOTH_GLSL_INCLUDED
#define CLOTH_GLSL_INCLUDED

#ifndef SCENE_ATTRIBUTES_GLSL_INCLUDED

#define ATTRIB_TOKEN in
#include "../SceneAttributes.glsl"

#endif

#include "../SceneCommon.glsl"

vec4 CLOTH()
{
    vec4 _tmp0, _tmp1, _tmp2;
    vec4 dev_pos;
    vec4 normal, eye, ref;
    vec4 org_normal, org_eye;
    vec4 lc, spec, diff, luce, env;
    vec4 spec_ratio, fres;
    vec4 col0;
    vec4 tmp;
    vec4 ndote;

    if ( ( uShaderFlags & SHADER_FLAGS_DIFFUSE ) != 0 )
        col0 = texture( uDiffuseTexture, fTexCoord.xy );

    else
        col0 = uDiffuse;

    if ( ( uShaderFlags & SHADER_FLAGS_TRANSPARENCY ) != 0 )
    {
        tmp = texture( uTransparencyTexture, fTexCoord.xy );
        col0.w = tmp.w;
    }

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

    fres.w = saturate( dot( normal.xyz, eye.xyz ) );
    ndote.x = fres.w;
    fres.x = fres.w * -fres.w + 1.0;
    fres.x = fres.x * fres.x;
    fres.x = fres.x * fres.x;
    fres.w = 1.0 - fres.w;
    fres.w = pow( fres.w, 5.0 );
    tmp.x = fres.x * fAnisoTangentAndLucency.w;
    tmp.x = tmp.x * 0.75;
    tmp.x = tmp.x * lc.x;
    luce.xyz = ( uIBL.FrontLight * uCharaLight.Specular.xyz * LUCE_COEF ) * tmp.x;
    tmp.w = dot( -org_eye.xyz, uCharaLight.Direction );
    tmp.w = tmp.w * 0.2 + 0.4;
    tmp.w = tmp.w * fres.w;
    luce.w = tmp.w * 0.5;

    spec_ratio = uSpecular;
    if ( ( uShaderFlags & SHADER_FLAGS_SPECULAR ) != 0 )
    {
        tmp = texture( uSpecularTexture, fTexCoord.xy );
        spec_ratio = spec_ratio * tmp;
    }

    tmp.w = dot( spec_ratio.xyz, vec3( 1 ) );
    tmp.w = saturate( tmp.w * -3.0 + 1.3 );
    luce = luce * tmp.w;
    tmp.y = lc.z * 0.7 + 0.3;
    tmp.y = tmp.y * fres.w;
    tmp.x = tmp.y * uFresnelCoefficientAndShininess.x + uFresnelCoefficientAndShininess.y;
    tmp.w = uFresnelCoefficientAndShininess.x * 10.0;
    tmp.w = tmp.w * tmp.y + 1;
    spec_ratio = spec_ratio * tmp.xxxw;

    diff = texture( uDiffuseIBLTexture, normal.xyz );
    tmp = texture( uDiffuseIBLShadowedTexture, normal.xyz );
    diff = mix( tmp, diff, lc.y );

    diff.xyz = diff.xyz * uCharaLight.Diffuse.xyz /*+ a_color0.w*/;
    diff.xyz = diff.xyz + uCharaLight.Ambient.xyz;
    diff.xyz = diff.xyz + luce.xyz;
    diff.xyz = diff.xyz;
    diff.xyz = diff.xyz * col0.xyz;

    spec = texture( uSpecularIBLShinyTexture, ref.xyz );
    _tmp2 = texture( uSpecularIBLRoughTexture, ref.xyz );
    spec.xyz = mix( _tmp2, spec, uFresnelCoefficientAndShininess.w / 128 ).xyz;
    tmp = texture( uSpecularIBLShinyShadowedTexture, ref.xyz );
    _tmp2 = texture( uSpecularIBLRoughShadowedTexture, ref.xyz );
    tmp.xyz = mix( _tmp2, tmp, uFresnelCoefficientAndShininess.w / 128 ).xyz;
    spec.xyz = mix( tmp, spec, lc.z ).xyz;

    spec.xyz = spec.xyz * uCharaLight.Specular.xyz;
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

    diff.xyz = diff.xyz + luce.w;
    diff.xyz = diff.xyz;

    return vec4( diff.xyz, col0.w );
}

#endif