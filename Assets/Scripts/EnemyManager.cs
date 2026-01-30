using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    public GameObject enemyPrefab;
    public GameObject bossPrefab;

    public int enemiesPerWave;
    public int baseEnemyHp;
    public int baseEnemyDamage;

    private List<Enemy> enemies = new();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetupForLevel(LevelManager.Instance.levelIndex);
        SpawnWave();
    }

    public void SetupForLevel(int level)
    {
        enemiesPerWave = 6 + level * 3;
        baseEnemyHp = 2 + level;
        baseEnemyDamage = 1 + level / 2;
    }

    public void SpawnWave()
    {
        enemies.Clear();

        for (int i = 0; i < enemiesPerWave; i++)
        {
            Vector2 pos = Random.insideUnitCircle.normalized * 8f;
            enemies.Add(Instantiate(enemyPrefab, pos, Quaternion.identity).GetComponent<Enemy>());
        }
    }

    public void SpawnBoss()
    {
        Vector2 pos = Random.insideUnitCircle.normalized * 7f;
        Instantiate(bossPrefab, pos, Quaternion.identity);
    }

    public void OnEnemyKilled(Enemy e)
{
    if (e is BossEnemy)
        return; // босс не считается волной

    enemies.Remove(e);

    if (enemies.Count == 0)
        GameManager.Instance.OnWaveCleared();
}

    public Enemy GetClosestEnemy(Vector3 pos)
    {
        float min = float.MaxValue;
        Enemy best = null;

        foreach (var e in enemies)
        {
            if (e == null) continue;
            float d = Vector3.Distance(pos, e.transform.position);
            if (d < min)
            {
                min = d;
                best = e;
            }
        }
        return best;
    }
}
