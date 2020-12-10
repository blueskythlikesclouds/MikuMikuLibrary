#version 330

in vec2 fTexCoord;

uniform vec2 uResolution;
uniform vec2 uDirection;

uniform sampler2D uTexture;

void main()
{
  // TODO: Implement the actual filtering algorithm.

  float depth = 0.0;

  vec2 offset1 = vec2( 1.411764705882353 ) * uDirection;
  vec2 offset2 = vec2( 3.2941176470588234 ) * uDirection;
  vec2 offset3 = vec2( 5.176470588235294 ) * uDirection;

  depth += texture( uTexture, fTexCoord ).x * 0.1964825501511404;
  depth += texture( uTexture, fTexCoord + ( offset1 * uResolution ) ).x * 0.2969069646728344;
  depth += texture( uTexture, fTexCoord - ( offset1 * uResolution ) ).x * 0.2969069646728344;
  depth += texture( uTexture, fTexCoord + ( offset2 * uResolution ) ).x * 0.09447039785044732;
  depth += texture( uTexture, fTexCoord - ( offset2 * uResolution ) ).x * 0.09447039785044732;
  depth += texture( uTexture, fTexCoord + ( offset3 * uResolution ) ).x * 0.010381362401148057;
  depth += texture( uTexture, fTexCoord - ( offset3 * uResolution ) ).x * 0.010381362401148057;

  gl_FragDepth = depth;
}