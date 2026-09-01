using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BootstrapLoader : MonoBehaviour
{
    [SerializeField]
    private string firstScene = "MainMenu";

    private string GetStartupScene()
{
#if UNITY_EDITOR

    if (EditorPrefs.HasKey("StartupScene"))
    {
        string scene = EditorPrefs.GetString("StartupScene");

        EditorPrefs.DeleteKey("StartupScene");

        Debug.Log("Editor Startup Scene = " + scene);

        return scene;
    }

#endif

    return firstScene;
}
    private void Start()
    {
        Debug.Log("===== BOOTSTRAP =====");

        string sceneToLoad = GetStartupScene();

        Debug.Log("Loading Scene : " + sceneToLoad);

        SceneManager.LoadScene(sceneToLoad);
    }
}