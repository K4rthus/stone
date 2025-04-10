using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MonsterController : MonoBehaviour
{
    [Header("References")][SerializeField] 
    private GameObject gameOverCanvas; 
    [SerializeField] private Transform player; 
    [SerializeField] private Image fadeImage;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float delayBeforeMenu = 2f;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private AudioClip gameOverSound;

    [Header("Player Speed Settings")]
    [SerializeField] private float speedBoostAmount = 2f;
    [SerializeField] private float speedBoostDuration = 3f;

    private float originalSpeed;
    private Coroutine boostCoroutine;

    private bool isChasing = false;
    private Animator animator;
    private Rigidbody2D rb;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        gameOverCanvas.SetActive(false);

        if (fadeImage != null)
        {
            fadeImage.color = Color.clear;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(HandlePlayerCaught());
        }
    }

    public void StartChasing(Transform playerTransform)
    {
        player = playerTransform;
        isChasing = true;
        gameObject.SetActive(true);

        var playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            originalSpeed = playerMovement.moveSpeed;

            playerMovement.moveSpeed += speedBoostAmount;

            if (boostCoroutine != null) StopCoroutine(boostCoroutine);
            boostCoroutine = StartCoroutine(ResetSpeed(playerMovement));
        }

        if (animator != null)
        {
            animator.SetBool("IsRunning", true);
        }
    }

    private IEnumerator ResetSpeed(PlayerMovement playerMovement)
    {
        yield return new WaitForSeconds(speedBoostDuration);
        playerMovement.moveSpeed = originalSpeed;
        boostCoroutine = null;
    }

    private IEnumerator HandlePlayerCaught()
    {
        PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.SetControl(false);
            SoundManager.Instance.sfxSource.Stop();
        }

        if (gameOverSound != null)
        {
            SoundManager.Instance.PlaySFX(gameOverSound);
        }

        gameOverCanvas.SetActive(true);

        yield return StartCoroutine(FadeToBlack());

        yield return new WaitForSecondsRealtime(delayBeforeMenu);

        GameManager.Instance.ResetGame();

        SceneManager.LoadScene("MainMenu");

        Time.timeScale = 1f;
    }

    private IEnumerator FadeToBlack()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            if (fadeImage != null)
            {
                fadeImage.color = Color.Lerp(Color.clear, Color.black, elapsed / fadeDuration);
            }
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    private void Update()
    {
        if (isChasing && player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;

            if (animator != null)
            {
                animator.SetBool("IsRunning", true);
            }

            if (direction.x != 0)
            {
                transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
            }
        }
    }

    public void StopChasing()
    {
        isChasing = false;
        rb.linearVelocity = Vector2.zero;
        gameObject.SetActive(false);

        if (boostCoroutine != null)
        {
            StopCoroutine(boostCoroutine);
            var playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.moveSpeed = originalSpeed;
            }
            boostCoroutine = null;
        }

        if (animator != null)
        {
            animator.SetBool("IsRunning", false);
        }
    }
}