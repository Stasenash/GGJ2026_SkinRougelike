using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UISystem : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI hpText;
    public Button restartButton;

    void Start()
    {
        restartButton.onClick.AddListener(Restart);
    }

    void Update()
    {
        if (Player.Instance == null || PlayerProgression.Instance == null)
            return;

        levelText.text = $"Level {PlayerProgression.Instance.level}";
        hpText.text = $"HP {Player.Instance.CurrentHp} / {Player.Instance.maxHp}";
    }

    void Restart()
    {
        LevelManager.Instance.RestartRun();
    }
}
