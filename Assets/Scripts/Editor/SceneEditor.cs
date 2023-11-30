using UnityEditor;
using UnityEngine;


public class SceneEditor : EditorWindow
{
    float timeScale = 1.0f;

    [MenuItem("Window/Time Scale Slider")]
    public static void ShowWindow()
    {
        GetWindow<SceneEditor>("Time Scale Slider");
    }

    void OnGUI()
    {
        GUILayout.Label("Adjust Game Time Scale", EditorStyles.boldLabel);
        timeScale = EditorGUILayout.Slider("Time Scale", timeScale, 0.0f, 1.0f);

        if (GUI.changed)
        {
            Time.timeScale = timeScale;
        }
    }
}
