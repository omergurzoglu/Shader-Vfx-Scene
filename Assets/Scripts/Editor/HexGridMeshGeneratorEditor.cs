// using UnityEngine;
// using UnityEditor;
// using System.Collections.Generic;
//
// [CustomEditor(typeof(MeshFilter))]
// public class HexGridMeshGeneratorEditor : Editor
// {
//     private int iterations = 1;
//
//     public override void OnInspectorGUI()
//     {
//         base.OnInspectorGUI();
//
//         MeshFilter meshFilter = (MeshFilter)target;
//
//         iterations = EditorGUILayout.IntSlider("Iterations", iterations, 1, 5);
//
//         if (GUILayout.Button("Generate Hexagonal Grid Mesh"))
//         {
//             meshFilter.mesh = GenerateHexagonalGridMesh(iterations);
//         }
//     }
//
//     private Mesh GenerateHexagonalGridMesh(int iterations)
//     {
//         Mesh mesh = new Mesh();
//         List<Vector3> vertices = new List<Vector3>();
//         List<int> triangles = new List<int>();
//
//         float size = 1f;
//         float angleDeg = 60f;
//         float angleRad = Mathf.Deg2Rad * angleDeg;
//
//         
//         for (int i = 0; i < iterations; i++)
//         {
//             for (int j = 0; j < 6 * i; j++)
//             {
//                 float angle = angleRad * j / i;
//                 Vector3 center = new Vector3(size * i * Mathf.Cos(angle), 0, size * i * Mathf.Sin(angle));
//
//                 AddHexagon(center, size, vertices, triangles);
//             }
//         }
//
//         mesh.vertices = vertices.ToArray();
//         mesh.triangles = triangles.ToArray();
//         mesh.RecalculateNormals();
//
//         return mesh;
//     }
//
//     private void AddHexagon(Vector3 center, float size, List<Vector3> vertices, List<int> triangles)
//     {
//         int vertexIndex = vertices.Count;
//         vertices.Add(center); // Center vertex
//
//         float angleDeg = 60f;
//         float angleRad = Mathf.Deg2Rad * angleDeg;
//
//         
//         for (int i = 0; i < 6; i++)
//         {
//             vertices.Add(center + new Vector3(size * Mathf.Cos(angleRad * i), 0, size * Mathf.Sin(angleRad * i)));
//         }
//
//        
//         for (int i = 0; i < 6; i++)
//         {
//             triangles.Add(vertexIndex);
//             triangles.Add(vertexIndex + i + 1);
//             triangles.Add(vertexIndex + (i + 1) % 6 + 1);
//         }
//     }
// }
