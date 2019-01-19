#version 330

out vec4 outColor;

//
// Vertex Attributes
//
in vec3 position;
in vec3 normal;
in vec3 tangent;
in vec3 bitangent;
in vec2 texCoord;
in vec2 texCoord2;
in vec4 color;

uniform bool hasNormal;
uniform bool hasTexCoord;
uniform bool hasTexCoord2;
uniform bool hasColor;
uniform bool hasTangent;

//
// Textures
//
uniform bool hasDiffuseTexture;
uniform bool hasAmbientTexture;
uniform bool hasNormalTexture;
uniform bool hasSpecularTexture;
uniform bool hasReflectionTexture;
uniform bool hasSpecularPowerTexture;

uniform sampler2D diffuseTexture;
uniform sampler2D ambientTexture;
uniform sampler2D normalTexture;
uniform sampler2D specularTexture;
uniform samplerCube reflectionTexture;
uniform sampler2D specularPowerTexture;

uniform vec4 diffuseColor;
uniform vec4 ambientColor;
uniform vec4 specularColor;
uniform float specularPower;

//
// Misc
//
uniform vec3 viewPosition;
uniform vec3 lightPosition;

void main()
{
	vec3 lightDirection = normalize( lightPosition - position );
	vec3 viewDirection = normalize( viewPosition - position );
	vec4 gamma = vec4( vec3( 2.2 ), 1 );
	vec3 nrm = normal;

	//
	// Ambient
	//
	vec4 ambient = ambientColor;
	if ( hasAmbientTexture && hasTexCoord && !hasTexCoord2 )
	{
		ambient *= pow( texture2D( ambientTexture, texCoord ), gamma );
	}
	else if ( hasDiffuseTexture && hasTexCoord && !hasTexCoord2 )
	{
		ambient *= pow( texture2D( diffuseTexture, texCoord ), gamma ) * 0.3;
	}

	//
	// Diffuse 
	//
	vec4 diffuse = diffuseColor;

	if ( hasDiffuseTexture && hasTexCoord ) 
	{
		diffuse *= pow( texture2D( diffuseTexture, texCoord ), gamma );
	}

	if ( hasColor )
	{
		diffuse *= color;
	}

	if ( hasNormal )
	{
		if ( hasTangent && hasNormalTexture && hasTexCoord )
		{
			nrm = 2 * texture2D( normalTexture, texCoord ).rgb - 1;

			// Calculate the Z axis as it doesn't exist in ATI2 textures.
			nrm.z = sqrt( 1 - ( nrm.x * nrm.x ) - ( nrm.y * nrm.y ) );

			// Convert normal from tangent space to model space
			nrm *= transpose( mat3( tangent, bitangent, normal ) );
		}

		diffuse.xyz *= clamp( dot( nrm, lightDirection ), 0, 1 );
	}

	//
	// Specular
	//
	vec4 specular = specularColor;

	if ( hasNormal )
	{
		if ( hasSpecularTexture && hasTexCoord )
		{
			specular *= texture2D( specularTexture, texCoord );
		}

		vec4 power = vec4( specularPower );
		if ( hasSpecularPowerTexture && hasTexCoord )
		{
			power *= texture2D( specularPowerTexture, texCoord );
		}

		if ( dot( nrm, lightDirection ) > 0 )
		{
			specular.xyz *= pow( vec4( dot( nrm, normalize( lightDirection + viewDirection ) ) ), power ).xyz;
		}

		else
		{
			specular.xyz = vec3( 0 );
		}
	}

	//
	// The Final Mix ;)
	//
	vec4 color = vec4( ( ambient + diffuse + specular ).xyz, diffuse.w );


	//
	// Reflection
	//
	if ( hasReflectionTexture )
	{
		vec3 reflection = reflect( -viewDirection, nrm );
		color.xyz += pow( texture( reflectionTexture, reflection ), gamma ).xyz * specularColor.w * 0.05;
	}

	//
	// Lightmap
	// 
	if ( hasAmbientTexture && hasTexCoord2 )
	{
		color *= pow( texture2D( ambientTexture, texCoord2 ), gamma );
	}

	if ( color.a < 0.1 )
	{
		discard;
	}	

	outColor = color;
}