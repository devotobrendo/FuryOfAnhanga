Shader "Skybox/DualCubemap" {
    Properties {
        _Tint1("Tint Color 1", Color) = (.5, .5, .5, .5)
        _Tint2("Tint Color 2", Color) = (.5, .5, .5, .5)
        [Gamma] _Exposure1("Exposure 1", Range(0, 8)) = 1.0
        [Gamma] _Exposure2("Exposure 2", Range(0, 8)) = 1.0
        _Rotation1("Rotation1", Range(0, 360)) = 0
        _Rotation2("Rotation2", Range(0, 360)) = 0
        [NoScaleOffset] _Cubemap1("Cubemap 1", CUBE) = "black" {}
        [NoScaleOffset] _Cubemap2("Cubemap 2", CUBE) = "black" {}
        _Blend("Blend", Range(0.0, 1.0)) = 0.0
    }

    SubShader {
        Tags { "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
        Cull Off ZWrite Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #include "UnityCG.cginc"

            samplerCUBE _Cubemap1;
            samplerCUBE _Cubemap2;

            half4 _Tint1;
            half4 _Tint2;
            half _Exposure1;
            half _Exposure2;
            float _Rotation1;
            float _Rotation2;
            float _Blend;

            struct appdata_t {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float3 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata_t v) {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                float3 rotated = mul(UNITY_MATRIX_IT_MV, v.vertex.xyz);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = rotated;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                float3 rotated1 = mul(float3x3(
                    cos(_Rotation1 * UNITY_PI / 180.0), 0, sin(_Rotation1 * UNITY_PI / 180.0),
                    0, 1, 0,
                    -sin(_Rotation1 * UNITY_PI / 180.0), 0, cos(_Rotation1 * UNITY_PI / 180.0)), i.texcoord);

                float3 rotated2 = mul(float3x3(
                    cos(_Rotation2 * UNITY_PI / 180.0), 0, sin(_Rotation2 * UNITY_PI / 180.0),
                    0, 1, 0,
                    -sin(_Rotation2 * UNITY_PI / 180.0), 0, cos(_Rotation2 * UNITY_PI / 180.0)), i.texcoord);

                half4 tex1 = texCUBE(_Cubemap1, rotated1);
                half4 tex2 = texCUBE(_Cubemap2, rotated2);

                half3 c1 = tex1.rgb;
                half3 c2 = tex2.rgb;

                c1 = lerp(c1, c2, _Blend) * lerp(_Tint1.rgb, _Tint2.rgb, _Blend) * unity_ColorSpaceDouble.rgb * lerp(_Exposure1, _Exposure2, _Blend);
                return half4(c1, 1);
            }
            ENDCG
        }
    }

    Fallback Off
}
