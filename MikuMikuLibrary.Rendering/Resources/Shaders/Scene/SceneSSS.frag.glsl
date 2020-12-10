#version 330

#include "SceneCommon.glsl"

in vec3 fPosition;
in vec3 fNormal;
in vec3 fTexCoordAndLucency;
in vec4 fShadowMapCoord;

out vec4 oColor;

vec3 SSS_SKIN()
{
    vec4 _tmp0, _tmp1, _tmp2;
    vec4 normal, eye;
    vec4 org_normal, org_eye;
    vec4 lc, diff, luce;
    vec4 col0, tmp;

    if ( ( uShaderFlags & SHADER_FLAGS_DIFFUSE ) != 0 )
        col0 = texture( uDiffuseTexture, fTexCoordAndLucency.xy );

    else
        col0 = uDiffuse;

    normal.xyz = normalize( fNormal );
    org_normal = normal;
    normal.x = dot( uIBL.IBLSpace[0].xyz, org_normal.xyz );
    normal.y = dot( uIBL.IBLSpace[1].xyz, org_normal.xyz );
    normal.z = dot( uIBL.IBLSpace[2].xyz, org_normal.xyz );

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

    if ( ( uShaderFlags & SHADER_FLAGS_SPECULAR ) != 0 )
        tmp = texture( uSpecularTexture, fTexCoordAndLucency.xy );

    else
        tmp = uSpecular;

    lc.z = lc.z * tmp.w;
    lc.w = tmp.w;
    tmp.x = lc.z * 0.7 + 0.3;
    tmp.x = tmp.x * fTexCoordAndLucency.z;
    luce.xyz = ( uIBL.FrontLight * uCharaLight.Specular.xyz * LUCE_COEF ) * tmp.x;
    luce.xyz = luce.xyz * vec3( 1, 0.90, 1.0 );
    diff = texture( uDiffuseIBLTexture, normal.xyz );
    tmp = texture( uDiffuseIBLShadowedTexture, normal.xyz );
    diff = mix( tmp, diff, lc.y );
    diff.xyz = diff.xyz * uCharaLight.Diffuse.xyz;
    diff.xyz = diff.xyz + uCharaLight.Ambient.xyz;
    diff.xyz = diff.xyz + luce.xyz;
    diff.xyz = diff.xyz/* + a_color0.w*/;
    tmp = vec4( 0 );
    tmp.xyz = tmp.xyz;
    tmp.w = float( lc.w > 0.99 );
    diff.xyz = tmp.xyz * tmp.w + diff.xyz;

    return diff.xyz;
}

void main()
{
    switch ( uShaderName )
    {
        case SHADER_NAME_SKIN:
        case SHADER_NAME_EYEBALL:
            oColor = vec4( SSS_SKIN(), 1.0 );
            break;
        
        default:
            //oColor = vec4( vec3( 0.6 ), 0.01 );
            //break;
            discard;
    }
}