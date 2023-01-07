Shader "Unlit/OutlineTransparent" {
	Properties {
		_BaseMap ("Example Texture", 2D) = "white" {}
		_BaseColor ("Example Colour", Color) = (0, 0.66, 0.73, 0.5)
		[NoiseTexture] _NoiseTexture("Noise Texture", 2D) = "black" {}

		[Toggle(_NORMALMAP)] _NormalMapToggle ("Normal Mapping", Float) = 0
		[NoScaleOffset] _BumpMap("Normal Map", 2D) = "bump" {}

		[HDR] _EmissionColor("Emission Color", Color) = (0,0,0)
		[Toggle(_EMISSION)] _Emission ("Emission", Float) = 0
		[NoScaleOffset]_EmissionMap("Emission Map", 2D) = "white" {}

		[Toggle(_ALPHATEST_ON)] _AlphaTestToggle ("Alpha Clipping", Float) = 0
		_Cutoff ("Alpha Cutoff", Float) = 0.5

		
		[Toggle(_SPECGLOSSMAP)] _SpecGlossMapToggle ("Use Specular Gloss Map", Float) = 0
		_SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 0.5)
		_SpecGlossMap("Specular Map", 2D) = "white" {}
		[Toggle(_GLOSSINESS_FROM_BASE_ALPHA)] _GlossSource ("Glossiness Source, from Albedo Alpha (if on) vs from Specular (if off)", Float) = 0
		_Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5    

	}
	SubShader {
		Tags {
			"RenderPipeline"="UniversalPipeline"
			"RenderType"="Transparent"
			"Queue"="Transparent"
		}

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		// #include <HLSLSupport.cginc>

		CBUFFER_START(UnityPerMaterial)
		float4 _BaseMap_ST;
		float4 _BaseColor;
		float4 _NoiseTexture_ST;
		float _Cutoff;
		float _Smoothness;
		//float4 _ExampleVector;
		//float _ExampleFloat;
		CBUFFER_END
		ENDHLSL
		
		Pass  // Executes this first
        {
        	Name "OutlineTransparent" 
			Tags { "LightMode" = "OutlineTransparent" }
 
            Cull Back
            ZWrite On
//            ZTest Equal
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            sampler2D _NoiseTexture;
            
            float4 _NoiseTexture_ST;
            
            float _ObjectId;
 
            struct appdata
            {
                float4 vertex : POSITION;
            	float2 uv : TEXCOORD0;
            	float3 normal: NORMAL;
            };
 
            struct v2f
            {
                float4 vertex : SV_POSITION;
            	float2 uv : TEXCOORD0;
            	float3 normal: TEXCOORD1;
            	float depth: TEXCOORD2;
            };
            
 
            v2f vert(appdata v)
            {
                v2f o;
            	
                o.vertex = UnityObjectToClipPos(v.vertex);
            	o.depth = (o.vertex.z / o.vertex.w);
            	o.normal = UnityObjectToWorldNormal(v.normal);
            	o.uv = TRANSFORM_TEX(v.uv, _NoiseTexture);
                return o;
            }
 
            fixed4 frag(v2f i) : SV_Target
            {
            	fixed3 normals = i.normal * 0.5 + 0.5;
                return float4(normals, i.depth);
            }
 
            ENDCG
        }

		Pass  // Executes this first
        {
        	Name "BackOutlineTransparent" 
			Tags { "LightMode" = "BackOutlineTransparent" }
 
            Cull Front
            ZWrite On
//            ZTest Equal
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            sampler2D _NoiseTexture;
            
            float4 _NoiseTexture_ST;
            
            float _ObjectId;
 
            struct appdata
            {
                float4 vertex : POSITION;
            	float2 uv : TEXCOORD0;
            	float3 normal: NORMAL;
            };
 
            struct v2f
            {
                float4 vertex : SV_POSITION;
            	float2 uv : TEXCOORD0;
            	float3 normal: TEXCOORD1;
            	float depth: TEXCOORD2;
            };
            
 
            v2f vert(appdata v)
            {
                v2f o;
            	
                o.vertex = UnityObjectToClipPos(v.vertex);
            	o.depth = (o.vertex.z / o.vertex.w);
            	o.normal = UnityObjectToWorldNormal(v.normal);
            	o.uv = TRANSFORM_TEX(v.uv, _NoiseTexture);
                return o;
            }
 
            fixed4 frag(v2f i) : SV_Target
            {
            	fixed3 normals = i.normal * 0.5 + 0.5;
                return float4(normals, i.depth);
            }
 
            ENDCG
        }
		
		Pass {
			Name "Unlit"
			Tags { "LightMode"="SRPDefaultUnlit" } // (is default anyway)

			Blend SrcAlpha OneMinusSrcAlpha

			HLSLPROGRAM
			#pragma vertex UnlitPassVertex
			#pragma fragment UnlitPassFragment
			

			// Structs
			struct Attributes {
				float4 positionOS	: POSITION;
				float2 uv		    : TEXCOORD0;
				float4 color		: COLOR;
			};

			struct Varyings {
				float4 positionCS 	: SV_POSITION;
				float2 uv		    : TEXCOORD0;
				float4 color		: COLOR;
			};


			// Textures, Samplers & Global Properties
			TEXTURE2D(_BaseMap);
			SAMPLER(sampler_BaseMap);

			// Vertex Shader
			Varyings UnlitPassVertex(Attributes IN) {
				Varyings OUT;

				const VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
				OUT.positionCS = positionInputs.positionCS;
				// Or :
				//OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
				OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
				OUT.color = IN.color;
				return OUT;
			}

			// Fragment Shader
			half4 UnlitPassFragment(Varyings IN) : SV_Target {
				half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);

				return baseMap * _BaseColor * IN.color;
			}
			ENDHLSL
		}
		
		// DepthNormals, used for SSAO & other custom renderer features that request it
		Pass {
			Name "DepthNormals"
			Tags { "LightMode"="DepthNormals" }

			ZWrite On
			ZTest LEqual

			HLSLPROGRAM
			#pragma vertex DepthNormalsVertex
			#pragma fragment DepthNormalsFragment

			// Material Keywords
			#pragma shader_feature_local _NORMALMAP
			#pragma shader_feature_local_fragment _ALPHATEST_ON
			#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			// GPU Instancing
			#pragma multi_compile_instancing
			//#pragma multi_compile _ DOTS_INSTANCING_ON

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/DepthNormalsPass.hlsl"

			// Note if we do any vertex displacement, we'll need to change the vertex function. e.g. :
			/*
			#pragma vertex DisplacedDepthNormalsVertex (instead of DepthNormalsVertex above)
			Varyings DisplacedDepthNormalsVertex(Attributes input) {
				Varyings output = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
				
				// Example Displacement
				input.positionOS += float4(0, _SinTime.y, 0, 0);
				
				output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
				output.positionCS = TransformObjectToHClip(input.position.xyz);
				VertexNormalInputs normalInput = GetVertexNormalInputs(input.normal, input.tangentOS);
				output.normalWS = NormalizeNormalPerVertex(normalInput.normalWS);
				return output;
			}
			*/
			
			ENDHLSL
		}
		Pass {
	        ZWrite On
	        ColorMask 0
		}
	}
}