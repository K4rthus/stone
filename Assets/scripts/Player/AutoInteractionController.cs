using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AutoInteractionController : MonoBehaviour
{
    public static bool IsActive { get; private set; }

    [Header("Настройки движения")]
    [SerializeField] private float targetX;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stoppingDistance = 0.1f;

    [Header("Настройки взаимодействия")]
    [SerializeField] private Interactable interactionTarget;
    [SerializeField] private float interactionDelay = 0.5f;

    private PlayerMovement _playerMovement;
    private Rigidbody2D _rb;
    private Animator _animator;
    private Vector3 _initialScale;

    void Start()
    {
        InitializeComponents();
        if (CheckCurrentScene())
        {
            StartCoroutine(EnchantedMovementRoutine());
        }
    }

    private void InitializeComponents()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _initialScale = transform.localScale;

        if (!_playerMovement || !_rb || !_animator || !interactionTarget)
        {
            this.enabled = false;
        }
    }

    private IEnumerator EnchantedMovementRoutine()
    {
        IsActive = true;
        _playerMovement.SetControl(false);

        _animator.SetFloat("Speed", moveSpeed);

        while (Mathf.Abs(transform.position.x - targetX) > stoppingDistance)
        {
            UpdateMovement();
            yield return null;
        }

        _animator.SetFloat("Speed", 0f);
        _rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(interactionDelay);

        if (interactionTarget != null)
        {
            interactionTarget.TriggerInteraction();
        }

        _playerMovement.SetControl(true);
        IsActive = false;
    }

    private void UpdateMovement()
    {
        float direction = Mathf.Sign(targetX - transform.position.x);

        _rb.linearVelocity = new Vector2(direction * moveSpeed, _rb.linearVelocity.y);

        UpdateCharacterFlip(direction);
    }

    private void UpdateCharacterFlip(float direction)
    {
        if (direction != 0)
        {
            float scaleX = Mathf.Abs(_initialScale.x) * Mathf.Sign(direction);
            transform.localScale = new Vector3(
                scaleX,
                _initialScale.y,
                _initialScale.z
            );
        }
    }

    void OnDisable()
    {
        if (IsActive)
        {
            IsActive = false;
            _animator.SetFloat("Speed", 0f);
            if (_playerMovement != null)
            {
                _playerMovement.SetControl(true);
            }
        }
    }

    private bool CheckCurrentScene()
    {
        return SceneManager.GetActiveScene().name == "Cave";
    }
}