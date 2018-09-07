#version 330

out vec4 outColor;

in vec3 position;
in vec3 normal;
in vec2 texCoord;
in vec4 color;

uniform bool hasNormal;
uniform bool hasTexCoord;
uniform bool hasColor;
uniform bool hasDiffuseTexture;
uniform sampler2D diffuseTexture;
uniform vec4 diffuseColor;
uniform vec3 lightPosition;

void main()
{
	vec3 lightDirection = normalize( lightPosition - position );
	vec4 gamma = vec4( vec3( 2.2 ), 1 );

	//
	// Diffuse 
	//
	vec4 diffuse = diffuseColor;

	if ( hasDiffuseTexture && hasTexCoord ) 
	{
		diffuse *= pow( texture2D( diffuseTexture, texCoord ), gamma );
	}

	if ( hasNormal )
	{
		diffuse.xyz *= clamp( dot( normal, lightDirection ), 0, 1 );
	}

	if ( hasColor )
	{
		diffuse *= color;
	}

	if ( diffuse.a < 0.1 )
	{
		discard;
	}	

	outColor = diffuse;
}