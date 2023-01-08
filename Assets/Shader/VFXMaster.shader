Shader "VFX/VFXMaster"
{
    Properties
    {
        [Header(Textures)]
        _MainTex ("Main texture", 2D) = "white" {}
        _MainTexProperties("Main texture properties (XY Panning, ZW Contrast & Power", Vector) = (0,0,1,1)

        [Toggle(SECONDARY_TEX)]
        _UseSecondaryTex("Use secondary texture", float) = 0
        _SecondaryTex("Secondary texture", 2D) = "white" {} 
        _SecondaryTexProperties("Secondary texture properties (XY Panning, ZW Contrast & Power", Vector) = (0,0,1,1)

        [Toggle(EXTRA_PANNING)]
        _ExtraPanning("Extra panning", float) = 0
        
        [Enum(Multiply,0,Add,1,Min,2,Max,3)]
        _TextureBlending("Texture blending", int) = 0

        //Polar coordinates
        [Space(20)]
        [Toggle(POLAR)]
        _PolarCoords("Polar coordinates", float) = 0
        [Toggle(SWIRL)]
        _Swirl("Swirl", float) = 0
        _SwirlSpin("Swirl spin", float) = 1
        _SwirlSpeed("Swirl speed", float) = 0

        [Header(Color)]
        [HDR]_Color("Color", Color) = (1,1,1,1)
        [Toggle()]
        _MultiplyAlpha("Multiply color by alpha", float) = 0

        [Header(Gradient Map)]
        [Toggle(USE_GRADIENT_MAP)]
        _UseGradientMap("Use gradient map", float) = 0
        _ValuePower("Value power", float) = 1
        _GradientMap("Gradient map", 2D) = "white" {} 
        _GradientOffset("Gradient offset", Range(-1.0,1.0)) = 0

        [Header(Displacement)]
        [Toggle(DISPLACEMENT)]
        _Displacement("Displacement", float) = 0
        _DisplacementGuide("Displacement guide", 2D) = "white" {} 
        _DisplacementSpeedX("Displacement speed X", float) = 0
        _DisplacementSpeedY("Displacement speed Y", float) = 0
        _DisplacementAmount("Displacement amount", float) = 0

        [Header(Vertex offset)]
        [Toggle(VERTEX_OFFSET)]
        _VertexOffset("Vertex offset", float) = 0
        _VertexOffsetGuide("Vertex offset guide", 2D) = "white" {} 
        _VertexOffsetGuideSpeedX("Vertex offset guide speed X", float) = 0
        _VertexOffsetGuideSpeedY("Vertex offset guide speed Y", float) = 0
        _VertexOffsetAmount("Vertex offset amount", float) = 0
        [Toggle()]
        _InvertVertexOffsetGuide("Invert vertex offset guide", float) = 0
        

        [Header(Lighting)]
        [Toggle(LIGHTING)]
        _Lighting("Lighting", float) = 0
        _ShadowStrength("Shadow strength", Range(0,1)) = 0
        
        [Header(Fresnel Glow)]
        [Toggle(FRESNEL_GLOW)]
        _FresnelGlow("Fresnel glow", float) = 0
        [HDR]_FresnelGlowColor("Fresnel glow color", Color) = (0,0,0,1)
        _FresnelGlowThreshold("Fresnel glow threshold", Range(0, 1)) = 0.5
        _FresnelGlowSmoothness("Fresnel glow smoothness", range(0, 1)) = 0
        
        [Header(Cutoff)]
        _Cutoff("Cutoff", Range(0,1)) = 0
        _CutoffSmoothness("Cutoff smoothness", Range(0,1)) = 0

        [HDR]_BurnColor("Burn color", Color) = (1,1,1,1)
        _BurnSize("Burn size", Range(0,1)) = 0
        _BurnSoftness("Burn softness", Range(0,1)) = 0

        [Toggle()]
        _UseAlphaForDissolve("Use vertex alpha for dissolve", float) = 0

        [Header(Screen space texture)]
        [Toggle(SCREEN_SPACE_TEXTURE)]
        _UseScreenSpaceTexture("Screen space texture", float) = 0
        _ScreenSpaceTexture("Screen space texture", 2D) = "white" {}
        [HDR]_ScreenSpaceTextureColor("Screen space texture color", Color) = (1,1,1,1)

        //Banding
        [Space(20)]
        [Toggle(BANDING)]
        _Banding("Color banding", float) = 0
        _Bands("Number of bands", float) = 3

        //Stepped time
        [Header(Stepped Time)]
        [Toggle(STEPPED_TIME)]
        _SteppedTime("Stepped time", float) = 0
        _TimeStep("Time step", float) = 0.02

        [Header(Masks)]
        //Circle mask
        [Space(20)]
        [Toggle(CIRCLE_MASK)]
        _CircleMask("Circle mask", float) = 0
        _OuterRadius("Outer radius", Range(0,1)) = 0.5
        _InnerRadius("Inner radius", Range(-1,1)) = 0
        _CircleMaskSmoothness("Circle mask smoothness", Range(0,1)) = 0.2

        //Rect mask
        [Space(20)]
        [Toggle(RECT_MASK)]
        _RectMask("Rectangle mask", float) = 0
        _RectWidth("Rectangle width", Range(0,1)) = 0
        _RectHeight("Rectangle height", Range(0,1)) = 0
        _RectMaskCutoff("Rectangle mask cutoff", Range(0,1)) = 0
        _RectSmoothness("Rectangle mask smoothness", Range(0,1)) = 0 

        //GradientMask
        [Space(20)]
        [Toggle(GRADIENT_MASK)]
        _GradientMask("Gradient mask", float) = 0
        _GradientMaskProperties("Gradient mask (XY Stard/End X, ZW Start/End Y)", Vector) = (0,1,0,1)

        //FresnelMask
        [Space(20)]
        [Toggle(FRESNEL_MASK)]
        _FresnelMask("Fresnel mask", float) = 0
        _FresnelMaskPower("Fresnel mask power", float) = 1

        _MaskPower("Mask power", Range(0,1)) = 1
        [Enum(Multiply, 0, AddSub, 1)] _MaskMode("Mask mode", Int) = 0

        [Header(Culling and Blending)]
        //Culling
        [Enum(UnityEngine.Rendering.CullMode)] _Culling ("Cull Mode", Int) = 2
        //Blending
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("BlendSource", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("BlendDestination", Float) = 10
        [Enum(Off,0,On,1)] _ZWrite("ZWrite", Int) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Int) = 4

        [Header(Soft blending)]
        [Toggle(SOFT_BLENDING)]
        _SoftBlending("Soft blending", float) = 0
        _BlendingFactor("Blending factor", float) = 1
        [Toggle()]
        _ApplyFog("Apply fog", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        Blend [_SrcBlend] [_DstBlend]
        ZWrite [_ZWrite]
        Offset -1, -1
        Cull [_Culling]
        ZTest [_ZTest]
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature_local SECONDARY_TEX
            #pragma shader_feature_local POLAR
            #pragma shader_feature_local SWIRL
            #pragma shader_feature_local VERTEX_OFFSET
            #pragma shader_feature_local BANDING
            #pragma shader_feature_local STEPPED_TIME
            #pragma shader_feature_local USE_GRADIENT_MAP
            #pragma shader_feature_local CIRCLE_MASK
            #pragma shader_feature_local RECT_MASK
            #pragma shader_feature_local GRADIENT_MASK
            #pragma shader_feature_local FRESNEL_MASK
            #pragma shader_feature_local EXTRA_PANNING
            #pragma shader_feature_local DISPLACEMENT
            #pragma shader_feature_local LIGHTING
            #pragma shader_feature_local SCREEN_SPACE_TEXTURE
            #pragma shader_feature_local SOFT_BLENDING
            #pragma shader_feature_local FRESNEL_GLOW
            
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
                float4 color : COLOR;
                float3 normal : NORMAL;
                float4 customData : TEXCOORD1;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float3 normal : NORMAL;
                float4 customData : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                float3 viewDir : TEXCOORD4;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTexProperties;

            sampler2D _SecondaryTex;
            float4 _SecondaryTex_ST;
            float4 _SecondaryTexProperties;

            int _TextureBlending;

            float _SwirlSpin;
            float _SwirlSpeed;
            
            fixed4 _Color;
            float _MultiplyAlpha;
            float _Bands;

            sampler2D _DisplacementGuide;
            float4 _DisplacementGuide_ST;
            float _DisplacementSpeedX;
            float _DisplacementSpeedY;
            float _DisplacementAmount;

            float _ShadowStrength;

            float4 _FresnelGlowColor;
            float _FresnelGlowThreshold;
            float _FresnelGlowSmoothness;

            float _GradientOffset;
            sampler2D _GradientMap;
            float _ValuePower;

            sampler2D _VertexOffsetGuide;
            float4 _VertexOffsetGuide_ST;
            float _InvertVertexOffsetGuide;
            float _VertexOffsetAmount;
            float _VertexOffsetGuideSpeedX;
            float _VertexOffsetGuideSpeedY;

            float _CutoffSmoothness;
            float _Cutoff;
            float _UseAlphaForDissolve;

            float _BurnSize;
            float _BurnSoftness;
            fixed4 _BurnColor;

            sampler2D _ScreenSpaceTexture;
            float4 _ScreenSpaceTexture_ST;
            fixed4 _ScreenSpaceTextureColor;

            float _CircleMaskSmoothness;
            float _OuterRadius;
            float _InnerRadius;

            float _RectSmoothness;
            float _RectHeight;
            float _RectWidth;
            float _RectMaskCutoff;

            float4 _GradientMaskProperties;

            float _FresnelMaskPower;

            int _MaskMode;
            float _MaskPower;

            float _TimeStep;

            float _BlendingFactor;
            sampler2D _CameraDepthTexture;

            float _ApplyFog;


            float getTime(float speed) {
                float time = _Time.y * speed;
                #ifdef STEPPED_TIME
                    time = ceil(_Time.y * (speed / _TimeStep)) * _TimeStep;
                #endif
                return time;
            }

            float2 rotateUV(float2 uv, float rotation)
            {
                float mid = 0.5;
                return float2(
                cos(rotation) * (uv.x - mid) + sin(rotation) * (uv.y - mid) + mid,
                cos(rotation) * (uv.y - mid) - sin(rotation) * (uv.x - mid) + mid
                );
            }

            v2f vert (appdata v)
            {
                v2f o;
                #ifdef VERTEX_OFFSET
                    float offset = tex2Dlod(_VertexOffsetGuide, float4(v.uv.xy * _VertexOffsetGuide_ST.xy + _VertexOffsetGuide_ST.zw + float2(getTime(_VertexOffsetGuideSpeedX), getTime(_VertexOffsetGuideSpeedY)), 0.0, 0.0)).x;
                    if (_InvertVertexOffsetGuide == 1) {
                        offset = 1.0 - offset;
                    }
                    offset = (offset * 2.0 - 1.0) * _VertexOffsetAmount;
                    float4 worldVertexPos = mul(unity_ObjectToWorld, v.vertex);
                    worldVertexPos.xyz += v.normal * offset;
                    v.vertex = mul(unity_WorldToObject, worldVertexPos);
                #endif
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.customData = v.customData;
                o.screenPos = ComputeScreenPos(o.vertex);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float sampleTex(sampler2D tex, float2 uv, float4 tex_st, float4 props, float4 extraPanning) {
                float2 timeOffset = float2(getTime(props.x), getTime(props.y));
                fixed4 col = tex2D(tex, uv * tex_st.xy + tex_st.zw + timeOffset + extraPanning.xy + extraPanning.z).x;
                float value = col.r;
                value = saturate(lerp(0.5, value, props.z)) * props.w;
                value *= lerp(1.0, col.a, _MultiplyAlpha);
                return value;
            }

            float4 sampleTex(sampler2D tex, float2 uv, float4 tex_st) {
                return tex2D(tex, uv * tex_st.xy + tex_st.zw);
            }

            float blendTextures(float valueA, float valueB) {
                switch(_TextureBlending) {
                    case 0:
                    return valueA * valueB * 2.0;
                    case 1:
                    return valueA + valueB;
                    case 2:
                    return min(valueA, valueB);
                    case 3:
                    return max(valueA, valueB);
                    default:
                    return valueA * valueB * 2.0;
                }
            }

            void applyMask(inout float value, float mask) {
                switch(_MaskMode) {
                    case 0:
                    value *= mask * _MaskPower;
                    break;
                    case 1:
                    value = saturate(value + saturate(mask * 2.0 - 1.0) * _MaskPower) * mask;
                    break;
                    default:
                    value *= mask * _MaskPower;
                    break;
                }
            }

            fixed4 frag (v2f i, fixed vface : VFACE) : SV_Target
            {
                float2 uv = i.uv.xy;
                #ifdef EXTRA_PANNING
                    float4 extraPanning = i.customData;
                #else
                    float4 extraPanning = 0;
                #endif

                #ifdef POLAR
                    float2 mappedUV = (uv * 2) - 1;
                    uv = float2(atan2(mappedUV.y, mappedUV.x) / UNITY_PI / 2.0 + 0.5, length(mappedUV));
                #endif

                #ifdef SWIRL
                    uv = rotateUV(uv, saturate(1.0 - length((uv - 0.5) * 2)) * _SwirlSpin + getTime(_SwirlSpeed) * UNITY_TWO_PI);
                #endif

                #ifdef DISPLACEMENT
                    float2 displ = tex2D(_DisplacementGuide, uv * _DisplacementGuide_ST.xy + _DisplacementGuide_ST.zw + float2(getTime(_DisplacementSpeedX), getTime(_DisplacementSpeedY))).xy;
                    displ = (displ * 2.0 - 1.0) * _DisplacementAmount;
                    uv += displ;
                #endif

                float value = sampleTex(_MainTex, uv, _MainTex_ST, _MainTexProperties, extraPanning);

                #ifdef SECONDARY_TEX
                    value = blendTextures(value, sampleTex(_SecondaryTex, uv, _SecondaryTex_ST, _SecondaryTexProperties, extraPanning));
                #endif

                //Masking
                float mask = 1;
                #ifdef CIRCLE_MASK
                    float circle = distance(i.uv.xy, float2(0.5, 0.5));
                    mask = 1 - smoothstep(_OuterRadius, _OuterRadius + _CircleMaskSmoothness, circle);
                    mask *= smoothstep(_InnerRadius, _InnerRadius + _CircleMaskSmoothness, circle);
                    applyMask(value, mask);
                #endif

                #ifdef RECT_MASK
                    float2 uvMapped = (i.uv * 2) - 1;
                    float rect = max(abs(uvMapped.x / _RectWidth), abs(uvMapped.y / _RectHeight));
                    mask = 1 - smoothstep(_RectMaskCutoff, _RectMaskCutoff + _RectSmoothness, rect);
                    applyMask(value, mask);
                #endif

                #ifdef GRADIENT_MASK
                    mask = smoothstep(_GradientMaskProperties.y, _GradientMaskProperties.x, i.uv.x);
                    mask *= smoothstep(_GradientMaskProperties.w, _GradientMaskProperties.z, i.uv.y);
                    applyMask(value, mask);
                #endif
                
                #ifdef FRESNEL_MASK
                    mask = pow(saturate(dot(normalize(i.viewDir), vface > 0 ? i.normal : -i.normal)), _FresnelMaskPower);
                    applyMask(value, mask);
                #endif
                
                //Cutoff
                float cutoff = saturate(_Cutoff + i.uv.z);
                cutoff += (1.0 - i.color.a) * _UseAlphaForDissolve;
                clip(value - saturate(cutoff));
                float alpha = smoothstep(cutoff, saturate(cutoff + _CutoffSmoothness), value) * _Color.a * saturate(i.color.a - _UseAlphaForDissolve);

                //Banding
                #ifdef BANDING
                    value = round(value * _Bands) / _Bands;
                #endif

                value = pow(value, _ValuePower);

                #ifdef USE_GRADIENT_MAP
                    fixed4 col = fixed4(i.color.rgb, _UseAlphaForDissolve ? i.color.a : 1.0) * _Color * tex2D(_GradientMap, float2(value + _GradientOffset, 0));
                #else
                    fixed4 col = fixed4(i.color.rgb, _UseAlphaForDissolve ? i.color.a : 1.0) * _Color * value;
                #endif

                #ifdef LIGHTING
                    float ndotl = saturate(dot(normalize(i.normal), normalize(_WorldSpaceLightPos0)));
                    ndotl = step(0.5, ndotl);
                    col.rgb *= max(1.0 - _ShadowStrength, ndotl);
                #endif

                #ifdef SOFT_BLENDING
                    float depth = LinearEyeDepth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));
                    float diff = saturate(_BlendingFactor * (depth - i.screenPos.w));
                    alpha *= diff;
                #endif

                col.a = alpha;
                col.rgb = lerp(col.rgb, _BurnColor, smoothstep(value - cutoff, saturate(value - cutoff + _BurnSoftness), _BurnSize) * smoothstep(0.001, 0.1, cutoff) * step(0.01, _BurnSize));

                #ifdef SCREEN_SPACE_TEXTURE
                    float2 screenUV = i.screenPos.xy / i.screenPos.w;
                    screenUV.x *= _ScreenParams.x / _ScreenParams.y;
                    col.rgb += tex2D(_ScreenSpaceTexture, screenUV * _ScreenSpaceTexture_ST.xy).rgb * _ScreenSpaceTextureColor;
                #endif

                #ifdef FRESNEL_GLOW
                    float fresnel = smoothstep(_FresnelGlowThreshold, _FresnelGlowThreshold + _FresnelGlowSmoothness, 1.0 - saturate(dot(normalize(i.viewDir), i.normal)));
                    col.rgb += fresnel * _FresnelGlowColor.rgb;
                #endif

                if (_ApplyFog) UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
