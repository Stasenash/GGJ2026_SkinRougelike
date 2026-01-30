using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int wavesBeforeBoss = 3;

    private int currentWave;
    private bool bossSpawned;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ResetState();
    }

    public void ResetState()
    {
        currentWave = 0;
        bossSpawned = false;
    }

    public void OnWaveCleared()
    {
        if (bossSpawned)
            return;

        currentWave++;

        if (currentWave >= wavesBeforeBoss)
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
        Invoke(nameof(FinishLevel), 0.8f);
    }

    void FinishLevel()
    {
        LevelManager.Instance.NextLevel();
    }
}
