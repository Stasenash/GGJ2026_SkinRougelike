using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp = 2;
    public float speed = 1.2f;
    public int contactDamage = 1;
    public float attackCooldown = 0.7f;

    private int stuckFrames = 0;
private const int STUCK_LIMIT = 12;
private Vector3 escapeDir;


    protected SpriteRenderer rend;
    private float lastAttackTime = -10f;
    public float collisionShrink = 1.0f;

    void Start()
{
    escapeDir = Random.insideUnitCircle.normalized;

    rend = GetComponent<SpriteRenderer>();
    rend.color = ColorManager.Instance.GetEnemyColor();

    hp = EnemyManager.Instance.baseEnemyHp;
    contactDamage = EnemyManager.Instance.baseEnemyDamage;
}


    void Update()
    {
            if (Player.Instance == null)
        return;

    ResolveObstacles();
    MoveTowardsPlayer();
    TryDamagePlayer();
    }

    void MoveTowardsPlayer()
{
    Vector3 dir = (Player.Instance.transform.position - transform.position).normalized;
    Vector3 delta = dir * speed * Time.deltaTime;

    TryMove(delta);
}

void TryMove(Vector3 delta)
{
    bool moved = false;

    if (CanMove(delta))
    {
        transform.position += delta;
        moved = true;
    }
    else
    {
        Vector3 xOnly = new Vector3(delta.x, 0, 0);
        if (CanMove(xOnly))
        {
            transform.position += xOnly;
            moved = true;
        }
        else
        {
            Vector3 yOnly = new Vector3(0, delta.y, 0);
            if (CanMove(yOnly))
            {
                transform.position += yOnly;
                moved = true;
            }
            else
            {
                Vector3 perp = new Vector3(-delta.y, delta.x, 0).normalized * 0.02f;
                if (CanMove(perp))
                {
                    transform.position += perp;
                    moved = true;
                }
            }
        }
    }

    if (moved)
    {
        stuckFrames = 0;
        return;
    }

    // 🚨 АНТИ-СТАК
    stuckFrames++;

    if (stuckFrames >= STUCK_LIMIT)
    {
        Vector3 escape = escapeDir * speed * Time.deltaTime;
        if (CanMove(escape))
        {
            transform.position += escape;
            stuckFrames = 0;
        }
    }
}


void ResolveObstacles()
{
    Collider2D[] hits = Physics2D.OverlapBoxAll(
        transform.position,
        transform.localScale,
        0,
        LayerMask.GetMask("Obstacle", "Wall")
    );

    foreach (var hit in hits)
    {
        Vector3 pushDir = transform.position - hit.transform.position;

        if (pushDir == Vector3.zero)
            pushDir = Random.insideUnitCircle;

        transform.position += pushDir.normalized * 0.05f;
    }
}



bool CanMove(Vector3 delta)
{
    Vector3 nextPos = transform.position + delta;

    return !Physics2D.OverlapBox(
        nextPos,
        transform.localScale,
        0,
        LayerMask.GetMask("Obstacle", "Wall"));
}

    void TryDamagePlayer()
    {
        float playerRadius = Player.Instance.transform.localScale.x * 0.5f;
        float enemyRadius = transform.localScale.x * 0.5f;

        float dist = Vector3.Distance(transform.position, Player.Instance.transform.position);

        if (dist <= playerRadius + enemyRadius)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                Player.Instance.TakeDamage(contactDamage);

                ForceSeparation(playerRadius, enemyRadius);
            }
        }
    }

    void ForceSeparation(float playerRadius, float enemyRadius)
    {
        Vector3 dir = (transform.position - Player.Instance.transform.position).normalized;
        if (dir == Vector3.zero)
            dir = Random.insideUnitCircle.normalized;

        transform.position = Player.Instance.transform.position +
                             dir * (playerRadius + enemyRadius + 0.05f);
    }

    public void TakeDamage(int dmg)
    {
        Debug.Log("ENEMY TAKE DAMAGE");
        hp -= dmg;
        if (hp <= 0)
            Die();
    }

    protected virtual void Die()
    {
        Debug.Log("ENEMY DIED");
    Player.Instance.AbsorbMask(rend.color);
    EnemyManager.Instance.OnEnemyKilled(this);
    Destroy(gameObject);
    }
}
