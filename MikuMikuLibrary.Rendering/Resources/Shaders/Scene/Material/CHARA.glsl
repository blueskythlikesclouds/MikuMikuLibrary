#version 330

#ifndef CHARA_GLSL_INCLUDED
#define CHARA_GLSL_INCLUDED

#ifndef SCENE_ATTRIBUTES_GLSL_INCLUDED

#define ATTRIB_TOKEN in
#include "../SceneAttributes.glsl"

#endif

#include "../SceneCommon.glsl"

vec4 CHARA()
{
    vec4 diffColor = texture( uDiffuseTexture, fTexCoord.xy );
    vec4 specColor = texture( uDiffuseTexture, fTexCoord.xy );

    vec3 lightDir = normalize( -uCharaLight.Direction );
    vec3 viewDir = normalize( uViewPosition - fPosition );
    
    vec3 halfwayDir;

    if ( uShaderName == SHADER_NAME_HAIR )
        halfwayDir = viewDir + vec3( 0, 1, 0 );

    else
        halfwayDir = viewDir + lightDir;

    halfwayDir = normalize( halfwayDir );

    vec3 normal = normalize( fNormal );

    float diffCosTheta = dot( normal, lightDir );
	float specCosTheta = dot( normal, halfwayDir );
	float fresCosTheta = dot( normal, viewDir );

    float toonDiffRampPos = ( diffCosTheta + 1 ) / 2;
	float toonSpecRampPos = saturate( specCosTheta );
	float toonFresRampPos = saturate( 1 - fresCosTheta );

    vec3 toonDiffColor = texture( uToonCurveTexture, vec2( toonDiffRampPos, 0.875f ) ).rgb;
	vec3 toonSpecColor = texture( uToonCurveTexture, vec2( toonSpecRampPos, 0.625f ) ).rgb;
	vec3 toonFresColor = texture( uToonCurveTexture, vec2( toonFresRampPos, 0.375f ) ).rgb;

    vec3 diffLighting = toonDiffColor + uEmission.rgb;
    vec3 specLighting = ( 1 - pow( saturate( 1 - diffCosTheta ), 8 ) ) * toonSpecColor;
    vec3 fresLighting = specColor.w * toonFresColor;

    vec3 finalColor = 
		diffColor.rgb * diffColor.rgb * diffLighting + 
		specColor.rgb * specColor.rgb * specLighting + fresLighting;

    return vec4( finalColor, diffColor.a );
}

#endif