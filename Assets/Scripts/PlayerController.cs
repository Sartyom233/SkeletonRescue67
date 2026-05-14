using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float jumpForce = 4f;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootCooldown = 0.3f;
    private float shootTimer;

    public int maxHealth = 5;
    private int currentHealth;
    public Text healthText;              // старый текстовый индикатор (опционально)
    public HealthUI healthUI;            // сердечки

    public int maxAmmo = 6;
    private int currentAmmo;
    public float reloadTime = 1.5f;
    private bool isReloading = false;
    public Text ammoText;

    public GameOverUI gameOverUI;        // экран проигрыша

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private bool isGrounded;
    private float moveInput;
    private Animator animator;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;

        currentAmmo = maxAmmo;
        UpdateAmmoUI();

        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        currentHealth = maxHealth;
        if (healthText != null) healthText.text = "Health: " + currentHealth;
        if (healthUI != null) healthUI.UpdateHealth(currentHealth);
    }

    void Update()
    {
        if (currentHealth <= 0) return; // если мёртв – не управляем

        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput != 0)
        {
            float absoluteScaleX = Mathf.Abs(transform.localScale.x);
            transform.localScale = new Vector3(absoluteScaleX * Mathf.Sign(moveInput), transform.localScale.y, transform.localScale.z);
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(moveInput));
            animator.SetBool("isGrounded", isGrounded);
            animator.SetFloat("yVelocity", rb.linearVelocity.y);
        }

        isGrounded = CheckGrounded();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (isReloading) return;

        if (currentAmmo <= 0 || (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo))
        {
            StartCoroutine(Reload());
            return;
        }

        if (shootTimer > 0) shootTimer -= Time.deltaTime;
        if (Input.GetButtonDown("Fire1") && shootTimer <= 0)
        {
            Shoot();
            shootTimer = shootCooldown;
        }
    }

    void FixedUpdate()
    {
        if (currentHealth <= 0) return;
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        if (animator != null) animator.SetBool("isGrounded", false);
    }

    private bool CheckGrounded()
    {
        if (boxCollider == null) return false;
        Bounds bounds = boxCollider.bounds;
        float bottomY = bounds.min.y;
        Vector2 origin = new Vector2(bounds.center.x, bottomY);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null;
    }

    void Shoot()
    {
        currentAmmo--;
        UpdateAmmoUI();

        GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bulletScript = newBullet.GetComponent<Bullet>();
        float direction = Mathf.Sign(transform.localScale.x);
        bulletScript.SetDirection(direction);
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return; // уже мёртв

        currentHealth -= damage;
        if (healthText != null) healthText.text = "Health: " + currentHealth;
        if (healthUI != null) healthUI.UpdateHealth(currentHealth);

        StartCoroutine(FlashRed());

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (gameOverUI != null) gameOverUI.ShowGameOver();
        this.enabled = false;  // отключаем скрипт управления
        rb.linearVelocity = Vector2.zero;
        // можно также отключить коллайдер или скрыть спрайт
        if (spriteRenderer != null) spriteRenderer.enabled = false;
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        if (ammoText != null) ammoText.text = "Reloading...";
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = "Ammo: " + currentAmmo + " / " + maxAmmo;
    }

    private IEnumerator FlashRed()
    {
        if (spriteRenderer == null) yield break;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }
}