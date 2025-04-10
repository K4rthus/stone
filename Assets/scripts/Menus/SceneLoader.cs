using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string firstSceneName = "MainMenu";

    void Start()
    {
        if (string.IsNullOrEmpty(firstSceneName))
        {
            Debug.LogError("First scene name is not set!");
            return;
        }

        SceneManager.LoadScene(firstSceneName);
    }
}