using UnityEngine;

public class SoundSettings : MonoBehaviour
{
    public static SoundSettings Instance { get; private set; }

    [Range(0, 1)]
    public float masterVolume = 1f;
    public event System.Action<float> OnVolumeChanged;

    void Awake()
    {
        Debug.Log("SoundSettings Awake");

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolume();
        }
        else
        {
            Destroy(gameObject);
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.UpdateAllVolumes(masterVolume);
        }
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        OnVolumeChanged?.Invoke(masterVolume);
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
    }

    private void LoadVolume()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
    }
}