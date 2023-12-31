﻿using UnityEngine;

namespace ComputeShaders
{
    public class InstancedFlocking : MonoBehaviour
    {
        public struct Boid
        {
            public Vector3 position;
            public Vector3 direction;
            public float noise_offset;

            public Boid(Vector3 pos, Vector3 dir, float offset)
            {
                position.x = pos.x;
                position.y = pos.y;
                position.z = pos.z;
                direction.x = dir.x;
                direction.y = dir.y;
                direction.z = dir.z;
                noise_offset = offset;
            }
        }

        public ComputeShader shader;

        public float rotationSpeed = 1f;
        public float boidSpeed = 1f;
        public float neighbourDistance = 1f;
        public float boidSpeedVariation = 1f;
        public Mesh boidMesh;
        public Material boidMaterial;
        public int boidsCount;
        public float spawnRadius;
        public Transform target;

        int kernelHandle;
        ComputeBuffer boidsBuffer;
        ComputeBuffer argsBuffer;
        uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
        Boid[] boidsArray;
        int groupSizeX;
        int numOfBoids;
        Bounds bounds;
        private static readonly int Time1 = Shader.PropertyToID("time");
        private static readonly int DeltaTime = Shader.PropertyToID("deltaTime");

        void Start()
        {
            kernelHandle = shader.FindKernel("CSMain");

            uint x;
            shader.GetKernelThreadGroupSizes(kernelHandle, out x, out _, out _);
            groupSizeX = Mathf.CeilToInt((float)boidsCount / (float)x);
            numOfBoids = groupSizeX * (int)x;

            bounds = new Bounds(Vector3.zero, Vector3.one * 1000);

            InitBoids();
            InitShader();
        }

        private void InitBoids()
        {
            boidsArray = new Boid[numOfBoids];

            for (int i = 0; i < numOfBoids; i++)
            {
                Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
                Quaternion rot = Quaternion.Slerp(transform.rotation, Random.rotation, 0.3f);
                float offset = Random.value * 1000.0f;
                boidsArray[i] = new Boid(pos, rot.eulerAngles, offset);
            }
        }

        void InitShader()
        {
            boidsBuffer = new ComputeBuffer(numOfBoids, 7 * sizeof(float));
            boidsBuffer.SetData(boidsArray);

            argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
            if (boidMesh != null)
            {
                args[0] = (uint)boidMesh.GetIndexCount(0);
                args[1] = (uint)numOfBoids;
            }
            argsBuffer.SetData(args);

            shader.SetBuffer(this.kernelHandle, "boidsBuffer", boidsBuffer);
            shader.SetFloat("rotationSpeed", rotationSpeed);
            shader.SetFloat("boidSpeed", boidSpeed);
            shader.SetFloat("boidSpeedVariation", boidSpeedVariation);
            
            shader.SetFloat("neighbourDistance", neighbourDistance);
            shader.SetInt("boidsCount", numOfBoids);

            boidMaterial.SetBuffer("boidsBuffer", boidsBuffer);
        }

        void Update()
        {
            shader.SetVector("flockPosition", target.transform.position);
            shader.SetFloat(Time1, Time.time);
            shader.SetFloat(DeltaTime, Time.deltaTime);
            shader.Dispatch(this.kernelHandle, groupSizeX, 1, 1);
            Graphics.DrawMeshInstancedIndirect(boidMesh, 0, boidMaterial, bounds, argsBuffer);
        }

        void OnDestroy()
        {
            if (boidsBuffer != null)
            {
                boidsBuffer.Dispose();
            }

            if (argsBuffer != null)
            {
                argsBuffer.Dispose();
            }
        }
        // void OnDrawGizmos()
        // {
        //     if (boidsArray != null && boidsArray.Length > 5)
        //     {
        //         Gizmos.DrawWireSphere(boidsArray[5].position, 0.5f);
        //     }
        // }
    }
}
