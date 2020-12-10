#version 330

#ifndef SCENE_ATTRIBUTES_GLSL_INCLUDED
#define SCENE_ATTRIBUTES_GLSL_INCLUDED

#ifdef ATTRIB_TOKEN

ATTRIB_TOKEN vec3 fPosition;
ATTRIB_TOKEN vec3 fNormal;
ATTRIB_TOKEN vec3 fTangent;
ATTRIB_TOKEN vec3 fBitangent;
ATTRIB_TOKEN vec4 fTexCoord;
ATTRIB_TOKEN vec4 fColor;
ATTRIB_TOKEN vec4 fShadowMapCoord;
ATTRIB_TOKEN vec4 fAnisoTangentAndLucency;
ATTRIB_TOKEN vec2 fFragCoord;

#endif

#endif