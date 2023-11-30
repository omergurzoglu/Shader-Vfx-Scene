Shader "Custom/HexagonalRippleShader" {
    Properties {
        _HitPosition ("Hit Position", Vector) = (0,0,0,0)
        _RippleSpeed ("Ripple Speed", Float) = 1
        _RippleSize ("Ripple Size", Float) = 1
    }
    SubShader {
        Pass {
            CGPROGRAM
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
#pragma exclude_renderers gles
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _HitPosition;
            float _RippleSpeed;
            float _RippleSize;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            // float2 TransformToHexagonalGrid(float2 uv) {
            //     // This is a conceptual implementation and might need adjustments
            //     float3x3 hexTransform = float3x3(
            //         sqrt(3), 0, 0,
            //         -sqrt(3)/2, 1.5, 0,
            //         0, 0, 1
            //     );
            //
            //     float2 hexUv = mul(hexTransform, float3(uv, 1)).xy;
            //     return hexUv;
            // }
//             float2 TransformToHexagonalGrid(float2 uv, float aspectRatio) {
//     // Adjust UVs based on the aspect ratio
//     uv.y *= aspectRatio;
//
//     // Transform UVs to a hexagonal grid
//     // This transformation will depend on whether you want pointy-topped or flat-topped hexagons
//     // Here's a simple transformation; you may need to adjust this based on your grid's orientation
//     float3x2 hexTransform = float3x2(
//         sqrt(3), sqrt(3)/2,
//         0, 3.0/2.0,
//         0, 0
//     );
//
//     float2 hexUv = mul(hexTransform, float3(uv, 1)).xy;
//
//     // Further adjustments can be made here to align the hex grid as needed
//     // ...
//
//     return hexUv;
// }
float2 TransformToHexagonalGrid(float2 uv, float aspectRatio) {
    // Adjust UVs based on the aspect ratio
    uv.y *= aspectRatio;

    // Calculate new hexagonal UVs. Adjust these values to properly align the hexagons.
    // These values may need fine-tuning based on your specific layout and requirements.
    float3x2 hexTransform = float3x2(
        0.57735, 0, // sqrt(3)/3, 0 for flat-topped hexagons
        -0.288675, 0.5, // -sqrt(3)/6, 0.5 for flat-topped hexagons
        0, 0
    );

    float2 hexUv = mul(hexTransform, float3(uv, 1)).xy;

    // Further adjustments can be made here to align the hex grid as needed
    // ...

    return hexUv;
}

            
            float3 hex_to_cube(float2 hex) {
                float x = hex.x - (hex.y - fmod(hex.y, 2.0)) / 2;
                float z = hex.y;
                float y = -x - z;
                return float3(x, y, z);
            }
       

            float cube_distance(float3 a, float3 b) {
                return max(max(abs(a.x - b.x), abs(a.y - b.y)), abs(a.z - b.z));
            }
            float HexagonalDistance(float2 hexA, float2 hexB) {
                float3 cubeA = hex_to_cube(hexA);
                float3 cubeB = hex_to_cube(hexB);
                return cube_distance(cubeA, cubeB);
            }

            float HexagonalRipple(float distance, float time, float speed, float size) {
                // Simple ripple effect placeholder
                return sin(distance * size - time * speed);
            }
            // float HexagonalScan(float distance, float time, float speed, float size) {
            //     // Calculate the current ring based on time and speed
            //     float currentRing = floor(time * speed);
            //
            //     // Determine if this hexagon is in the current ring
            //     if (distance >= currentRing && distance < currentRing + size) {
            //         // Hexagon is in the current ring, light it up
            //         return 1.0;
            //     } else {
            //         // Hexagon is not in the current ring, keep it off
            //         return 0.0;
            //     }
            // }
            float HexagonalScan(float distance, float time, float speed, float size) {
                // Calculate the current ring. Adjust the formula to control the expansion speed
                float currentRing = floor(time * speed);

                // Modify the condition to check if the hexagon is in the current ring
                // Adding a modulo operation to create a repeating scan effect
                float activeRing = fmod(currentRing, size);

                if (abs(distance - activeRing) < 1.0) {
                    // Hexagon is in the current ring
                    return 1.0;
                } else {
                    // Hexagon is not in the current ring
                    return 0.0;
                }
            }

            

            float4 frag (v2f i) : SV_Target {
                // Transform UVs to a hexagonal grid
                // float2 hexUv = TransformToHexagonalGrid(i.uv);
                //
                // // Calculate distance from the hit position to the current hex cell
                // float distance = HexagonalDistance(_HitPosition.xy, hexUv);
                //
                // // Calculate ripple effect based on distance
                // float rippleEffect = HexagonalRipple(distance, _Time.x, _RippleSpeed, _RippleSize);
                //
                // // Color the ripple
                // float4 color = float4(rippleEffect, rippleEffect, rippleEffect, 1);
                //
                // return color;

                 float aspectRatio = 1;
                float2 hexUv = TransformToHexagonalGrid(i.uv,1);
                float distance = HexagonalDistance(_HitPosition.xy, hexUv);
                float scanEffect = HexagonalScan(distance, _Time.x, _RippleSpeed, _RippleSize);

                float4 color = float4(scanEffect, scanEffect, scanEffect, 1);
                return color;
            }
            ENDCG
        }
    }
}