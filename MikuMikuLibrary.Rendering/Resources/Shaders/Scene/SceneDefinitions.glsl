#ifndef SCENE_DEFINITIONS_GLSL_INCLUDED
#define SCENE_DEFINITIONS_GLSL_INCLUDED

#define SHADER_FLAGS_DIFFUSE                   ( 1 << 0 )   
#define SHADER_FLAGS_AMBIENT                   ( 1 << 1 )   
#define SHADER_FLAGS_NORMAL                    ( 1 << 2 )   
#define SHADER_FLAGS_SPECULAR                  ( 1 << 3 )   
#define SHADER_FLAGS_TRANSPARENCY              ( 1 << 4 )   
#define SHADER_FLAGS_ENVIRONMENT               ( 1 << 5 )   
#define SHADER_FLAGS_TOON_CURVE                ( 1 << 6 )
#define SHADER_FLAGS_ANISO_DIRECTION_NORMAL    ( 1 << 7 )
#define SHADER_FLAGS_ANISO_DIRECTION_U         ( 1 << 8 )
#define SHADER_FLAGS_ANISO_DIRECTION_V         ( 1 << 9 )
#define SHADER_FLAGS_ANISO_DIRECTION_RADIAL    ( 1 << 10 )
#define SHADER_FLAGS_PUNCH_THROUGH             ( 1 << 11 )

#define SHADER_NAME_BLINN                      0
#define SHADER_NAME_CLOTH                      1
#define SHADER_NAME_EYEBALL                    2
#define SHADER_NAME_FLOOR                      3
#define SHADER_NAME_HAIR                       4
#define SHADER_NAME_ITEM                       5
#define SHADER_NAME_PUDDLE                     6
#define SHADER_NAME_SKIN                       7
#define SHADER_NAME_SKY                        8
#define SHADER_NAME_STAGE                      9
#define SHADER_NAME_TIGHTS                     10
#define SHADER_NAME_WATER01                    11

#define saturate( x )                          clamp( x, 0, 1 )

layout ( std140 ) uniform CameraData
{
    mat4 uView;
    mat4 uProjection;
    mat4 uLightViewProjection;
    vec3 uViewPosition;
};

struct IBLData
{
    mat4 IrradianceR;
    mat4 IrradianceG;
    mat4 IrradianceB;
    mat4 IBLSpace;

    vec3 FrontLight;
    vec3 BackLight;
};

struct LightData
{
    vec3 Direction;
    vec4 Diffuse;
    vec4 Ambient;
    vec4 Specular;
};

layout ( std140 ) uniform SceneData
{
    IBLData uIBL;
    LightData uCharaLight;
    LightData uStageLight;
};

layout ( std140 ) uniform MaterialData
{
    mat4 uDiffuseTransformation;
    mat4 uAmbientTransformation;

    vec4 uDiffuse;
    vec4 uAmbient;
    vec4 uSpecular;
    vec4 uEmission;

    vec4 uFresnelCoefficientAndShininess;

    int uShaderFlags;
    int uShaderName;
};

uniform sampler2D uDiffuseTexture;
uniform sampler2D uAmbientTexture;
uniform sampler2D uNormalTexture;
uniform sampler2D uSpecularTexture;
uniform samplerCube uEnvironmentTexture;
uniform sampler2D uTransparencyTexture;
uniform sampler2D uToonCurveTexture;

uniform sampler2D uShadowMapTexture;
uniform sampler2D uSSSTexture;

uniform samplerCube uDiffuseIBLTexture;
uniform samplerCube uDiffuseIBLShadowedTexture;
uniform samplerCube uSpecularIBLShinyTexture;
uniform samplerCube uSpecularIBLRoughTexture;    
uniform samplerCube uSpecularIBLShinyShadowedTexture;
uniform samplerCube uSpecularIBLRoughShadowedTexture;

uniform mat4 uModel;

#endif