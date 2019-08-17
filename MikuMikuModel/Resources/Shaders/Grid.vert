#version 330

layout ( location = 0 ) in vec3 aPosition;

uniform mat4 uView;
uniform mat4 uProjection;

void main()
{
    gl_Position = uProjection * uView * vec4( aPosition, 1.0 );
}