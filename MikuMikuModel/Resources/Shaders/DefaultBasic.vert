#version 330

layout ( location = 0 ) in vec3 aPosition;
layout ( location = 1 ) in vec3 aNormal;
layout ( location = 3 ) in vec2 aTexCoord;
layout ( location = 5 ) in vec4 aColor;


uniform mat4 view;
uniform mat4 projection;

out vec3 position;
out vec3 normal;
out vec2 texCoord;
out vec4 color;

void main()
{
	// Pass the attributes
	position = aPosition;
	normal = aNormal;
	texCoord = aTexCoord;
	color = aColor;

	// Pass our position
	gl_Position = projection * view * vec4( aPosition, 1.0 );
}