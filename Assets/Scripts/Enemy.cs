using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 2;
    public float moveSpeed = 2f;
    public Transform leftPoint;
    public Transform rightPoint;

    private bool movingRight = true;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (movingRight)
        {
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
            if (transform.position.x >= rightPoint.position.x)
                movingRight = false;
        }
        else
        {
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
            if (transform.position.x <= leftPoint.position.x)
                movingRight = true;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null) player.TakeDamage(1);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (leftPoint != null && rightPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(leftPoint.position, rightPoint.position);
            Gizmos.DrawWireSphere(leftPoint.position, 0.2f);
            Gizmos.DrawWireSphere(rightPoint.position, 0.2f);
        }
    }
}