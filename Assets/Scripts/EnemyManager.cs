using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    public GameObject enemyPrefab;
    public GameObject bossPrefab;
    public int enemiesPerWave = 4;
    public int baseEnemyHp = 2;
public int baseEnemyDamage = 1;

    private List<Enemy> enemies = new();

    void Awake()
    {
        Instance = this;
    }

   void Start()
{
    SetupForLevel(LevelManager.Instance.levelIndex);
    Debug.Log(LevelManager.Instance.levelIndex);
    SpawnWave();
}


    public void SpawnWave()
{
    enemies.Clear();

    float spawnRadius = 8f;

    for (int i = 0; i < enemiesPerWave; i++)
    {
        Vector2 dir = Random.insideUnitCircle.normalized;
        Vector2 pos = (Vector2)Player.Instance.transform.position + dir * spawnRadius;

        var e = Instantiate(enemyPrefab, pos, Quaternion.identity).GetComponent<Enemy>();
        enemies.Add(e);
    }
}


    public void SpawnBoss()
{
    Vector2 dir = Random.insideUnitCircle.normalized;
    Vector2 pos = (Vector2)Player.Instance.transform.position + dir * 7f;

    Instantiate(bossPrefab, pos, Quaternion.identity);
}

    public void OnEnemyKilled(Enemy enemy)
    {
        enemies.Remove(enemy);

        if (enemies.Count == 0)
            GameManager.Instance.OnWaveCleared();
    }

    public Enemy GetClosestEnemy(Vector3 pos)
    {
        float min = float.MaxValue;
        Enemy closest = null;

        foreach (var e in enemies)
        {
            if (e == null) continue;

            float d = Vector3.Distance(pos, e.transform.position);
            if (d < min)
            {
                min = d;
                closest = e;
            }
        }

        return closest;
    }
 public void SetupForLevel(int level)
{
    enemiesPerWave = 6 + level * 3;
    baseEnemyHp = 2 + level;
    baseEnemyDamage = 1 + level / 2;
}


}