using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp;
    public float speed = 1.2f;
    public int contactDamage;

    protected Rigidbody2D rb;
    protected Collider2D col;

    public MaskType maskType;
protected MaskData mask;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        hp = EnemyManager.Instance.baseEnemyHp;
        contactDamage = EnemyManager.Instance.baseEnemyDamage;

        mask = MaskDatabase.Instance.GetRandom();
maskType = mask.type;

GetComponent<SpriteRenderer>().color = mask.color;

    }

    void FixedUpdate()
    {
        ResolvePenetration();
        MoveTowardsPlayer();
        TryDamagePlayer();
    }

    void MoveTowardsPlayer()
    {
        if (Player.Instance == null)
            return;

        Vector2 dir = (Player.Instance.transform.position - transform.position).normalized;
        Vector2 delta = dir * speed * Time.fixedDeltaTime;

        MoveWithSlide(delta);
    }

    void MoveWithSlide(Vector2 delta)
{
    ContactFilter2D filter = new ContactFilter2D();
    filter.SetLayerMask(LayerMask.GetMask("Wall"));
    filter.useTriggers = false;

    RaycastHit2D[] hits = new RaycastHit2D[4];
    int count = col.Cast(delta.normalized, filter, hits, delta.magnitude);

    if (count == 0)
    {
        rb.MovePosition(rb.position + delta);
        return;
    }

    Vector2 normal = hits[0].normal;
    Vector2 slide = Vector2.Perpendicular(normal);
    slide *= Vector2.Dot(slide, delta.normalized);

    rb.MovePosition(rb.position + slide * speed * Time.fixedDeltaTime);
}


    void ResolvePenetration()
{
    Collider2D[] hits = Physics2D.OverlapBoxAll(
        rb.position,
        col.bounds.size,
        0,
        LayerMask.GetMask("Wall", "Player", "Enemy")
    );

    foreach (var hit in hits)
    {
        if (hit == col)
            continue;

        ColliderDistance2D dist = col.Distance(hit);
        if (dist.isOverlapped)
            rb.position += dist.normal * dist.distance;
    }
}


    void TryDamagePlayer()
{
    Debug.Log("try damage");
    if (Player.Instance == null)
        return;

    float dist = Vector2.Distance(rb.position, Player.Instance.transform.position);

    float hitDistance =
        (col.bounds.extents.x + Player.Instance.GetComponent<Collider2D>().bounds.extents.x);

    if (dist <= hitDistance)
    {
        Player.Instance.TakeDamage(contactDamage);
    }
}


    public void TakeDamage(int dmg)
    {
        Debug.Log("damage");
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
