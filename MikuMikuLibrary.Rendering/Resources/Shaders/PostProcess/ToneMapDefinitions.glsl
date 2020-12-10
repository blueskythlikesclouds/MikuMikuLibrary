layout ( std140 ) uniform ToneMapData
{
    vec4 uExposure;
    vec4 uToneOffset;
    vec4 uToneScale;
    vec4 uFadeColorAndFunc;
};