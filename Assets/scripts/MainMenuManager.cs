using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Кнопки")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button volumeButton;
    [SerializeField] private Button exitButton;

    [Header("Окно подтверждения")]
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private TextMeshProUGUI confirmationText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    [Header("Настройки управления")]
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private Animator panelAnimator;
    [SerializeField] private CanvasGroup panelCanvasGroup;

    [Header("Эффекты выхода")]
    [SerializeField] private Image backgroundFade;
    [SerializeField] private float fadeDuration = 5f;

    [Header("Настройки звука")]
    [SerializeField] private Slider volumeSlider;

    [Header("Звуки UI")]
    [SerializeField] private AudioClip buttonClickSound;

    [Header("Настройки")]
    [SerializeField] private string firstSceneName = "Field";

    private bool isPanelVisible;
    private bool isAnimating;
    private bool isExiting;

    private void Start()
    {
        UpdateLoadButtonState();

        newGameButton.onClick.AddListener(() => SoundManager.Instance.PlayUI(buttonClickSound));
        newGameButton.onClick.AddListener(StartNewGame);

        loadGameButton.onClick.AddListener(() => SoundManager.Instance.PlayUI(buttonClickSound));
        loadGameButton.onClick.AddListener(LoadSavedGame);

        controlsButton.onClick.AddListener(() => SoundManager.Instance.PlayUI(buttonClickSound));
        controlsButton.onClick.AddListener(ToggleControlsPanel);

        exitButton.onClick.AddListener(() => SoundManager.Instance.PlayUI(buttonClickSound));
        exitButton.onClick.AddListener(StartExitSequence);

        confirmationPanel.SetActive(false);
        confirmButton.onClick.AddListener(ConfirmNewGame);
        cancelButton.onClick.AddListener(CancelNewGame);

        volumeSlider.gameObject.SetActive(false);
        volumeButton.onClick.AddListener(() => SoundManager.Instance.PlayUI(buttonClickSound));
        volumeButton.onClick.AddListener(ToggleVolumeSlider);

        controlsPanel.SetActive(false);
        panelCanvasGroup.alpha = 0;
        controlsPanel.transform.localScale = Vector3.zero;

        backgroundFade.color = new Color(0, 0, 0, 0);
        backgroundFade.gameObject.SetActive(false);

        if (SoundSettings.Instance == null)
        {
            Debug.LogError("SoundSettings.Instance is null!");
            return;
        }

        volumeSlider.value = SoundSettings.Instance.masterVolume;
        volumeSlider.onValueChanged.AddListener(volume =>
        {
            SoundSettings.Instance.SetMasterVolume(volume);
        });
    }

    private void StartExitSequence()
    {
        if (!isExiting)
        {
            StartCoroutine(ExitAnimationRoutine());
        }
    }

    private IEnumerator ExitAnimationRoutine()
    {
        isExiting = true;
        backgroundFade.gameObject.SetActive(true);

        SetButtonsInteractable(false);

        float elapsed = 0f;
        Color startColor = backgroundFade.color;
        Color endColor = Color.black;

        while (elapsed < fadeDuration)
        {
            backgroundFade.color = Color.Lerp(startColor, endColor, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        ExitGame();
    }

    private void ToggleVolumeSlider()
    {
        volumeSlider.gameObject.SetActive(!volumeSlider.gameObject.activeSelf);
    }

    private void SetButtonsInteractable(bool state)
    {
        newGameButton.interactable = state;
        loadGameButton.interactable = state;
        controlsButton.interactable = state;
        exitButton.interactable = state;
    }

    private void ExitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    private void UpdateLoadButtonState()
    {
        bool hasSave = File.Exists(Path.Combine(
            Application.persistentDataPath,
            "savegame.json"
        ));

        if (loadGameButton.TryGetComponent<CanvasGroup>(out var cg))
        {
            cg.alpha = hasSave ? 1f : 0.5f;
            cg.interactable = hasSave;
            cg.blocksRaycasts = hasSave;
        }
    }

    private void StartNewGame()
    {
        bool hasSave = File.Exists(Path.Combine(
            Application.persistentDataPath,
            "savegame.json"
        ));

        if (hasSave)
        {
            confirmationPanel.SetActive(true);
        }
        else
        {
            ProceedWithNewGame();
        }
    }

    private void ProceedWithNewGame()
    {
        GameManager.Instance.DeleteSave();
        GameManager.Instance.ResetGame();
        SceneManager.LoadScene(firstSceneName);
    }

    private void ConfirmNewGame()
    {
        confirmationPanel.SetActive(false);
        ProceedWithNewGame();
    }

    private void CancelNewGame()
    {
        confirmationPanel.SetActive(false);
        UpdateLoadButtonState();
    }

    private void ToggleControlsPanel()
    {
        if (isAnimating) return;

        if (!isPanelVisible)
        {
            StartCoroutine(ShowPanel());
        }
        else
        {
            StartCoroutine(HidePanel());
        }
    }

    private IEnumerator ShowPanel()
    {
        isAnimating = true;
        controlsPanel.SetActive(true);
        panelAnimator.SetTrigger("Show");

        yield return new WaitForSeconds(1f);
        isPanelVisible = true;
        isAnimating = false;
    }

    private IEnumerator HidePanel()
    {
        isAnimating = true;
        panelAnimator.SetTrigger("Hide");

        yield return new WaitForSeconds(1f);
        controlsPanel.SetActive(false);
        isPanelVisible = false;
        isAnimating = false;
    }

    private void LoadSavedGame()
    {
        if (File.Exists(Path.Combine(Application.persistentDataPath, "savegame.json")))
        {
            GameManager.Instance.LoadGame();
        }
    }
}