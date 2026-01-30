using UnityEngine;

public class RunTimer : MonoBehaviour
{
    public static RunTimer Instance;
    public float TimeAlive { get; private set; }

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        TimeAlive += Time.deltaTime;
    }
}
