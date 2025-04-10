using UnityEngine;

public class SceneSoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip sceneAmbient;

    void Start()
    {
        if (sceneAmbient == null)
        {
            Debug.LogError("Scene ambient clip is not assigned!");
            return;
        }

        if (SoundManager.Instance == null)
        {
            Debug.LogError("SoundManager instance is not found!");
            return;
        }

        if (SoundManager.Instance.ambientSource.clip != sceneAmbient)
        {
            SoundManager.Instance.PlayAmbient(sceneAmbient);
        }
        else if (!SoundManager.Instance.ambientSource.isPlaying)
        {
            SoundManager.Instance.ambientSource.Play();
        }
    }
}