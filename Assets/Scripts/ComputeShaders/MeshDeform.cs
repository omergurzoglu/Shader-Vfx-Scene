﻿using UnityEngine;

namespace ComputeShaders
{
    public class MeshDeform : MonoBehaviour
    {
        public ComputeShader shader;
        [Range(0.5f, 2.0f)]
        public float radius;
	
        int kernelHandle;
        Mesh mesh;

        private Vertex[] vertexArray;
        private Vertex[] initialArray; //model vertex
        private ComputeBuffer vertexBuffer;
        private ComputeBuffer initialBuffer;
        public struct Vertex
        {
            public Vector3 position;
            public Vector3 normal;

            public Vertex(Vector3 p , Vector3 n)
            {
                position.x = p.x;
                position.y = p.y;
                position.z = p.z;
                normal.x = n.x;
                normal.y = n.y;
                normal.z = n.z;
            }
        }
    
        // Use this for initialization
        void Start()
        {
    
            if (InitData())
            {
                InitShader();
            }
        }

        private bool InitData()
        {
            kernelHandle = shader.FindKernel("CSMain");

            MeshFilter mf = GetComponent<MeshFilter>();

            if (mf == null)
            {
                Debug.Log("No MeshFilter found");
                return false;
            }

            InitVertexArrays(mf.mesh);
            InitGPUBuffers();

            mesh = mf.mesh;

            return true;
        }

        private void InitShader()
        {
            shader.SetFloat("radius", radius);

        }
    
        private void InitVertexArrays(Mesh mesh)
        {
            vertexArray = new Vertex[mesh.vertices.Length];
            initialArray = new Vertex[mesh.vertices.Length];

            for (int i = 0; i < vertexArray.Length; i++)
            {
                Vertex v1 = new Vertex(mesh.vertices[i], mesh.normals[i]);
                vertexArray[i] = v1;
                Vertex v2 = new Vertex(mesh.vertices[i], mesh.normals[i]);
                initialArray[i] = v2;
                
            }


        }

        private void InitGPUBuffers()
        {
            vertexBuffer = new ComputeBuffer(vertexArray.Length, sizeof(float) * 6);
            vertexBuffer.SetData(vertexArray);
            initialBuffer = new ComputeBuffer(initialArray.Length, sizeof(float) * 6);
            initialBuffer.SetData(initialArray);
            
            shader.SetBuffer(kernelHandle,"vertexBuffer",vertexBuffer);
            shader.SetBuffer(kernelHandle,"initialBuffer",initialBuffer);
                    
        }
    
        private void GetVerticesFromGPU()
        {
            vertexBuffer.GetData(vertexArray);
            Vector3[] vertices = new Vector3[vertexArray.Length];
            Vector3[] normals = new Vector3[vertexArray.Length];
            for (int i = 0; i < vertexArray.Length; i++)
            {
                vertices[i] = vertexArray[i].position;
                normals[i] = vertexArray[i].normal;
            }

            mesh.vertices = vertices;
            mesh.normals = normals;

        }
        
 
        private void Update(){
            if (shader)
            {
                shader.SetFloat("radius", radius);
                float delta = (Mathf.Sin(Time.time) + 1)/ 2;
                shader.SetFloat("delta", delta);
                shader.Dispatch(kernelHandle, vertexArray.Length, 1, 1);
            
                GetVerticesFromGPU();
            }
        }

        private void OnDestroy()
        {
            vertexBuffer.Dispose();
            initialBuffer.Dispose();
        }
    }
}