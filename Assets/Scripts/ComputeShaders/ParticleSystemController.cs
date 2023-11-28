using System.Runtime.InteropServices;
using UnityEngine;

namespace ComputeShaders
{
    public class ParticleSystemController : MonoBehaviour
    {
        public ComputeShader computeShader;
        private ComputeBuffer particleBuffer;
        private Vector3[] particlePositions=new Vector3[64];
        private int particleCount = 64;
        public Mesh particleMesh;
        public Material particleMaterial;
        private Matrix4x4[] matrices;
        private Vector4[] properties; 
        struct Particle
        {
            public Vector3 position;
            public Vector3 velocity;
        }
        
        void Start()
        {
            particleBuffer = new ComputeBuffer(particleCount, Marshal.SizeOf(typeof(Particle)));
            Particle[] particles = new Particle[particleCount];
            
            for (int i = 0; i < particleCount; i++)
            {
                particlePositions[i] = new Vector3(Random.Range(-5.0f, 5.0f), 5.0f, Random.Range(-5.0f, 5.0f));
                particles[i].velocity = Vector3.zero; 
            }
            particleBuffer.SetData(particles);
            matrices = new Matrix4x4[particleCount];
            properties = new Vector4[particleCount];
        }
        void Update()
        {
            int kernelHandle = computeShader.FindKernel("CSMain");
            computeShader.SetBuffer(kernelHandle, "particles", particleBuffer);
            computeShader.SetFloat("_DeltaTime", Time.deltaTime);
            computeShader.SetInt("particleCount", particleCount);
            
            int threadGroupsX = Mathf.CeilToInt(Mathf.Sqrt(particleCount));
            computeShader.Dispatch(kernelHandle, threadGroupsX, threadGroupsX, 1);
            particleBuffer.GetData(particlePositions);
           
            for (int i = 0; i < particleCount; i++)
            {
                matrices[i] = Matrix4x4.TRS(particlePositions[i], Quaternion.identity, Vector3.one);
            }
            Graphics.DrawMeshInstanced(particleMesh, 0, particleMaterial, matrices);
        }
        void OnDestroy()
        {
            particleBuffer.Release();
        }
    }

}