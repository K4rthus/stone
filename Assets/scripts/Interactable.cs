using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interactable : MonoBehaviour
{
    [SerializeField] private string targetScene;
    [SerializeField] private string uniqueId;
    [SerializeField] private GameObject outlineObject;
    [SerializeField] private Animator animator;

    [Header("Настройки диалога")]
    [SerializeField] private string dialogueFileName;
    [SerializeField] private string startNodeId = "node1";

    [Header("Звук нажатия")]
    [SerializeField] private AudioClip clickSound;

    private Collider2D _collider;
    private SpriteRenderer _renderer;

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
        _renderer = GetComponent<SpriteRenderer>();

        if (GameManager.Instance.sceneStates.ContainsKey(SceneManager.GetActiveScene().name) &&
            GameManager.Instance.sceneStates[SceneManager.GetActiveScene().name].usedInteractables.Contains(uniqueId))
        {
            DisableInteraction();
        }
        else
        {
            DisableOutline();
        }
    }

    public void TriggerInteraction()
    {
        if (!string.IsNullOrEmpty(targetScene))
        {
            StartCoroutine(PlayAnimationAndLoadScene());
            SoundManager.Instance.PlayUI(clickSound);
        }
    }

    private IEnumerator PlayAnimationAndLoadScene()
    {
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            animator.SetTrigger("Flash");
        }

        yield return new WaitForSeconds(0.25f);

        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        GameManager.Instance.SaveCurrentState(playerPos, uniqueId);

        if (!string.IsNullOrEmpty(dialogueFileName))
        {
            GameManager.Instance.SaveDialogueData(dialogueFileName, startNodeId);
        }

        GameManager.Instance.SaveGame();
        SceneManager.LoadScene(targetScene);
    }

    public void DisableInteraction()
    {
        if (_collider != null) _collider.enabled = false;
        DisableOutline();
    }

    private void EnableOutline()
    {
        if (outlineObject != null)
        {
            outlineObject.SetActive(true);
        }
    }

    private void DisableOutline()
    {
        if (outlineObject != null)
        {
            outlineObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            EnableOutline();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DisableOutline();
        }
    }
}