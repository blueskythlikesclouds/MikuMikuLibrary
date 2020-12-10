#version 330

#include "Material/CHARA.glsl"
#include "Material/CLOTH.glsl"
#include "Material/HAIR.glsl"
#include "Material/ITEM.glsl"
#include "Material/SKIN.glsl"

out vec4 oColor;

void main()
{
    if ( ( uShaderFlags & SHADER_FLAGS_TOON_CURVE ) != 0 )
        oColor = CHARA();

    else
    {
        switch ( uShaderName )
        {
            case SHADER_NAME_CLOTH:
                oColor = CLOTH();
                break;

            case SHADER_NAME_ITEM:
                oColor = ITEM();
                break;

            case SHADER_NAME_HAIR:
                oColor = HAIR();
                break;           
                
            case SHADER_NAME_SKIN:
                oColor = SKIN();
                break;

            case SHADER_NAME_BLINN:
            case SHADER_NAME_EYEBALL:
            case SHADER_NAME_FLOOR:
            case SHADER_NAME_PUDDLE:
            case SHADER_NAME_SKY:
            case SHADER_NAME_STAGE:
            case SHADER_NAME_TIGHTS:
            case SHADER_NAME_WATER01:
            default:
                oColor = vec4( 1, 0, 0, 1 );
                break;
        }
    }

    if ( ( uShaderFlags & SHADER_FLAGS_PUNCH_THROUGH ) != 0 && oColor.a < 0.5 )
        discard;
}