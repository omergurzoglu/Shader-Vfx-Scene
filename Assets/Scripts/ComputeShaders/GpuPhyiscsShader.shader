Shader "Physics/InstancedRigidBodyVertexFragment" {
    Properties {
        _Color ("Color", Color) = (1, 1, 1, 1)
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            float4x4 _Matrix;
            struct appdata {
                float4 vertex : POSITION;
                uint instanceID : SV_InstanceID;
            };
            struct v2f {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
            };
            struct RigidBody
            {
	            float3 position;
	            float4 quaternion;
	            float3 velocity;
	            float3 angularVelocity;
	            int particleIndex;
	            int particleCount;
            };
            StructuredBuffer<RigidBody> rigidBodiesBuffer; 
            fixed4 _Color;
            float4x4 quaternion_to_matrix(float4 quat) {
                float4x4 m = float4x4(float4(0, 0, 0, 0), float4(0, 0, 0, 0), float4(0, 0, 0, 0), float4(0, 0, 0, 0));

            float x = quat.x, y = quat.y, z = quat.z, w = quat.w;
            float x2 = x + x, y2 = y + y, z2 = z + z;
            float xx = x * x2, xy = x * y2, xz = x * z2;
            float yy = y * y2, yz = y * z2, zz = z * z2;
            float wx = w * x2, wy = w * y2, wz = w * z2;

            m[0][0] = 1.0 - (yy + zz);
            m[0][1] = xy - wz;
            m[0][2] = xz + wy;

            m[1][0] = xy + wz;
            m[1][1] = 1.0 - (xx + zz);
            m[1][2] = yz - wx;

            m[2][0] = xz - wy;
            m[2][1] = yz + wx;
            m[2][2] = 1.0 - (xx + yy);

            m[3][3] = 1.0;

            return m;
            }
            float4x4 create_matrix(float3 pos, float4 quat) {
               float4x4 rotation = quaternion_to_matrix(quat);
			float3 position = pos;
			float4x4 translation = {
				1,0,0,position.x,
				0,1,0,position.y,
				0,0,1,position.z,
				0,0,0,1
			};
			return mul(translation, rotation);
            }
            void Setup(uint instanceID: SV_InstanceID)
            {
                RigidBody body = rigidBodiesBuffer[instanceID];
                _Matrix = create_matrix(body.position, body.quaternion); 
            }
            v2f vert(appdata v,uint instanceID: SV_InstanceID)
            {
                v2f o;
                Setup(instanceID);
                v.vertex = mul(_Matrix, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                return i.color;
            }
            ENDCG
        }
    }
}
