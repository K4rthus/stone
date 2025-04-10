using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextScene : MonoBehaviour
{
    [SerializeField] public string targetSceneName;

    [Header("UI sounds")]
    [SerializeField] private AudioClip buttonClickSound;

    [Header("Buttons")]
    [SerializeField] private Button nextButton;

    private void Start()
    {
        nextButton.onClick.AddListener(() => SoundManager.Instance.PlayUI(buttonClickSound));
        nextButton.onClick.AddListener(LoadScene);
    }
    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError("Имя сцены не указано!");
        }
    }
}