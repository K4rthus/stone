using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionButton : MonoBehaviour
{
    [SerializeField] public string targetScene;
    private bool _isTransitioning;

    public void TransitionToScene()
    {
        if (_isTransitioning || string.IsNullOrEmpty(targetScene)) return;

        _isTransitioning = true;

        SceneStateManager.Instance.SetNextScene(targetScene);

        if (TransitionManager.Instance != null)
        {
            TransitionManager.Instance.TransitionToScene(targetScene);
        }
        else
        {
            SceneManager.LoadScene(targetScene);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SaveManager.Instance.SaveGame();
        _isTransitioning = false;
    }
}