using System.Collections;
using UnityEngine;

public class CanvasFade : MonoBehaviour
{
    public static CanvasFade Instance;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeSpeed = 2f;

    private bool isFading = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        FadeIn();
    }

    public void FadeIn()
    {
        StartCoroutine(Fade(1, 0));
    }

    public void FadeOut()
    {
        StartCoroutine(Fade(0, 1));
    }

    IEnumerator Fade(float start, float end)
    {
        if (isFading)
            yield break;

        isFading = true;

        float t = 0;

        canvasGroup.alpha = start;

        while (t < 1)
        {
            t += Time.deltaTime * fadeSpeed;
            canvasGroup.alpha = Mathf.Lerp(start, end, t);
            yield return null;
        }

        canvasGroup.alpha = end;

        isFading = false;
    }

    public bool IsFading()
    {
        return isFading;
    }
}