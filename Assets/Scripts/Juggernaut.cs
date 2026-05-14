using UnityEngine;

public class Juggernaut : MonoBehaviour
{
    public int health = 20;
    public float moveSpeed = 1.5f;
    public int damage = 3;
    public float attackCooldown = 1f;
    public float attackRange = 1.5f; 
    public Transform leftPoint;
    public Transform rightPoint;

    [Header("Анимация")]
    public Animator animator;
    public float attackAnimationDuration = 0.5f;

    private Rigidbody2D rb;
    private bool movingRight = true;
    private float attackTimer;
    private bool isAttacking = false;
    
    private Transform playerTransform;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;
            
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            bool playerIsClose = distanceToPlayer <= attackRange;

            if (playerIsClose && attackTimer <= 0 && !isAttacking)
            {
                Attack();
            }
        }
        
        if (animator != null)
        {
            animator.SetBool("isNear", isAttacking);
        }
        
        if (!isAttacking)
        {
            if (leftPoint != null && rightPoint != null)
            {
                if (movingRight)
                {
                    rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
                    if (transform.position.x >= rightPoint.position.x)
                    {
                        movingRight = false;
                        transform.position = new Vector3(rightPoint.position.x, transform.position.y, transform.position.z);
                        Flip();
                    }
                }
                else
                {
                    rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
                    if (transform.position.x <= leftPoint.position.x)
                    {
                        movingRight = true;
                        transform.position = new Vector3(leftPoint.position.x, transform.position.y, transform.position.z);
                        Flip();
                    }
                }
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void Attack()
    {
        isAttacking = true;
        attackTimer = attackCooldown;

        Invoke("ApplyDamage", 0.3f);
        Invoke("EndAttack", attackAnimationDuration);
    }

    void ApplyDamage()
    {
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D player in hitPlayers)
        {
            if (player.CompareTag("Player"))
            {
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                    playerController.TakeDamage(damage);
                Debug.Log("Juggernaut нанёс урон");
            }
        }
    }

    void EndAttack()
    {
        isAttacking = false;
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        if (animator != null)
        {
            animator.SetTrigger("Hit"); 
        }
        if (health <= 0)
            Die();
    }

    void Die()
    {
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
        GetComponent<Collider2D>().enabled = false;
        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject, 0.5f);
    }
}