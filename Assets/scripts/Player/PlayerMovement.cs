using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 1.1f; 
    [SerializeField] private Animator animator; 
    [SerializeField] private float minXBoundary;

    [Header("Walking sound")]
    [SerializeField] private AudioClip footstepSound;

    private Rigidbody2D rb;
    private Interactable currentInteractable;

    private PauseMenuManager pauseMenu;
    private bool isMoving = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pauseMenu = FindFirstObjectByType<PauseMenuManager>();
    }

    void Update()
    {
        if (pauseMenu != null && pauseMenu.IsPaused) return;

        HandleMovement();
        HandleInteraction();
    }

    private void HandleMovement()
    {
        if (pauseMenu != null && pauseMenu.IsPaused)
        {
            if (isMoving)
            {
                SoundManager.Instance.sfxSource.Stop();
                isMoving = false;
                animator.SetFloat("Speed", 0f);
            }
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        Vector2 movement = new(horizontal * moveSpeed, rb.linearVelocity.y);

        if (!pauseMenu.IsPaused)
        {
            rb.linearVelocity = movement;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (transform.position.x < minXBoundary)
        {
            Vector3 newPos = transform.position;
            newPos.x = minXBoundary;
            transform.position = newPos;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        float speed = Mathf.Abs(horizontal);
        animator.SetFloat("Speed", speed);

        bool wasMoving = isMoving;
        isMoving = speed > 0.1f;

        if (isMoving)
        {
            if (!wasMoving || !SoundManager.Instance.sfxSource.isPlaying)
            {
                SoundManager.Instance.PlaySFX(footstepSound);
            }
        }
        else if (wasMoving)
        {
            SoundManager.Instance.sfxSource.Stop();
        }

        if (horizontal != 0 && !pauseMenu.IsPaused)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(horizontal),
                1,
                1
            );
        }
    }

    private void HandleInteraction()
    {
        if (pauseMenu.IsPaused ||
            !Input.GetKeyDown(KeyCode.E) ||
            currentInteractable == null)
        {
            return;
        }
        currentInteractable.TriggerInteraction();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Interactable>(out var interactable))
        {
            currentInteractable = interactable;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Interactable>(out var interactable) &&
            interactable == currentInteractable)
        {
            currentInteractable = null;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SoundManager.Instance.sfxSource.Stop();
        isMoving = false;
        animator.SetFloat("Speed", 0f);
    }

    public void SetControl(bool enabled)
    {
        this.enabled = enabled;

        if (!enabled)
        {
            SoundManager.Instance.sfxSource.Stop();
            isMoving = false;
            animator.SetFloat("Speed", 0f);
        }
    }
}