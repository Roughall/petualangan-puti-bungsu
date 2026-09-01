using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;
    public Image fadeImage; // full-screen image (black) on UI Canvas
    public float fadeDuration = 0.6f;

    private void Awake()
    {
        Debug.Log("SceneTransition Awake");

        if (Instance == null)
        {
            Instance = this;

            Debug.Log("SceneTransition Instance dibuat");

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("SceneTransition Duplicate");

            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(DoLoad(sceneName));
    }

    IEnumerator DoLoad(string sceneName)
{
    Debug.Log("Fade Out");

    yield return StartCoroutine(FadeOut());

    Debug.Log("Loading Scene");

    AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);

    while (!op.isDone)
        yield return null;

    Debug.Log("Fade In");

    yield return StartCoroutine(FadeIn());

    Debug.Log("Finish");
}

    IEnumerator FadeOut()
    {
        Debug.Log("=== Fade Out Start ===");
        
        if (fadeImage == null) yield break;
        fadeImage.raycastTarget = true;
        float t = 0f;
        Color c = fadeImage.color;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = 1f; fadeImage.color = c;
    }

    IEnumerator FadeIn()
    {
        Debug.Log("=== Fade In Start ===");
        
        if (fadeImage == null) yield break;
        float t = 0f;
        Color c = fadeImage.color;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = 0f; fadeImage.color = c;
        fadeImage.raycastTarget = false;
    }
}
