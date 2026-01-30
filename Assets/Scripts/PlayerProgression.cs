using UnityEngine;

public class PlayerProgression : MonoBehaviour
{
    public static PlayerProgression Instance;

    public int level = 1;

    // прирост за уровень
    public int hpPerLevel = 1;
    public float damageMulPerLevel = 0.1f;      // +10%
    public float speedMulPerLevel = 0.05f;      // +5%
    public float radiusMulPerLevel = 0.05f;     // +5%

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OnLevelUp(Player player)
    {
        level++;

        player.maxHp += hpPerLevel;
        player.damage = Mathf.RoundToInt(player.damage * (1f + damageMulPerLevel));
        player.moveSpeed *= (1f + speedMulPerLevel);
        player.attackRadius *= (1f + radiusMulPerLevel);
    }

    public void ResetRun()
    {
        level = 1;
    }
}
