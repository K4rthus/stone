using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource ambientSource;
    public AudioSource sfxSource;
    public AudioSource uiSource;

    void Awake()
    {
        Debug.Log("SoundManager Awake");

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Находим SoundSettings в сцене
            SoundSettings soundSettings = FindFirstObjectByType<SoundSettings>();
            if (soundSettings == null)
            {
                Debug.LogError("SoundSettings не найден в сцене!");
                return;
            }

            soundSettings.OnVolumeChanged += UpdateAllVolumes;
            UpdateAllVolumes(soundSettings.masterVolume);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateAllVolumes(float volume)
    {
        ambientSource.volume = volume;
        sfxSource.volume = volume;
        uiSource.volume = volume;
    }

    public void PlayAmbient(AudioClip clip)
    {
        ambientSource.clip = clip;
        ambientSource.loop = true;
        ambientSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayUI(AudioClip clip)
    {
        uiSource.PlayOneShot(clip);
    }

    void OnDestroy()
    {
        if (SoundSettings.Instance != null)
        {
            SoundSettings.Instance.OnVolumeChanged -= UpdateAllVolumes;
        }
    }
}