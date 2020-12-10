#version 330

layout ( location = 0 ) out vec4 oColor;

in vec2 fTexCoord;

uniform vec4[ 7 ] uGaussianKernel;
uniform vec2[ 6 ] uDirection;
uniform sampler2D uTexture;

void main()
{
    vec4 color0 = vec4( 0.0 );
    vec4 color1;

    color1 = texture( uTexture, fTexCoord                  ); color0 += uGaussianKernel[ 0 ] * color1;
    color1 = texture( uTexture, fTexCoord + uDirection[ 0 ]); color0 += uGaussianKernel[ 1 ] * color1;
    color1 = texture( uTexture, fTexCoord - uDirection[ 0 ]); color0 += uGaussianKernel[ 1 ] * color1;
    color1 = texture( uTexture, fTexCoord + uDirection[ 1 ]); color0 += uGaussianKernel[ 2 ] * color1;
    color1 = texture( uTexture, fTexCoord - uDirection[ 1 ]); color0 += uGaussianKernel[ 2 ] * color1;
    color1 = texture( uTexture, fTexCoord + uDirection[ 2 ]); color0 += uGaussianKernel[ 3 ] * color1;
    color1 = texture( uTexture, fTexCoord - uDirection[ 2 ]); color0 += uGaussianKernel[ 3 ] * color1;
    color1 = texture( uTexture, fTexCoord + uDirection[ 3 ]); color0 += uGaussianKernel[ 4 ] * color1;
    color1 = texture( uTexture, fTexCoord - uDirection[ 3 ]); color0 += uGaussianKernel[ 4 ] * color1;
    color1 = texture( uTexture, fTexCoord + uDirection[ 4 ]); color0 += uGaussianKernel[ 5 ] * color1;
    color1 = texture( uTexture, fTexCoord - uDirection[ 4 ]); color0 += uGaussianKernel[ 5 ] * color1;
    color1 = texture( uTexture, fTexCoord + uDirection[ 5 ]); color0 += uGaussianKernel[ 6 ] * color1;
    color1 = texture( uTexture, fTexCoord - uDirection[ 5 ]); color0 += uGaussianKernel[ 6 ] * color1;

    oColor = color0;
}
