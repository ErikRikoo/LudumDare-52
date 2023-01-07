TEXTURE2D(_CameraColorTexture);
SAMPLER(sampler_CameraColorTexture);
float4 _CameraColorTexture_TexelSize;
//
TEXTURE2D(_TransparentDiscontinuityTexture);
SAMPLER(sampler_TransparentDiscontinuityTexture);
//
TEXTURE2D(_TransparentBackfaceDiscontinuityTexture);
SAMPLER(sampler_TransparentBackfaceDiscontinuityTexture);

TEXTURE2D(_CameraDepthTexture);
SAMPLER(sampler_CameraDepthTexture);

TEXTURE2D(_DiscontinuityTexture);
SAMPLER(sampler_DiscontinuityTexture);

#ifndef SHADERGRAPH_PREVIEW
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
#endif
 
float3 DecodeNormal(float4 enc)
{
    float kScale = 1.7777;
    float3 nn = enc.xyz*float3(2*kScale,2*kScale,0) + float3(-kScale,-kScale,1);
    float g = 2.0 / dot(nn.xyz,nn.xyz);
    float3 n;
    n.xy = g*nn.xy;
    n.z = g-1;
    return n;
}

void Outline_float(float2 UV, float OutlineThickness, float discSensitivity, float transparentDiscSensitivity, float DepthSensitivity, float TransparentDepthSensivity, float NormalsSensitivity, float ColorSensitivity, float4 OutlineColor, out float4 Out)
{
    float halfScaleFloor = floor(OutlineThickness * 0.5);
    float halfScaleCeil = ceil(OutlineThickness * 0.5);
    float2 Texel = (1.0) / float2(_CameraColorTexture_TexelSize.z, _CameraColorTexture_TexelSize.w);

    float2 uvSamples[4];
    float depthSamples[4];
    float3 normalSamples[4], colorSamples[4];
    float4 discontinuitySamples[4];
    float3 transparentNormalSamples[4];
    float4 transparentDepthSamples[4];
    float3 transparentBackfaceNormalSamples[4];
    float4 transparentBackfaceDepthSamples[4];

    uvSamples[0] = UV - float2(Texel.x, Texel.y) * halfScaleFloor;
    uvSamples[1] = UV + float2(Texel.x, Texel.y) * halfScaleCeil;
    uvSamples[2] = UV + float2(Texel.x * halfScaleCeil, -Texel.y * halfScaleFloor);
    uvSamples[3] = UV + float2(-Texel.x * halfScaleFloor, Texel.y * halfScaleCeil);
    
    for(int i = 0; i < 4 ; i++)
    {
        discontinuitySamples[i] = SAMPLE_TEXTURE2D(_DiscontinuityTexture, sampler_DiscontinuityTexture, uvSamples[i]);
        float4 transparentDisc = SAMPLE_TEXTURE2D(_TransparentDiscontinuityTexture, sampler_TransparentDiscontinuityTexture, uvSamples[i]);
        transparentNormalSamples[i] = transparentDisc.rgb;
        transparentDepthSamples[i] = transparentDisc.a;
        float4 transparentBackfaceDisc = SAMPLE_TEXTURE2D(_TransparentBackfaceDiscontinuityTexture, sampler_TransparentBackfaceDiscontinuityTexture, uvSamples[i]);
        transparentBackfaceNormalSamples[i] = transparentBackfaceDisc.rgb;
        transparentBackfaceDepthSamples[i] = transparentBackfaceDisc.a;
        depthSamples[i] = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uvSamples[i]).r;
        normalSamples[i] = SampleSceneNormals(uvSamples[i]);
        colorSamples[i] = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, uvSamples[i]);
    }

    float4 discontinuitySamplesOrig = SAMPLE_TEXTURE2D(_DiscontinuityTexture, sampler_DiscontinuityTexture, UV);
    
    // Custom Source
    const float discontinuityDifference0 = discontinuitySamples[1].r - discontinuitySamples[0].r;
    const float discontinuityDifference1 = discontinuitySamples[3].r - discontinuitySamples[2].r;
    float edgeDiscontinuity = sqrt(pow(discontinuityDifference0, 2) + pow(discontinuityDifference1, 2)) * 100;
    const float discontinuityThreshold = (1/discSensitivity) * discontinuitySamples[0];
    edgeDiscontinuity = edgeDiscontinuity > discontinuityThreshold ? 1 : 0;
    edgeDiscontinuity += discontinuitySamplesOrig.g > 0 ? 1 : 0;

    // Transparent Outlines
    float3 tNormalFiniteDifference0 = transparentNormalSamples[1] - transparentNormalSamples[0];
    float3 tNormalFiniteDifference1 = transparentNormalSamples[3] - transparentNormalSamples[2];
    float tEdgeNormal = sqrt(dot(tNormalFiniteDifference0, tNormalFiniteDifference0) + dot(tNormalFiniteDifference1, tNormalFiniteDifference1));
    tEdgeNormal = tEdgeNormal > (1/transparentDiscSensitivity) ? 1 : 0;

    // Transparent Depth
    float tDepthFiniteDifference0 = transparentDepthSamples[1] - transparentDepthSamples[0];
    float tDepthFiniteDifference1 = transparentDepthSamples[3] - transparentDepthSamples[2];
    float tEdgeDepth = sqrt(pow(tDepthFiniteDifference0, 2) + pow(tDepthFiniteDifference1, 2)) * 100;
    float tDepthThreshold = (1/TransparentDepthSensivity) * transparentDepthSamples[0];
    tEdgeDepth = tEdgeDepth > tDepthThreshold ? 1 : 0;

    // Transparent Backface Outlines
    float3 tNormalBackFiniteDifference0 = transparentBackfaceNormalSamples[1] - transparentBackfaceNormalSamples[0];
    float3 tNormalBackFiniteDifference1 = transparentBackfaceNormalSamples[3] - transparentBackfaceNormalSamples[2];
    float tEdgeBackfaceNormal = sqrt(dot(tNormalBackFiniteDifference0, tNormalBackFiniteDifference0) + dot(tNormalBackFiniteDifference1, tNormalBackFiniteDifference1));
    tEdgeBackfaceNormal = tEdgeBackfaceNormal > (1/transparentDiscSensitivity) ? 1 : 0;

    // Transparent Backface Depth
    float tDepthBackfaceFiniteDifference0 = transparentBackfaceDepthSamples[1] - transparentBackfaceDepthSamples[0];
    float tDepthBackfaceFiniteDifference1 = transparentBackfaceDepthSamples[3] - transparentBackfaceDepthSamples[2];
    float tEdgeBackfaceDepth = sqrt(pow(tDepthBackfaceFiniteDifference0, 2) + pow(tDepthBackfaceFiniteDifference1, 2)) * 100;
    float tDepthBackfaceThreshold = (1/TransparentDepthSensivity) * transparentBackfaceDepthSamples[0];
    tEdgeBackfaceDepth = tEdgeBackfaceDepth > tDepthBackfaceThreshold ? 1 : 0;
    
    // Depth
    float depthFiniteDifference0 = depthSamples[1] - depthSamples[0];
    float depthFiniteDifference1 = depthSamples[3] - depthSamples[2];
    float edgeDepth = sqrt(pow(depthFiniteDifference0, 2) + pow(depthFiniteDifference1, 2)) * 100;
    float depthThreshold = (1/DepthSensitivity) * depthSamples[0];
    edgeDepth = edgeDepth > depthThreshold ? 1 : 0;
    
    // // Normals
    float3 normalFiniteDifference0 = normalSamples[1] - normalSamples[0];
    float3 normalFiniteDifference1 = normalSamples[3] - normalSamples[2];
    float edgeNormal = sqrt(dot(normalFiniteDifference0, normalFiniteDifference0) + dot(normalFiniteDifference1, normalFiniteDifference1));
    edgeNormal = edgeNormal > (1/NormalsSensitivity) ? 1 : 0;

    // Color
     float3 colorFiniteDifference0 = colorSamples[1] - colorSamples[0];
     float3 colorFiniteDifference1 = colorSamples[3] - colorSamples[2];
     float edgeColor = sqrt(dot(colorFiniteDifference0, colorFiniteDifference0) + dot(colorFiniteDifference1, colorFiniteDifference1));
     edgeColor = edgeColor > (1/ColorSensitivity) ? 1 : 0;

    float edge = max(max(max(max(max(max(edgeDepth, max(edgeNormal, edgeColor)), edgeDiscontinuity), tEdgeNormal), tEdgeDepth), tEdgeBackfaceDepth),tEdgeBackfaceNormal);

    float4 original = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, uvSamples[0]);
    
    Out = (1 - edge) * original + edge * saturate(lerp(original, OutlineColor,  OutlineColor.a));
}