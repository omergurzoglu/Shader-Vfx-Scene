using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Boids
{
    public class BoidManager : MonoBehaviour
    {
        public struct BoidData
        {
            public Vector3 position;
            public Vector3 direction;
            public float speed;
        }
        public Mesh boidMesh;
        public Material boidMaterial;
        public int boidsCount;
        public float spawnRadius;
        public Transform target;

        public float alignmentWeight;
        public float cohesionWeight;
        public float separationWeight;
        public float neighbourDistance;

        private NativeArray<BoidData> boidArray;
        private NativeArray<Vector3> newVelocities;
        private JobHandle jobHandle;

        void Start()
        {
            boidArray = new NativeArray<BoidData>(boidsCount, Allocator.Persistent);
            newVelocities = new NativeArray<Vector3>(boidsCount, Allocator.Persistent);
            for (int i = 0; i < boidsCount; i++)
            {
                Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
                Vector3 dir = Random.insideUnitSphere.normalized;

                BoidData boid = new BoidData
                {
                    position = pos,
                    direction = dir,
                    speed = Random.Range(7f, 14f) 
                };

                boidArray[i] = boid;
            }
        }
        void Update()
        {
            BoidJob boidJob = new BoidJob
            {
                boids = boidArray,
                newVelocities = newVelocities,
                alignmentWeight = alignmentWeight,
                cohesionWeight = cohesionWeight,
                separationWeight = separationWeight,
                neighbourDistance = neighbourDistance
            };

            jobHandle = boidJob.Schedule(boidsCount, 64);
            jobHandle.Complete();
            Matrix4x4[] matrices = new Matrix4x4[boidsCount];

            // Apply new velocities to boids
            for (int i = 0; i < boidsCount; i++)
            {
                BoidData boid = boidArray[i];
                Vector3 newVelocity = newVelocities[i];
            
                // Update boid direction and position
                boid.direction = newVelocity.normalized;
                boid.position += boid.direction * (boid.speed * Time.deltaTime);

                boidArray[i] = boid;
                matrices[i] = Matrix4x4.TRS(boid.position, Quaternion.LookRotation(boid.direction), Vector3.one);
            }
            Graphics.DrawMeshInstanced(boidMesh, 0, boidMaterial, matrices, boidsCount, null, UnityEngine.Rendering.ShadowCastingMode.Off, false, 0, null, UnityEngine.Rendering.LightProbeUsage.Off, null);

        }

        void OnDestroy()
        {
            if (boidArray.IsCreated)
            {
                boidArray.Dispose();
            }
        
            if (newVelocities.IsCreated)
            {
                newVelocities.Dispose();
            }
        }
    }
}
