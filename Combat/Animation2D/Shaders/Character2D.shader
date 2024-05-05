Shader "CombatSystem/Character2D" {
    Properties {
        [MainTexture] _MainTex("Sheet Texture", 2D) = "white" {}
        _Columns("Columns", Float) = 1
        _Rows("Rows", Float) = 1
        _Frame("Frame", Float) = 0
        [Toggle(_FLIP_ON)] _Flip("Flip", Float) = 0
        _LightMaskTex("Light Mask Texture", 2D) = "white" {}
        _LightRampTex("Light Ramp Texture", 2D) = "white" {}
        _LightOffsetRange("Shadow Offset Range", Vector) = (-0.01,-0.01,0.01,0.01)
        _ShadowThickness("Shadow Thickness", Float) = 0.015
        _ShadowColor("Shadow Color", Color) = (0.2, 0.2, 0.2, 1)
        _AlphaClipThreshold("Alpha Clip Threshold", Range(0, 1)) = 0.15
        _AdditionalLightsLightInfluence("Additional Light Influence", Range(0, 5)) = 1
        _AdditionalLightsColorInfluence("Additional Color Intensity", Range(0, 1)) = 1
        _AdditionalLightsColorMultiplier("Additional Color Influence", Range(0, 30)) = 15

        _CurrentBaseZ("Current Base Z", Range(0,1)) = 0
        _MaxOffsetZ("_Max Offset Z", Range(0,1)) = 0
    }

    SubShader {
        Tags {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalRenderPipeline"
        }

        Pass {
            Name "UniversalForward"

            // Render State
            Cull Back
            Blend One Zero
            ZTest LEqual
            ZWrite On

            HLSLPROGRAM
            #pragma vertex ComputeVertex
            #pragma fragment ComputeFragment

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE

            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

            // Keywords
            #pragma multi_compile_local _ _FLIP_ON

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            // The structure definition defines which variables it contains.
            // This example uses the Attributes structure as an input structure in
            // the vertex shader.
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv0 : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float2 uv0 : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float3 positionWST: TEXCOORD3; //position in world space (twisted)
            };

            //Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_TexelSize;
                float _Columns;
                float _Rows;
                float _Frame;
                float4 _LightMaskTex_TexelSize;
                float4 _LightOffsetRange;
                float _ShadowThickness;
                float4 _ShadowColor;
                float _AlphaClipThreshold;
                float _AdditionalLightsLightInfluence;
                float _AdditionalLightsColorInfluence;
                float _AdditionalLightsColorMultiplier;
                float _CurrentBaseZ;
                float _MaxOffsetZ;
            CBUFFER_END

            // Textures and Samplers
            SamplerState sampler_linear_repeat;
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            SamplerState sampler_linear_clamp;
            TEXTURE2D(_LightMaskTex);
            SAMPLER(sampler_LightMaskTex);

            TEXTURE2D(_LightRampTex);
            SAMPLER(sampler_LightRampTex);

            float2 FlipBookUv(float2 uv, float columns, float rows, float frame, float flip)
            {
                float2 output = float2(0, 0);

                float currentColumn = floor(frame % columns);
                float currentRow = floor(frame / columns);

                float uvx = lerp(uv.x, 1 - uv.x, flip);
                output.x = uvx / columns + (currentColumn / columns);
                output.y = uv.y / rows + (rows - 1) / rows - (currentRow / rows);

                return output;
            }

            float3 ProjectOnPlane(float3 vec, float3 normal)
            {
                normal = normalize(normal);
                return vec - normal * dot(vec, normal);
            }

            float2 GetShadowUvOffset(float3 normal, float distance, float4 range, float threshold)
            {
                float2 result;

                float3 lightDir = GetMainLight().direction;

                if (dot(lightDir, normal) < threshold)
                {
                    result = float2(100, 100); //be in shadow
                }
                else
                {
                    float3 projected = ProjectOnPlane(-lightDir, normal);
                    //convert projected to local space
                    float3x3 worldToLocal = float3x3(unity_WorldToObject[0].xyz, unity_WorldToObject[1].xyz, unity_WorldToObject[2].xyz);
                    float3 localProjected = mul(worldToLocal, projected);
                    float2 localOffset = localProjected.xy * distance;
                    result = localOffset;
                    result = clamp(result, range.xy, range.zw);
                }

                return result;
            }

            void GetAdditionalLightsInfo(float3 positionWS, out float shadowMask, out float3 lightColor)
            {
                float shadow = 0;
                float3 color = 0;
                for (int lightIndex = 0; lightIndex < GetAdditionalLightsCount(); lightIndex++)
                {
                    Light light = GetAdditionalLight(lightIndex, positionWS, 1.0);
                    float3 lightC = light.color * light.shadowAttenuation * clamp(light.distanceAttenuation * _AdditionalLightsColorMultiplier, 0, 1);
                    color += lightC;

                    float shadowAtten = light.shadowAttenuation * light.distanceAttenuation * distance(0, light.color);
                    shadow += shadowAtten;
                }

                shadowMask = clamp(shadow, 0, 1);
                lightColor = color;
            }

            float3 TransformHClipToWorld(float4 positionHCS)
            {
                return mul(UNITY_MATRIX_I_VP, positionHCS).xyz;
            }

            Varyings ComputeVertex(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.uv0 = IN.uv0.xy;
                OUT.normalWS = normalize(TransformObjectToWorldNormal(IN.normalOS));

                OUT.positionHCS.z += lerp(0, _MaxOffsetZ, _CurrentBaseZ - OUT.positionHCS.z);
                OUT.positionWST = TransformHClipToWorld(OUT.positionHCS);

                return OUT;
            }

            float4 ComputeFragment(Varyings IN) : SV_Target
            {
                float flip = 0.;
                #ifdef _FLIP_ON
                flip=1.;
                #endif

                //color
                float2 uv = FlipBookUv(IN.uv0, _Columns, _Rows, _Frame, flip);
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

                if (color.a < _AlphaClipThreshold)
                {
                    discard;
                }

                //light mask
                float2 shadowOffset = GetShadowUvOffset(IN.normalWS, _ShadowThickness, _LightOffsetRange, 0.5);

                float4 shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
                Light mainLight = GetMainLight(shadowCoord);

                //additional light
                float addShadowMask = 0;
                float3 addLightColor = 0;
                GetAdditionalLightsInfo(IN.positionWS, addShadowMask, addLightColor);
                addShadowMask *= _AdditionalLightsLightInfluence;
                addShadowMask = SAMPLE_TEXTURE2D(_LightRampTex, sampler_LightRampTex, float2(addShadowMask, 0)).r;

                //receive shadow
                float mainShadowAtten = mainLight.shadowAttenuation;

                #ifdef _FLIP_ON
                shadowOffset.x *= -1;
                #endif

                float lightMask = SAMPLE_TEXTURE2D(_LightMaskTex, sampler_LightMaskTex, uv+shadowOffset).r;
                lightMask *= mainShadowAtten;
                lightMask = max(lightMask, addShadowMask);

                float3 resultColor = lerp((color * _ShadowColor).rgb, color.rgb * (1 + addLightColor * addShadowMask * _AdditionalLightsColorInfluence), lightMask);
                resultColor *= GetMainLight().color;

                float4 result = float4(resultColor, color.a);
                return result;
            }
            ENDHLSL
        }

        //        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode" = "ShadowCaster"
            }

            HLSLPROGRAM
            #pragma vertex ShadowVertex
            #pragma fragment ShadowFragment

            #pragma multi_compile_local _ _FLIP_ON

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"

            SamplerState sampler_linear_repeat;
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_TexelSize;
                float _Columns;
                float _Rows;
                float _Frame;
                float4 _LightMaskTex_TexelSize;
                float4 _LightOffsetRange;
                float _ShadowThickness;
                float4 _ShadowColor;
                float _AlphaClipThreshold;
                float _AdditionalLightsLightInfluence;
                float _AdditionalLightsColorInfluence;
                float _AdditionalLightsColorMultiplier;
                float _CurrentBaseZ;
                float _MaxOffsetZ;
            CBUFFER_END

            float2 FlipBookUv(float2 uv, float columns, float rows, float frame, float flip)
            {
                float2 output = float2(0, 0);

                float currentColumn = floor(frame % columns);
                float currentRow = floor(frame / columns);

                float uvx = lerp(uv.x, 1 - uv.x, flip);
                output.x = uvx / columns + (currentColumn / columns);
                output.y = uv.y / rows + (rows - 1) / rows - (currentRow / rows);

                return output;
            }

            v2f ShadowVertex(appdata v)
            {
                v2f o;
                o.positionHCS = TransformObjectToHClip(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 ShadowFragment(v2f i) : SV_Target
            {
                float flip = 0.;
                #ifdef _FLIP_ON
                flip=1.;
                #endif

                float2 uv = FlipBookUv(i.uv, _Columns, _Rows, _Frame, flip);

                float a = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).a;
                clip(a - _AlphaClipThreshold);
                return float4(0, 0, 0, 0);
            }
            ENDHLSL
        }

    }

}