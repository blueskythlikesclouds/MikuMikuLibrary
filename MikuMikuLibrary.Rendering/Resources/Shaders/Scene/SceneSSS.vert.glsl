#version 330

#include "SceneCommon.glsl"

layout ( location = 0 ) in vec3 aPosition;
layout ( location = 1 ) in vec3 aNormal;
layout ( location = 3 ) in vec2 aTexCoord0;

out vec3 fPosition;
out vec3 fNormal;
out vec3 fTexCoordAndLucency;
out vec4 fShadowMapCoord;

void main()
{
    fPosition = ( uModel * vec4( aPosition, 1.0 ) ).xyz;
    fNormal = ( uModel * vec4( aNormal, 0.0 ) ).xyz;

    fTexCoordAndLucency.xy = ( uDiffuseTransformation * vec4( aTexCoord0.xy, 0, 0 ) ).xy;

    vec3 posViewSpace = ( uView * vec4( fPosition, 1.0 ) ).xyz;
    fTexCoordAndLucency.z = computeLucency( uCharaLight.Direction, fNormal, normalize( posViewSpace ) );

    fShadowMapCoord = uLightViewProjection * vec4( fPosition, 1.0 );
    fShadowMapCoord.xyz = ( fShadowMapCoord.xyz + fShadowMapCoord.w ) * 0.5;

    gl_Position = uProjection * vec4( posViewSpace, 1.0 );
}