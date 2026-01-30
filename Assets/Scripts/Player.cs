using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [Header("Movement")]
    public float moveSpeed = 4.5f;

    [Header("Attack")]
    public float attackRadius = 1.6f;
    public float attackCooldown = 0.4f;
    public int damage = 1;

    [Header("Energy")]
    public float screamEnergy = 3f;
    private float currentEnergy;

    [Header("Health")]
    public int maxHp = 10;

    private int hp;
    private float lastAttackTime = -10f;
    private float lastHitTime = -10f;
    private float hitCooldown = 0.5f;

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer rend;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rend = GetComponent<SpriteRenderer>();

        hp = maxHp;
        currentEnergy = screamEnergy;
    }

    void Update()
    {
        if (VoiceInput.Instance == null)
            return;

        // Энергия крика
        if (VoiceInput.Instance.CurrentState == VoiceState.Attack)
            currentEnergy -= Time.deltaTime;
        else
            currentEnergy += Time.deltaTime * 0.8f;

        currentEnergy = Mathf.Clamp(currentEnergy, 0, screamEnergy);

        // Атака
        if (VoiceInput.Instance.CurrentState == VoiceState.Attack)
        {
            if (currentEnergy > 0 && Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                Attack();
            }
        }
    }

    void FixedUpdate()
    {
        ResolvePenetration();
        if (VoiceInput.Instance == null)
            return;

        if (VoiceInput.Instance.CurrentState == VoiceState.Move)
            Move();
    }

    void Move()
    {
        Enemy target = EnemyManager.Instance.GetClosestEnemy(transform.position);
        if (target == null)
            return;

        Vector2 dir = (target.transform.position - transform.position).normalized;
        Vector2 delta = dir * moveSpeed * Time.fixedDeltaTime;

        MoveWithSlide(delta);
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

        Vector2 normal = hits[0].normal;
        Vector2 slide = Vector2.Perpendicular(normal);
        slide *= Vector2.Dot(slide, delta.normalized);

        rb.MovePosition(rb.position + slide * moveSpeed * Time.fixedDeltaTime);
    }

    void Attack()
    {
        int mask = LayerMask.GetMask("Enemy");
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            attackRadius * 1.5f,
            mask
        );

        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
                enemy.TakeDamage(damage);
        }
    }

    public void AbsorbMask(Color color)
    {
        rend.color = color;

        Vector3 scale = transform.localScale;
        scale += Vector3.one * 0.1f;
        scale = Vector3.Min(scale, Vector3.one * 2.5f);
        transform.localScale = scale;

        maxHp += 1;
    }

    public void TakeDamage(int dmg)
    {
        if (Time.time - lastHitTime < hitCooldown)
            return;

        lastHitTime = Time.time;
        hp -= dmg;

        rend.color = Color.white;
        Invoke(nameof(RestoreColor), 0.1f);

        if (hp <= 0)
            Die();
    }

    void RestoreColor()
    {
        // оставляем текущий цвет маски
    }

    void Die()
    {
        LevelManager.Instance.RestartRun();
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


}
