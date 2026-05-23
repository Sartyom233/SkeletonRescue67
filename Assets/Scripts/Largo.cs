using UnityEngine;

public class Largo : MonoBehaviour
{
    [Header("Характеристики")]
    public int health = 12;
    public float moveSpeed = 1.2f;
    public int contactDamage = 2;        

    [Header("Границы движения")]
    public Transform leftPoint;
    public Transform rightPoint;

    [Header("Атака волной")]
    public GameObject wavePrefab;
    public float attackCooldown = 2f;
    public float attackDistance = 5f;
    private bool isAttacking = false;

    private Rigidbody2D rb;
    private bool movingRight = true;
    private float attackTimer;
    private Transform player;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer > attackDistance)
        {
            Patrol();
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        if (attackTimer > 0) attackTimer -= Time.deltaTime;
        if (distToPlayer <= attackDistance && attackTimer <= 0 && wavePrefab != null)
        {
            Attack();
        }
    }

    void Patrol()
    {
        if (leftPoint != null && rightPoint != null)
        {
            if (movingRight)
            {
                rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
                if (transform.position.x >= rightPoint.position.x)
                {
                    movingRight = false;
                    Flip();
                }
            }
            else
            {
                rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
                if (transform.position.x <= leftPoint.position.x)
                {
                    movingRight = true;
                    Flip();
                }
            }
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
        attackTimer = attackCooldown;  
        isAttacking = true;

        Vector2 dir = (player.position - transform.position).normalized;
        animator.SetBool("isSpit", isAttacking);

        GameObject wave = Instantiate(wavePrefab, transform.position, Quaternion.identity);
        Wave waveScript = wave.GetComponent<Wave>();
        if (waveScript != null) waveScript.SetDirection(dir);
        Invoke("EndAttack", 0.5f);
    }

    void EndAttack()
    {
        isAttacking = false;
        animator.SetBool("isSpit", false);
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log("Health: " + health);
        if (health <= 0)
        {
            Debug.Log("Destroying Largo");
            Destroy(gameObject);
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (leftPoint != null) Gizmos.DrawWireSphere(leftPoint.position, 0.2f);
        if (rightPoint != null) Gizmos.DrawWireSphere(rightPoint.position, 0.2f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}