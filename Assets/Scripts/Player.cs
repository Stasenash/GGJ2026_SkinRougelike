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


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        hp = maxHp;
    }

    void Update()
    {
        if (VoiceInput.Instance == null)
            return;

        switch (VoiceInput.Instance.CurrentState)
        {
            case VoiceState.Move:
                Move();
                break;
            case VoiceState.Attack:
                Attack();
                break;
        }
    }

    void Move()
    {
        Enemy target = EnemyManager.Instance.GetClosestEnemy(transform.position);
        if (target == null) return;

        Vector3 dir = (target.transform.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    void Attack()
{
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