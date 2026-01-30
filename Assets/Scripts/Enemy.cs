using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp = 2;
    public float speed = 2f;

    protected SpriteRenderer rend;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        rend.color = ColorManager.Instance.GetEnemyColor();
    }

    void Update()
    {
        Vector3 dir = (Player.Instance.transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
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