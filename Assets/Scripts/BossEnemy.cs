using UnityEngine;

public class BossEnemy : Enemy
{
    void Awake()
    {
        hp = 20;
        speed = 0.8f;
        transform.localScale = Vector3.one * 2.5f;
    }

    protected override void Die()
{
    Player.Instance.AbsorbMask(GetComponent<SpriteRenderer>().color);
    GameManager.Instance.OnBossKilled();
    Destroy(gameObject);
}
}