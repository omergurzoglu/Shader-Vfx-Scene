//Shader "Custom/GrassClumps"
//{
//    Properties
//    {
//        _Color ("Color", Color) = (1,1,1,1)
//        _MainTex ("Albedo (RGB)", 2D) = "white" {}
//        _Glossiness ("Smoothness", Range(0,1)) = 0.5
//        _Metallic ("Metallic", Range(0,1)) = 0.0
//    }
//    SubShader
//    {
//        Tags{ "RenderType"="Opaque" }
//        
//		LOD 200
//		Cull Off
//		
//        CGPROGRAM
//      
//        #pragma vert vertex
//        #pragma frag fragment
//        #pragma instancing_options procedural:setup
//        sampler2D _MainTex;
//        struct Input
//        {
//            float2 uv_MainTex;
//        };
//        half _Glossiness;
//        half _Metallic;
//        fixed4 _Color;
//        float _Scale;
//        float4x4 _Matrix;
//        float3 _Position;
//
//        float4x4 create_matrix(float3 pos, float theta){
//            float c = cos(theta);
//            float s = sin(theta);
//            return float4x4(
//                c,-s, 0, pos.x,
//                s, c, 0, pos.y,
//                0, 0, 1, pos.z,
//                0, 0, 0, 1
//            );
//        }
//
//        struct v2f {
//            float4 vertex : SV_POSITION;
//            float3 normal : NORMAL;
//            float2 texcoord : TEXCOORD0;
//        }; 
//        
//        struct appdata {
//            float4 vertex : POSITION;
//            float3 normal : NORMAL;
//            float2 texcoord : TEXCOORD0;
//            UNITY_VERTEX_INPUT_INSTANCE_ID
//        }; 
//       
//        
//            struct GrassClump
//            {
//                float3 position;
//                float lean;
//                float noise;
//            };
//            StructuredBuffer<GrassClump> clumpsBuffer; 
//
//        void vert(inout appdata_full v, out Input data)
//        {
//            UNITY_INITIALIZE_OUTPUT(Input, data);
//            
//                v.vertex.xyz *= _Scale;
//                float4 rotatedVertex = mul(_Matrix,v.vertex);
//                v.vertex.xyz += _Position ;
//                v.vertex= lerp(v.vertex,rotatedVertex,v.texcoord.y);
//          
//        }
//
//        void setup()
//        {
//           
//                GrassClump clump = clumpsBuffer[unity_InstanceID];
//                _Position = clump.position;
//                _Matrix = create_matrix(clump.position, clump.lean);
//           
//        }
//
//        // void surf (Input IN, inout SurfaceOutputStandard o)
//        // {
//        //     // Albedo comes from a texture tinted by color
//        //     fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
//        //     o.Albedo = c.rgb;
//        //     // Metallic and smoothness come from slider variables
//        //     o.Metallic = _Metallic;
//        //     o.Smoothness = _Glossiness;
//        //     o.Alpha = c.a;
//        //     clip(c.a-0.4);
//        // }
//        ENDCG
//    }
//    FallBack "Diffuse"
//}