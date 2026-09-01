using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class EditorBootstrapHelper
{
    static EditorBootstrapHelper()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingEditMode)
            return;

        string currentScene =
            EditorSceneManager.GetActiveScene().name;

        if (currentScene == "Bootstrap")
            return;

        EditorPrefs.SetString(
            "StartupScene",
            currentScene
        );

        Debug.Log("===== EDITOR HELPER =====");
        Debug.Log("Startup Scene = " + currentScene);

        EditorSceneManager.OpenScene(
            "Assets/Scenes/Bootstrap/Bootstrap.unity"
        );
    }
}