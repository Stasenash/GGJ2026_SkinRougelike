using UnityEngine;

public class BossEnemy : Enemy
{
    void Awake()
    {
        hp = 20;
        speed = 0.6f;
        contactDamage = 2;
        transform.localScale = Vector3.one * 2.5f;
    }

    void Update()
    {
        if (hp < 10)
    speed = 1.0f;

    if (hp < 5)
        contactDamage = 4;

    }

    protected override void Die()
    {
        Debug.Log("BOSS DIED");

        Player.Instance.AbsorbMask(GetComponent<SpriteRenderer>().color);

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance == null");
            return;
        }

        GameManager.Instance.OnBossKilled();
        Destroy(gameObject);
    }
}
