using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public float moveSpeed = 3f;
    public float attackRadius = 1.5f;
    public int damage = 1;

    private SpriteRenderer rend;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
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
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRadius);
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
    scale = Vector3.Min(scale, Vector3.one * 2.5f); // хард-кап
    transform.localScale = scale;
}
}
