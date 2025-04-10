using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource ambientSource;
    public AudioSource sfxSource;
    public AudioSource loopingSource;
    public AudioSource uiSource;

    void Awake()
    {
        Debug.Log("SoundManager Awake");

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SoundSettings soundSettings = FindFirstObjectByType<SoundSettings>();
            if (soundSettings == null)
            {
                Debug.LogError("SoundSettings not found in the scene!");
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
        loopingSource.volume = volume;
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

    public void PlayLoopingSFX(AudioClip clip)
    {
        if (loopingSource.isPlaying) loopingSource.Stop();
        loopingSource.clip = clip;
        loopingSource.loop = true;
        loopingSource.Play();
    }

    public void StopLoopingSFX()
    {
        loopingSource.Stop();
    }

    public void StopAllSounds()
    {
        ambientSource.Stop();
        sfxSource.Stop();
        loopingSource.Stop();
        uiSource.Stop();
    }

    public void PauseAllSounds()
    {
        ambientSource.Pause();
        sfxSource.Pause();
        loopingSource.Pause();
        uiSource.Pause();
    }

    public void ResumeAllSounds()
    {
        ambientSource.UnPause();
        sfxSource.UnPause();
        loopingSource.UnPause();
        uiSource.UnPause();
    }

    void OnDestroy()
    {
        if (SoundSettings.Instance != null)
        {
            SoundSettings.Instance.OnVolumeChanged -= UpdateAllVolumes;
        }
    }
}