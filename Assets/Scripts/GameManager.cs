using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int wavesBeforeBoss = 3;

    private int currentWave = 0;
    private bool bossSpawned = false;
    private bool gameFinished = false;

    void Awake()
    {
        Instance = this;
    }

    public void OnWaveCleared()
    {
        if (gameFinished)
            return;

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
    Debug.Log("ON BOSS KILLED");
    Invoke(nameof(FinishLevel), 0.8f);
}

void FinishLevel()
{
    Debug.Log("FINISH LEVEL");
    LevelManager.Instance.NextLevel();
}


void Next()
{
    LevelManager.Instance.NextLevel();
}

}