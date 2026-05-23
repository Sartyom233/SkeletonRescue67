using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Движение")]
    public float moveSpeed = 8f;
    public float jumpForce = 4f;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;

    [Header("Лестница")]
    public float climbSpeed = 5f;
    private bool isNearLadder = false;
    private bool isClimbing = false;
    private float defaultGravity;
    private float verticalInput;

    [Header("Стрельба")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootCooldown = 0.3f;
    private float shootTimer;

    [Header("Интерфейс и Здоровье")]
    public int maxHealth = 5;
    private int currentHealth;
    public Text healthText;            
    public HealthUI healthUI;            

    public int maxAmmo = 6;
    private int currentAmmo;
    public float reloadTime = 1.5f;
    private bool isReloading = false;
    public Text ammoText;

    public GameOverUI gameOverUI;        

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

        defaultGravity = rb.gravityScale; // Сохраняем базовую гравитацию (обычно 1 или больше)

        currentAmmo = maxAmmo;
        UpdateAmmoUI();

        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        currentHealth = maxHealth;
        if (healthText != null) healthText.text = "Health: " + currentHealth;
        if (healthUI != null) healthUI.UpdateHealth(currentHealth);
    }

    void Update()
    {
        if (currentHealth <= 0) return;

        moveInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (isNearLadder && Mathf.Abs(verticalInput) > 0.1f)
        {
            isClimbing = true;
        }

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

        if (Input.GetButtonDown("Jump") && (isGrounded || isClimbing))
        {
            isClimbing = false; // Отрываемся от лестницы
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

        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, verticalInput * climbSpeed);
        }
        else
        {
            rb.gravityScale = defaultGravity;
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
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
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        if (healthText != null) healthText.text = "Health: " + currentHealth;
        if (healthUI != null) healthUI.UpdateHealth(currentHealth);

        StartCoroutine(FlashRed());

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (gameOverUI != null) gameOverUI.ShowGameOver();
        this.enabled = false; 
        rb.linearVelocity = Vector2.zero;
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

    // --- ЛОГИКА ОПРЕДЕЛЕНИЯ ЛЕСТНИЦЫ ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isNearLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isNearLadder = false;
            isClimbing = false;
        }
    }
}