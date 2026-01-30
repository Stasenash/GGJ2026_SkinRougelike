using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp = 2;
    public float speed = 1.2f;
    public int contactDamage = 1;

    private Rigidbody2D rb;
    private Collider2D col;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        hp = EnemyManager.Instance.baseEnemyHp;
        contactDamage = EnemyManager.Instance.baseEnemyDamage;
    }

    void FixedUpdate()
    {
        ResolvePenetration();
        if (Player.Instance == null)
            return;

        Vector2 dir = (Player.Instance.transform.position - transform.position).normalized;
        Vector2 delta = dir * speed * Time.fixedDeltaTime;

        MoveWithSlide(delta);
        TryDamagePlayer();
    }

    void MoveWithSlide(Vector2 delta)
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Obstacle", "Wall"));
        filter.useTriggers = false;

        RaycastHit2D[] hits = new RaycastHit2D[4];

        int count = col.Cast(delta.normalized, filter, hits, delta.magnitude);

        if (count == 0)
        {
            rb.MovePosition(rb.position + delta);
            return;
        }

        // скольжение вдоль поверхности
        Vector2 normal = hits[0].normal;
        Vector2 slide = Vector2.Perpendicular(normal);
        slide *= Vector2.Dot(slide, delta.normalized);

        rb.MovePosition(rb.position + slide * speed * Time.fixedDeltaTime);
    }

    void TryDamagePlayer()
    {
        float dist = Vector2.Distance(transform.position, Player.Instance.transform.position);
        if (dist < (transform.localScale.x + Player.Instance.transform.localScale.x) * 0.5f)
        {
            Player.Instance.TakeDamage(contactDamage);
        }
    }
    void ResolvePenetration()
{
    int mask = LayerMask.GetMask("Obstacle", "Wall", "Enemy", "Player");

    Collider2D[] hits = Physics2D.OverlapBoxAll(
        rb.position,
        col.bounds.size,
        0,
        mask
    );

    foreach (var hit in hits)
    {
        if (hit == col)
            continue;

        ColliderDistance2D dist = col.Distance(hit);

        if (dist.isOverlapped)
        {
            // normal указывает НАПРАВЛЕНИЕ ВЫХОДА
            rb.position += dist.normal * dist.distance;
        }
    }
}

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0)
            Die();
    }

    protected virtual void Die()
    {
        Player.Instance.AbsorbMask(GetComponent<SpriteRenderer>().color);
        EnemyManager.Instance.OnEnemyKilled(this);
        Destroy(gameObject);
    }
}
