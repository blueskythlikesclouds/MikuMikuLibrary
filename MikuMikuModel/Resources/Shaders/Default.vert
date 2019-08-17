#version 330

layout ( location = 0 ) in vec3 aPosition;
layout ( location = 1 ) in vec3 aNormal;
layout ( location = 2 ) in vec4 aTangent;
layout ( location = 3 ) in vec2 aTexCoord1;
layout ( location = 4 ) in vec2 aTexCoord2;
layout ( location = 5 ) in vec4 aColor;

uniform mat4 uView;
uniform mat4 uProjection;

out vec3 fPosition;
out vec3 fNormal;
out vec3 fTangent;
out vec3 fBitangent;
out vec2 fTexCoord1;
out vec2 fTexCoord2;
out vec4 fColor;

void main()
{
    fPosition = aPosition;
    fNormal = aNormal;
    fTangent = aTangent.xyz;
    fBitangent = normalize( cross( fNormal, fTangent ) * aTangent.w );
    fTexCoord1 = aTexCoord1;
    fTexCoord2 = aTexCoord2;
    fColor = aColor;

    gl_Position = uProjection * uView * vec4( aPosition, 1.0 );
}