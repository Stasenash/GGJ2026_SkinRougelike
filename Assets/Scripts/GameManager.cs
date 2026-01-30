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
        if (gameFinished)
            return;

        gameFinished = true;
        Debug.Log("WIN");
        Time.timeScale = 0f;
    }
}