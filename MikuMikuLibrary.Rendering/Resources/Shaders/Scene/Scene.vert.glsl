#version 330

#define ATTRIB_TOKEN out
#include "SceneAttributes.glsl"

#include "SceneCommon.glsl"

layout ( location = 0 ) in vec3 aPosition;
layout ( location = 1 ) in vec3 aNormal;
layout ( location = 2 ) in vec4 aTangent;
layout ( location = 3 ) in vec2 aTexCoord0;
layout ( location = 4 ) in vec2 aTexCoord1;
layout ( location = 5 ) in vec2 aTexCoord2;
layout ( location = 6 ) in vec2 aTexCoord3;
layout ( location = 7 ) in vec4 aColor0;
layout ( location = 8 ) in vec4 aColor1;

void main()
{
    fPosition = ( uModel * vec4( aPosition, 1.0 ) ).xyz;
    fNormal = ( uModel * vec4( aNormal, 0.0 ) ).xyz;
    fTangent = ( uModel * vec4( aTangent.xyz, 0.0 ) ).xyz;
    fBitangent = ( uModel * vec4( cross( aNormal, aTangent.xyz ) * aTangent.w, 0.0 ) ).xyz;
    fTexCoord.xy = ( uDiffuseTransformation * vec4( aTexCoord0.xy, 0, 0 ) ).xy;
    fTexCoord.zw = ( uAmbientTransformation * vec4( aTexCoord1.xy, 0, 0 ) ).xy;
    fColor = aColor0;

    fShadowMapCoord = uLightViewProjection * vec4( fPosition, 1.0 );
    fShadowMapCoord.xyz = ( fShadowMapCoord.xyz + fShadowMapCoord.w ) * 0.5;

    fAnisoTangentAndLucency.xyz = computeAnisoTangent( fNormal, fTangent, fBitangent, fTexCoord.xy );

    vec3 posViewSpace = ( uView * vec4( fPosition, 1.0 ) ).xyz;
    fAnisoTangentAndLucency.w = computeLucency( uCharaLight.Direction, fNormal, normalize( posViewSpace ) );

    gl_Position = uProjection * vec4( posViewSpace, 1.0 );
    fFragCoord = gl_Position.xy / gl_Position.w * 0.5 + 0.5;
}