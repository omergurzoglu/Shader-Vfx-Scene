namespace Editor
{
    using UnityEngine;
    using UnityEditor;
    using Objects; // Make sure this matches the namespace of your ExplosionEffect script

    public class ExplosionEffectPlacer : EditorWindow
    {
        [MenuItem("Tools/Explosion Effect Placer")]
        public static void ShowWindow()
        {
            GetWindow<ExplosionEffectPlacer>("Explosion Effect Placer");
        }

        private void OnGUI()
        {
            GUILayout.Label("Place Explosion Effect in Scene", EditorStyles.boldLabel);
            if (GUILayout.Button("Enable Placement Mode"))
            {
                SceneView.duringSceneGui += OnSceneGUI;
            }
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    ExplosionEffect explosionEffect = FindObjectOfType<ExplosionEffect>();
                    if (explosionEffect != null)
                    {
                        explosionEffect.PlayEffect(hit.point);
                    }
                    else
                    {
                        Debug.LogError("ExplosionEffect script not found in the scene!");
                    }
                }

                SceneView.duringSceneGui -= OnSceneGUI; // Disable after placing
                e.Use();
            }
        }
    }

}