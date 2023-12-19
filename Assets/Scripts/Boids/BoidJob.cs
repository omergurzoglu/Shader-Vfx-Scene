using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Boids
{
    public struct BoidJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<BoidManager.BoidData> boids; // All boids data
        public NativeArray<Vector3> newVelocities; // To store updated velocities

        // Boid behavior parameters
        public float alignmentWeight;
        public float cohesionWeight;
        public float separationWeight;
        public float neighbourDistance;

        public void Execute(int index)
        {
            Vector3 alignment = Vector3.zero;
            Vector3 cohesion = Vector3.zero;
            Vector3 separation = Vector3.zero;
            int neighboursCount = 0;

            var currentBoid = boids[index];

            // Iterate over all boids to calculate alignment, cohesion, and separation
            for (int i = 0; i < boids.Length; i++)
            {
                if (i == index) continue; // Skip self

                var otherBoid = boids[i];
                float distance = Vector3.Distance(currentBoid.position, otherBoid.position);

                if (distance < neighbourDistance)
                {
                    // Alignment - Averages the direction of nearby boids
                    alignment += otherBoid.direction;

                    // Cohesion - Moves towards the average position of nearby boids
                    cohesion += otherBoid.position;

                    // Separation - Moves away from nearby boids to avoid crowding
                    Vector3 difference = currentBoid.position - otherBoid.position;
                    separation += difference.normalized / distance;

                    neighboursCount++;
                }
            }

            if (neighboursCount > 0)
            {
                alignment /= neighboursCount;
                cohesion /= neighboursCount;
                cohesion = (cohesion - currentBoid.position).normalized;
                separation /= neighboursCount;
            }

            // Combine behaviors
            Vector3 newVelocity = alignment * alignmentWeight + cohesion * cohesionWeight + separation * separationWeight;
            newVelocities[index] = newVelocity;
        }
    }
}