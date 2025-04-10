using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;

    [Header("Prefab Reference")]
    [SerializeField] private GameObject fadeOverlayPrefab;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 2f;

    private CanvasGroup fadeCanvasGroup;
    private GameObject fadeInstance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFadeSystem();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeFadeSystem()
    {
        if (fadeOverlayPrefab == null)
        {
            Debug.LogError("FadeOverlay prefab is not assigned!");
            return;
        }

        fadeInstance = Instantiate(fadeOverlayPrefab);
        DontDestroyOnLoad(fadeInstance);
        fadeInstance.transform.SetParent(transform);

        fadeCanvasGroup = fadeInstance.GetComponentInChildren<CanvasGroup>();
        fadeCanvasGroup.alpha = 0;
    }

    public void TransitionToScene(string targetScene)
    {
        StartCoroutine(SceneTransition(targetScene));
    }

    private IEnumerator SceneTransition(string targetScene)
    {
        fadeCanvasGroup.alpha = 0;
        yield return StartCoroutine(Fade(0, 1));

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        fadeCanvasGroup.alpha = 1;
        yield return StartCoroutine(Fade(1, 0));
    }

    private IEnumerator Fade(float startAlpha, float targetAlpha)
    {
        float elapsed = 0f;
        fadeCanvasGroup.alpha = startAlpha;

        while (elapsed < fadeDuration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "InitialScene")
        {
            StartCoroutine(Fade(1, 0));
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (fadeInstance != null)
        {
            Destroy(fadeInstance);
        }
    }
}