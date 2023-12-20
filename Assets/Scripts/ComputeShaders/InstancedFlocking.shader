/*/Shader "Flocking/Instanced" { 
//
//   Properties {
//		_Color ("Color", Color) = (1,1,1,1)
//		_MainTex ("Albedo (RGB)", 2D) = "white" {}
//		_BumpMap ("Bumpmap", 2D) = "bump" {}
//		_MetallicGlossMap("Metallic", 2D) = "white" {}
//		_Metallic ("Metallic", Range(0,1)) = 0.0
//		_Glossiness ("Smoothness", Range(0,1)) = 1.0
//	}
//
//   SubShader {
// 
//		CGPROGRAM
//
//		sampler2D _MainTex;
//		sampler2D _BumpMap;
//		sampler2D _MetallicGlossMap;
//		struct Input {
//			float2 uv_MainTex;
//			float2 uv_BumpMap;
//			float3 worldPos;
//		};
//		half _Glossiness;
//		half _Metallic;
//		fixed4 _Color;
// 
//        #pragma surface surf Standard vertex:vert addshadow nolightmap
//        #pragma instancing_options procedural:setup
//
//        float4x4 _LookAtMatrix;
//        float4x4 _Matrix;
//        float3 _BoidPosition;
//
//         #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
//            struct Boid
//            {
//                float3 position;
//                float3 direction;
//                float noise_offset;
//            };
//
//            StructuredBuffer<Boid> boidsBuffer; 
//         #endif
//
//        float4x4 look_at_matrix(float3 dir, float3 up) {
//            float3 zaxis = normalize(dir);
//            float3 xaxis = normalize(cross(up, zaxis));
//            float3 yaxis = cross(zaxis, xaxis);
//            return float4x4(
//                xaxis.x, yaxis.x, zaxis.x, 0,
//                xaxis.y, yaxis.y, zaxis.y, 0,
//                xaxis.z, yaxis.z, zaxis.z, 0,
//                0, 0, 0, 1
//            );
//        }
//        
//        float4x4 create_matrix(float3 pos, float3 dir, float3 up) {
//            float3 zaxis = normalize(dir);
//            float3 xaxis = normalize(cross(up, zaxis));
//            float3 yaxis = cross(zaxis, xaxis);
//            return float4x4(
//                xaxis.x, yaxis.x, zaxis.x, pos.x,
//                xaxis.y, yaxis.y, zaxis.y, pos.y,
//                xaxis.z, yaxis.z, zaxis.z, pos.z,
//                0, 0, 0, 1
//            );
//        }
//     
//         void vert(inout appdata_full v, out Input data)
//        {
//            UNITY_INITIALIZE_OUTPUT(Input, data);
//
//            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
//                //v.vertex = mul(_LookAtMatrix, v.vertex);
//                //v.vertex.xyz += _BoidPosition;
//                v.vertex = mul(_Matrix, v.vertex);
//            #endif
//        }
//
//        void setup()
//        {
//            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
//                _BoidPosition = boidsBuffer[unity_InstanceID].position;
//                //_LookAtMatrix = look_at_matrix(boidsBuffer[unity_InstanceID].direction, float3(0.0, 1.0, 0.0));
//                _Matrix = create_matrix(boidsBuffer[unity_InstanceID].position, boidsBuffer[unity_InstanceID].direction, float3(0.0, 1.0, 0.0));
//            #endif
//        }
// 
//         void surf (Input IN, inout SurfaceOutputStandard o) {
//            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
//			fixed4 m = tex2D (_MetallicGlossMap, IN.uv_MainTex); 
//			o.Albedo = c.rgb;
//			o.Alpha = c.a;
//			o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
//			o.Metallic = m.r;
//			o.Smoothness = _Glossiness * m.a;
//         }
// 
//         ENDCG
//   }
//}*/

Shader "Custom/URPBoidShader"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        //_MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            Cull off
            Tags{ "RenderType" = "Opaque" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            // #pragma target 5.0
            // #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
       
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            #include "AutoLight.cginc"

            //sampler2D _MainTex;
            struct Boid
            {
                float3 position;
                float3 direction;
                float noise_offset;
            };
            struct appdata
            {
                float4 vertex : POSITION;
                //float3 normal : NORMAL;
                
            };
            struct v2f
            {
                float4 vertex : SV_POSITION;
                //float2 uv_MainTex : TEXCOORD0;
                // float3 ambient : TEXCOORD1;
                // float3 diffuse : TEXCOORD2;
                float4 color : COLOR;
                //SHADOW_COORDS(4)
            };
            float4x4 create_matrix(float3 pos, float3 dir, float3 up) {
                float3 zaxis = normalize(dir);
                float3 xaxis = normalize(cross(up, zaxis));
                float3 yaxis = cross(zaxis, xaxis);
                return float4x4(
                    xaxis.x, yaxis.x, zaxis.x, pos.x,
                    xaxis.y, yaxis.y, zaxis.y, pos.y,
                    xaxis.z, yaxis.z, zaxis.z, pos.z,
                    0, 0, 0, 1
                );
            }
            StructuredBuffer<Boid> boidsBuffer; 
            v2f vert(appdata v, uint instanceID : SV_InstanceID)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                Boid boid = boidsBuffer[instanceID];
                float4x4 matrx = create_matrix(boid.position, boid.direction, float3(0, 1, 0));
                // float3 worldNormal = v.normal;
                // float3 ndotl = saturate(dot(worldNormal, _WorldSpaceLightPos0.xyz));
                // float3 ambient = ShadeSH9(float4(worldNormal, 1.0f));
                // float3 diffuse = (ndotl * _LightColor0.rgb);
                o.vertex = UnityObjectToClipPos(mul(matrx, v.vertex));
                o.color= float4(1, 1, 1, 1);
                // o.diffuse = diffuse;
                // o.ambient = ambient;
                // o.uv_MainTex = v.vertex.xy;
                //
                // TRANSFER_SHADOW(o);
                return o;
            }
             UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)
            
            float4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                //fixed shadow = SHADOW_ATTENUATION(i);
                // fixed4 albedo = tex2D(_MainTex, i.uv_MainTex);
                // float3 lighting = i.diffuse * shadow + i.ambient;
                // fixed4 output = fixed4(albedo.rgb * i.color * lighting, albedo.w);
                UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                return i.color;
                //UNITY_APPLY_FOG(i.fogCoord, output);
                //return  output;
                //return i.color;
            }
            ENDCG
        }
    }
    Fallback Off
}