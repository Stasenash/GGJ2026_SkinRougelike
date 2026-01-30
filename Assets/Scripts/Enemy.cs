using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp = 2;
    public float speed = 1.2f;
    public int contactDamage = 1;
    public float attackCooldown = 0.7f;

    protected SpriteRenderer rend;
    private float lastAttackTime = -10f;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        rend.color = ColorManager.Instance.GetEnemyColor();
    }

    void Update()
    {
        if (Player.Instance == null)
            return;

        MoveTowardsPlayer();
        TryDamagePlayer();
    }

    void MoveTowardsPlayer()
    {
        Vector3 dir = (Player.Instance.transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
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
        hp -= dmg;
        if (hp <= 0)
            Die();
    }

    protected virtual void Die()
    {
        Player.Instance.AbsorbMask(rend.color);
        EnemyManager.Instance.OnEnemyKilled(this);
        Destroy(gameObject);
    }
}
