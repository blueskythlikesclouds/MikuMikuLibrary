#version 330

layout ( location = 0 ) in vec3 aPosition;
layout ( location = 1 ) in vec3 aNormal;
layout ( location = 2 ) in vec4 aTangent;
layout ( location = 3 ) in vec2 aTexCoord0;
layout ( location = 4 ) in vec2 aTexCoord1;
layout ( location = 5 ) in vec4 aColor0;

uniform mat4 uView;
uniform mat4 uProjection;

out vec3 fPosition;
out vec3 fNormal;
out vec3 fTangent;
out vec3 fBitangent;
out vec2 fTexCoord0;
out vec2 fTexCoord1;
out vec4 fColor0;

void main()
{
    fPosition = aPosition;
    fNormal = aNormal;
    fTangent = normalize( aTangent.xyz - dot( aTangent.xyz, aNormal ) * aNormal );
    fBitangent = cross( fNormal, fTangent ) * aTangent.w;
    fTexCoord0 = aTexCoord0;
    fTexCoord1 = aTexCoord1;
    fColor0 = aColor0;

    gl_Position = uProjection * uView * vec4( aPosition, 1.0 );
}