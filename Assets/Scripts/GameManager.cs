using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int wavesBeforeBoss = 3;

    private int wave;
    private bool bossSpawned;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        wave = 0;
        bossSpawned = false;
    }

    public void OnWaveCleared()
    {
        wave++;

        if (!bossSpawned && wave >= wavesBeforeBoss)
        {
            bossSpawned = true;
            EnemyManager.Instance.SpawnBoss();
        }
        else
        {
            EnemyManager.Instance.SpawnWave();
        }
    }

    public void OnBossKilled()
    {
        LevelManager.Instance.NextLevel();
    }
}
