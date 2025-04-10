using UnityEngine;

public class SceneSoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip sceneAmbient;

    void Start()
    {
        SoundManager.Instance.PlayAmbient(sceneAmbient);
    }
}