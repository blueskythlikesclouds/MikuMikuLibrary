#version 330

layout ( location = 0 ) in vec2 aPosition;

out vec2 fTexCoord;

void main()
{
    fTexCoord = aPosition.xy * 0.5 + 0.5;
    gl_Position = vec4( aPosition.xy, 0, 1 );
}