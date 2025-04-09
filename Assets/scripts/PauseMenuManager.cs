using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Меню паузы")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button volumeButton;
    [SerializeField] private Button exitButton;

    [Header("Настройки звука")]
    [SerializeField] private Slider volumeSlider;

    [Header("Звуки UI")]
    [SerializeField] private AudioClip buttonClickSound;

    public bool IsPaused { get; private set; }
    public static event System.Action<bool> OnPauseStateChanged;

    private void Start()
    {
        pauseMenuUI.SetActive(false);

        volumeSlider.value = SoundSettings.Instance.masterVolume; // Установка значения слайдера
        volumeSlider.onValueChanged.AddListener(v => {
            SoundSettings.Instance.SetMasterVolume(v); // Привязка изменения слайдера к MasterVolume
        });

        volumeSlider.gameObject.SetActive(false); // Скрываем слайдер при старте
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
            TogglePause();
        }
    }

    private void ToggleVolumeSlider()
    {
        volumeSlider.gameObject.SetActive(!volumeSlider.gameObject.activeSelf); // Переключаем видимость слайдера
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
        OnPauseStateChanged?.Invoke(true);
    }

    private void ContinueGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        SoundManager.Instance.ambientSource.UnPause();
        OnPauseStateChanged?.Invoke(false);
    }

    private void ExitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}