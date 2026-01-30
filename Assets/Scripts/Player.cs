using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public float moveSpeed = 4.5f;
public float attackRadius = 1.6f;
    public int damage = 1;

    public int maxHp = 10;
    private int hp;

    private SpriteRenderer rend;
    private float hitCooldown = 0.5f;
    private float lastHitTime = -10f;
    public float attackCooldown = 0.4f;
    private float lastAttackTime = -10f;
    public float screamEnergy = 3f;
private float currentEnergy;
public float collisionShrink = 0.75f;



    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentEnergy = screamEnergy;
        rend = GetComponent<SpriteRenderer>();
        hp = maxHp;
    }

    void Update()
{

    if (VoiceInput.Instance == null)
        return;

    // 1. Энергия
    if (VoiceInput.Instance.CurrentState == VoiceState.Attack)
        currentEnergy -= Time.deltaTime;
    else
        currentEnergy += Time.deltaTime * 0.8f;

    currentEnergy = Mathf.Clamp(currentEnergy, 0, screamEnergy);

    // 2. Управление
    switch (VoiceInput.Instance.CurrentState)
    {
        case VoiceState.Move:
            Move();
            break;

        case VoiceState.Attack:
            if (currentEnergy > 0)
                Attack();
            break;
    }
}


    void Move()
{
    Enemy target = EnemyManager.Instance.GetClosestEnemy(transform.position);
    if (target == null) return;

    Vector3 dir = (target.transform.position - transform.position).normalized;
    Vector3 delta = dir * moveSpeed * Time.deltaTime;

    TryMove(delta);
}

void TryMove(Vector3 delta)
{
    if (CanMove(delta))
    {
        transform.position += delta;
        return;
    }

    Vector3 xOnly = new Vector3(delta.x, 0, 0);
    if (CanMove(xOnly))
    {
        transform.position += xOnly;
        return;
    }

    Vector3 yOnly = new Vector3(0, delta.y, 0);
    if (CanMove(yOnly))
    {
        transform.position += yOnly;
    }
}

bool CanMove(Vector3 delta)
{
    Vector3 nextPos = transform.position + delta;

    Vector2 checkSize = (Vector2)transform.localScale * collisionShrink;

    return !Physics2D.OverlapBox(
        nextPos,
        checkSize,
        0,
        LayerMask.GetMask("Obstacle", "Wall"));
}


    void Attack()
{

    if (currentEnergy <= 0)
    return;

    if (Time.time - lastAttackTime < attackCooldown)
        return;

    lastAttackTime = Time.time;

    float radius = attackRadius * 1.5f;

    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
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
    rend.color = ColorManager.Instance.GetEnemyColor();
}

    void Die()
    {
        LevelManager.Instance.RestartRun();
    }
}