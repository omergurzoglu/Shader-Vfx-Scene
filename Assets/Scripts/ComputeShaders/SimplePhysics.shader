

Shader "Custom/BallShader"
{
    Properties
    {
        _Radius ("Radius", Float) = 0.08
    }
    SubShader
    {
        
        Tags {  
            "RenderPipeline"="UniversalPipeline"
            "RenderType" = "Opaque" }
        
        LOD 100
        Pass
        {
            CGPROGRAM
      
            #pragma multi_compile_instancing
            #pragma vertex vert
            #pragma fragment frag
            #pragma  target 3.0
            
            #include "UnityCG.cginc"
            
            
            
            struct appdata
            {
                float4 vertex : POSITION;
                uint instanceID : SV_InstanceID;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };
            struct Ball
            {
                float3 position;
                float3 velocity;
                float4 color;
               
            };
            StructuredBuffer<Ball> ballsBuffer; 
            float _Radius;
            
            v2f vert (appdata v)
            {
                v2f o;
                Ball ball = ballsBuffer[v.instanceID];
                float4 pos = UnityObjectToClipPos(v.vertex * _Radius + ball.position); 
                o.vertex = pos;
                o.color = ball.color;
                return o;
            }
           
            fixed4 frag (v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}