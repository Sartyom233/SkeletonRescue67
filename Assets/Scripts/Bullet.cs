using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 1;
    public float lifeTime = 2f;

    private Rigidbody2D rb;
    private Vector2 direction;

    public void SetDirection(float dir)
    {
        direction = new Vector2(dir, 0);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * speed;
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null) enemy.TakeDamage(damage);

            // ★ ВОТ ЭТО ДОБАВЬ ★
            Largo largo = other.GetComponent<Largo>();
            if (largo != null) largo.TakeDamage(damage);

            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}