using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public int levelIndex;

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

    public void NextLevel()
{
    levelIndex++;

    if (Player.Instance != null)
        PlayerProgression.Instance.OnLevelUp(Player.Instance);

    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}


    public void RestartRun()
{
    Time.timeScale = 1f;
    levelIndex = 0;
    PlayerProgression.Instance.ResetRun();
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}

}
