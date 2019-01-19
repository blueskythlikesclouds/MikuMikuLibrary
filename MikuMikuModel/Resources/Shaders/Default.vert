#version 330

layout ( location = 0 ) in vec3 aPosition;
layout ( location = 1 ) in vec3 aNormal;
layout ( location = 2 ) in vec4 aTangent;
layout ( location = 3 ) in vec2 aTexCoord;
layout ( location = 4 ) in vec2 aTexCoord2;
layout ( location = 5 ) in vec4 aColor;


uniform mat4 view;
uniform mat4 projection;
uniform bool hasTangent;

out vec3 position;
out vec3 normal;
out vec3 tangent;
out vec3 bitangent;
out vec2 texCoord;
out vec2 texCoord2;
out vec4 color;

void main()
{
	// Pass the attributes
	position = aPosition;
	normal = aNormal;
	texCoord = aTexCoord;
	texCoord2 = aTexCoord2;
	color = aColor;

	if ( hasTangent )
	{
		tangent = aTangent.xyz * -aTangent.w;
		bitangent = normalize( cross( aTangent.xyz, aNormal ) );
	}

	// Pass our position
	gl_Position = projection * view * vec4( aPosition, 1.0 );
}