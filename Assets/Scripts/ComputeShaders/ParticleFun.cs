
using UnityEngine;

namespace ComputeShaders
{
    public class ParticleFun : MonoBehaviour
    {
        private Vector2 cursorPos;

        private struct Particle
        {
            public Vector3 position;
            public Vector3 velocity;
            public float life;
        }
        const int SIZE_PARTICLE = 7 * sizeof(float);
        public int particleCount = 1000000;
        public Material material;
        public ComputeShader shader;
        [Range(1, 10)]
        public int pointSize = 2;
        int kernelID;
        ComputeBuffer particleBuffer;
        int groupSizeX;
        private Camera c;

        private void Awake()
        {
            c = Camera.main;
        }

        private void Start() => Init();

        private void Init()
        {
            Particle[] particleArray = new Particle[particleCount];
            for (int i = 0; i < particleCount; i++)
            {
                float x = Random.value * 2 - 1.0f;
                float y = Random.value * 2 - 1.0f;
                float z = Random.value * 2 - 1.0f;
                Vector3 xyz = new Vector3(x, y, z);
                xyz.Normalize();
                xyz *= Random.value;
                xyz *= 0.5f;
                
                particleArray[i].position.x = xyz.x;
                particleArray[i].position.y = xyz.y;
                particleArray[i].position.z = xyz.z + 3;

                particleArray[i].velocity.x = 0;
                particleArray[i].velocity.y = 0;
                particleArray[i].velocity.z = 0;
                
                particleArray[i].life = Random.value * 5.0f + 1.0f;
            }
            
            particleBuffer = new ComputeBuffer(particleCount, SIZE_PARTICLE);
            particleBuffer.SetData(particleArray);
            kernelID = shader.FindKernel("CSParticle");

            shader.GetKernelThreadGroupSizes(kernelID, out var threadsX, out _, out _);
            groupSizeX = Mathf.CeilToInt((float)particleCount / (float)threadsX);
            
            shader.SetBuffer(kernelID, "particleBuffer", particleBuffer);
            material.SetBuffer("particleBuffer", particleBuffer);
            material.SetInt("_PointSize", pointSize);
        }

        private void OnRenderObject()
        {
            material.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Points, 1, particleCount);
        }

        private void OnDestroy()
        {
            particleBuffer?.Release();
        }

       
        private void Update()
        {

            float[] mousePosition2D = { cursorPos.x, cursorPos.y };
            
            shader.SetFloat("deltaTime", Time.deltaTime);
            shader.SetFloats("mousePosition", mousePosition2D);
            shader.Dispatch(kernelID, groupSizeX, 1, 1);
        }

        private void OnGUI()
        {
            Vector3 point = new Vector3();
            Event e = Event.current;
            Vector2 mousePos = new Vector2
            {
                // Get the mouse position from Event.
                // Note that the y position from Event is inverted.
                x = e.mousePosition.x,
                y = c.pixelHeight - e.mousePosition.y
            };

            point = c.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, c.nearClipPlane + 14));// z = 3.
            cursorPos.x = point.x;
            cursorPos.y = point.y;
        
        }
    }
}