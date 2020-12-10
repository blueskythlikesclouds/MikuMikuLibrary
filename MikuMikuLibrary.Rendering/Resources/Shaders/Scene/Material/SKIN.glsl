#version 330

#ifndef SKIN_GLSL_INCLUDED
#define SKIN_GLSL_INCLUDED

#ifndef SCENE_ATTRIBUTES_GLSL_INCLUDED

#define ATTRIB_TOKEN in
#include "../SceneAttributes.glsl"

#endif

#include "../SceneCommon.glsl"

vec4 SKIN()
{
    vec4 _tmp0, _tmp1, _tmp2;
    vec4 dev_pos;
    vec4 normal, ref, eye, halfway;
    vec4 org_normal, org_eye;
    vec4 lc, spec, diff, luce, env;
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
    normal.x = dot( uIBL.IBLSpace[0].xyz, org_normal.xyz );
    normal.y = dot( uIBL.IBLSpace[1].xyz, org_normal.xyz );
    normal.z = dot( uIBL.IBLSpace[2].xyz, org_normal.xyz );
    eye.x = dot( uIBL.IBLSpace[0].xyz, org_eye.xyz );
    eye.y = dot( uIBL.IBLSpace[1].xyz, org_eye.xyz );
    eye.z = dot( uIBL.IBLSpace[2].xyz, org_eye.xyz );
    tmp.x = dot( org_eye.xyz, org_normal.xyz );
    tmp.x = tmp.x * 2.0;
    tmp.xyz = tmp.x * org_normal.xyz + -org_eye.xyz;
    ref.x = dot( uIBL.IBLSpace[0].xyz, tmp.xyz );
    ref.y = dot( uIBL.IBLSpace[1].xyz, tmp.xyz );
    ref.z = dot( uIBL.IBLSpace[2].xyz, tmp.xyz );
    halfway = mix( eye, normal, 1.25 );
    halfway.xyz = normalize( halfway.xyz );
    tmp.x = dot( halfway.xyz, eye.xyz );
    tmp.x = saturate( 1 - tmp.x );
    tmp.x = pow( tmp.x, 5 );
    spec_ratio.x = tmp.x * 0.20 + 0.022;
    spec_ratio.w = saturate( dot( normal.xyz, eye.xyz ) );
    ndote.x = spec_ratio.w;
    spec_ratio.w = pow( spec_ratio.w, 0.5 );
    spec_ratio.w = spec_ratio.w * 0.75 + 0.25;

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

    tmp = texture( uSpecularTexture, fTexCoord.xy );
    lc.z = lc.z * tmp.w;
    lc.w = tmp.w;
    diff = texture( uDiffuseIBLTexture, normal.xyz );
    tmp = texture( uDiffuseIBLShadowedTexture, normal.xyz );
    diff = mix( tmp, diff, lc.y );
    diff.xyz = diff.xyz * uCharaLight.Diffuse.xyz;
    diff.xyz = diff.xyz/* + a_color0.w*/;
    diff.xyz = diff.xyz + uCharaLight.Ambient.xyz;
    tmp = vec4( 0 );
    tmp.xyz = tmp.xyz;
    tmp.w = float( lc.w > 0.99 );
    diff.xyz = tmp.xyz * tmp.w + diff.xyz;
    luce = texture( uSSSTexture, fFragCoord );
    tmp.x = SSS_COEF * spec_ratio.w;
    diff = mix( diff, luce, tmp.x );
    diff.xyz = diff.xyz * col0.xyz;
    lc.w = 0.1;
    spec = texture( uSpecularIBLShinyTexture, ref.xyz );
    _tmp2 = texture( uSpecularIBLRoughTexture, ref.xyz );
    spec.xyz = mix( _tmp2, spec, lc.w ).xyz;
    tmp = texture( uSpecularIBLShinyShadowedTexture, ref.xyz );
    _tmp2 = texture( uSpecularIBLRoughShadowedTexture, ref.xyz );
    tmp.xyz = mix( _tmp2, tmp, lc.w ).xyz;
    spec.xyz = mix( tmp, spec, lc.z ).xyz;
    spec.xyz = spec.xyz * uCharaLight.Specular.xyz;
    tmp.x = lc.z * 0.7 + 0.3;
    spec.xyz = spec.xyz * tmp.x;
    spec_ratio.xyz = vec3( spec_ratio.x );
    spec_ratio.xyz = spec_ratio.xyz;
    diff.xyz = diff.xyz * 0.98;
    diff.xyz = spec.xyz * spec_ratio.xyz + diff.xyz;
    diff.xyz = diff.xyz;

    return vec4( diff.xyz, col0.w );
}

#endif