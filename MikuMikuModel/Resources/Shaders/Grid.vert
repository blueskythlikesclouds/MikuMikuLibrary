#version 330

layout ( location = 0 ) in vec4 aPosition;

out vec3 fPosition;
out vec3 fColor;

uniform mat4 uView;
uniform mat4 uProjection;

uniform vec4 uInnerColor;
uniform vec4 uOuterColor;
uniform vec4 uXColor;
uniform vec4 uZColor;

void main()
{
    fPosition = ( uView * vec4( aPosition.xyz, 1.0 ) ).xyz;

    switch ( int( aPosition.w ) )
    {
        case 0:
            fColor = uZColor.rgb;
            break;

        case 1:
            fColor = uXColor.rgb;
            break;

        case 2:
            fColor = uInnerColor.rgb;
            break;

        default:
            fColor = uOuterColor.rgb;
            break;
    }

    gl_Position = uProjection * vec4( fPosition, 1.0 );
}