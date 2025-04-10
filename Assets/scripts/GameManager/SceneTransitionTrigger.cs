using UnityEngine;

public class SceneTransitionTrigger : MonoBehaviour
{
    [SerializeField] private string targetScene;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && TransitionManager.Instance != null)
        {
            TransitionManager.Instance.TransitionToScene(targetScene);
        }
    }
}