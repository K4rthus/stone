using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnButton : MonoBehaviour
{
    private bool _isReturning = false;

    public void ReturnToPreviousScene()
    {
        if (_isReturning) return;

        _isReturning = true;
        string targetScene = GameManager.Instance.GetPreviousScene();
        if (!string.IsNullOrEmpty(targetScene))
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(targetScene);
        }
        else
        {
            Debug.LogError("Previous scene not found!");
            _isReturning = false;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameManager.Instance.SaveGame();
        _isReturning = false;
    }
}