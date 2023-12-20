using UnityEngine;

public class InstancedRendering : MonoBehaviour
{
    public Mesh instanceMesh;
    public Material instanceMaterial;
    public int instanceCount = 100;
    private Matrix4x4[] matrices;

    void Start()
    {
        matrices = new Matrix4x4[instanceCount];
        for (int i = 0; i < instanceCount; i++)
        {
            matrices[i] = Matrix4x4.TRS(
                Random.insideUnitSphere * 10, // Position
                Quaternion.Euler(Random.value * 360, Random.value * 360, Random.value * 360), // Rotation
                Vector3.one // Scale
            );
        }
    }

    void Update()
    {
        Graphics.DrawMeshInstanced(instanceMesh, 0, instanceMaterial, matrices);
    }
}