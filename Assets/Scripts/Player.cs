using UnityEngine;

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
    private float lastAttackTime;
    private float lastHitTime;
    private float hitCooldown = 0.5f;

    // ·‡ÁÓ‚˚Â ÁÌ‡˜ÂÌËˇ
    private float baseMoveSpeed;
    private int baseDamage;
    private int baseMaxHp;
    private float baseAttackRadius;

    private MaskData activeMask;

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer rend;
    public int CurrentHp => hp;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rend = GetComponent<SpriteRenderer>();

        transform.localScale = Vector3.one;

        hp = maxHp;
        currentEnergy = screamEnergy;

        // ∆®—“ »… —¡–Œ—
        lastHitTime = -100f;
        lastAttackTime = -100f;

        baseMoveSpeed = moveSpeed;
        baseDamage = damage;
        baseMaxHp = maxHp;
        baseAttackRadius = attackRadius;

        activeMask = null;
        rend.color = Color.gray;
    }

    void Update()
    {
        if (VoiceInput.Instance == null)
            return;

        if (VoiceInput.Instance.CurrentState == VoiceState.Attack)
            currentEnergy -= Time.deltaTime;
        else
            currentEnergy += Time.deltaTime * 0.8f;

        currentEnergy = Mathf.Clamp(currentEnergy, 0, screamEnergy);

        if (VoiceInput.Instance.CurrentState == VoiceState.Attack &&
            currentEnergy > 0 &&
            Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            Attack();
        }
    }

    void FixedUpdate()
    {
        ResolvePenetration();

        if (VoiceInput.Instance != null &&
            VoiceInput.Instance.CurrentState == VoiceState.Move)
        {
            Move();
        }
    }

    // ================= ƒ¬»∆≈Õ»≈ =================

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

    void ResolvePenetration()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            rb.position,
            col.bounds.size,
            0,
            LayerMask.GetMask("Obstacle", "Wall", "Enemy")
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

    // ================= ¡Œ… =================

    void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            attackRadius * 1.5f,
            LayerMask.GetMask("Enemy")
        );

        foreach (var hit in hits)
        {
            Enemy e = hit.GetComponent<Enemy>();
            if (e == null)
                continue;

            e.TakeDamage(damage);

            if (activeMask != null && activeMask.knockbackOnHit)
            {
                Rigidbody2D erb = hit.GetComponent<Rigidbody2D>();
                if (erb != null)
                {
                    Vector2 dir = (erb.position - rb.position).normalized;
                    erb.MovePosition(erb.position + dir * 0.3f);
                }
            }
        }
    }

    public void TakeDamage(int dmg)
    {
        Debug.Log("TAKE DAMAGE");

        // 1. ÛÍÎÓÌÂÌËÂ
        if (activeMask != null && activeMask.dodgeChance > 0f)
        {
            if (Random.value < activeMask.dodgeChance)
                return;
        }

        // 2. ÍÛÎ‰‡ÛÌ
        if (Time.time - lastHitTime < hitCooldown)
            return;

        lastHitTime = Time.time;
        hp -= dmg;

        if (hp <= 0)
        {
            Time.timeScale = 0f;
        }
    }
    // ================= Ã¿— » =================

    public void AbsorbMask(Color color)
    {
        MaskData mask = MaskDatabase.Instance.masks
            .Find(m => m.color == color);

        if (mask == null)
            return;

        ApplyMask(mask);
        rend.color = mask.color;
    }

    void ApplyMask(MaskData mask)
    {
        moveSpeed = baseMoveSpeed;
        damage = baseDamage;
        maxHp = baseMaxHp;
        attackRadius = baseAttackRadius;

        moveSpeed *= mask.moveSpeedMul;
        damage = Mathf.RoundToInt(damage * mask.damageMul);
        maxHp = Mathf.RoundToInt(maxHp * mask.maxHpMul);
        attackRadius *= mask.attackRadiusMul;

        if (mask.type == MaskType.Bear)
            hp = maxHp;
        else
            hp = Mathf.Min(hp, maxHp);

        activeMask = mask;
    }
}