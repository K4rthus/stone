using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Animator animator;
    [SerializeField] private float minXBoundary;

    [Header("Звук шагов")]
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
        float horizontal = Input.GetAxisRaw("Horizontal");
        Vector2 movement = new(horizontal * moveSpeed, rb.linearVelocity.y);
        rb.linearVelocity = movement;
        bool wasMoving = isMoving;
        isMoving = Mathf.Abs(horizontal) > 0.1f;

        if (transform.position.x < minXBoundary)
        {
            Vector3 newPos = transform.position;
            newPos.x = minXBoundary;
            transform.position = newPos;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        animator.SetFloat("Speed", Mathf.Abs(horizontal));

        if (isMoving && !wasMoving)
        {
            SoundManager.Instance.PlaySFX(footstepSound);
        }
        else if (!isMoving && wasMoving)
        {
            SoundManager.Instance.sfxSource.Stop();
        }

        if (horizontal != 0)
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
        if ((pauseMenu != null && pauseMenu.IsPaused) == false &&
            Input.GetKeyDown(KeyCode.E) &&
            currentInteractable != null)
        {
            currentInteractable.TriggerInteraction();
        }
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
}