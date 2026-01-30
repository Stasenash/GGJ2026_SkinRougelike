using UnityEngine;

public class BossEnemy : Enemy
{
    void Awake()
    {
        hp = 20;
        speed = 0.6f;
        contactDamage = 2;
        transform.localScale = Vector3.one * 2.5f;

        mask = MaskDatabase.Instance.GetRandom();
maskType = mask.type;

GetComponent<SpriteRenderer>().color = mask.color;
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

        Player.Instance.AbsorbMask(GetComponent<SpriteRenderer>().color);

        GameManager.Instance.OnBossKilled();
        Destroy(gameObject);
    }
}
