using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Pause menu")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button volumeButton;
    [SerializeField] private Button exitButton;

    [Header("Sound settings")]
    [SerializeField] private Slider volumeSlider;

    [Header("UI sounds")]
    [SerializeField] private AudioClip buttonClickSound;

    public bool IsPaused { get; private set; }
    public static event System.Action<bool> OnPauseStateChanged = delegate { };

    private void Start()
    {
        pauseMenuUI.SetActive(false);

        volumeSlider.value = SoundSettings.Instance.masterVolume;
        volumeSlider.onValueChanged.AddListener(v => {
            SoundSettings.Instance.SetMasterVolume(v);
        });

        volumeSlider.gameObject.SetActive(false);
        volumeButton.onClick.AddListener(() => SoundManager.Instance.PlayUI(buttonClickSound));
        volumeButton.onClick.AddListener(ToggleVolumeSlider);

        continueButton.onClick.AddListener(() => SoundManager.Instance.PlayUI(buttonClickSound));
        continueButton.onClick.AddListener(ContinueGame);
        exitButton.onClick.AddListener(() => SoundManager.Instance.PlayUI(buttonClickSound));
        exitButton.onClick.AddListener(ExitToMenu);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!AutoInteractionController.IsActive) 
            {
                TogglePause();
            }
        }
    }

    private void ToggleVolumeSlider()
    {
        volumeSlider.gameObject.SetActive(!volumeSlider.gameObject.activeSelf);
    }

    private void TogglePause()
    {
        if (IsPaused)
        {
            ContinueGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);

        SoundManager.Instance.ambientSource.Pause();
        SoundManager.Instance.sfxSource.Stop();
        SoundManager.Instance.uiSource.Pause();

        OnPauseStateChanged?.Invoke(true);
    }

    private void ContinueGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);

        SoundManager.Instance.ambientSource.UnPause();
        SoundManager.Instance.uiSource.UnPause();

        OnPauseStateChanged?.Invoke(false);
    }

    private void ExitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}