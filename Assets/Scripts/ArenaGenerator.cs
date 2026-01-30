using UnityEngine;
using System.Collections.Generic;

public class ArenaGenerator : MonoBehaviour
{
    public static ArenaGenerator Instance;

    public GameObject wallPrefab;
    public GameObject obstaclePrefab;

    private List<GameObject> spawned = new();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Generate();
    }

    public void Generate()
    {
        Clear();

        float arenaSize = 10f;

        // Границы арены
        Spawn(wallPrefab, new Vector2(0, arenaSize), new Vector3(arenaSize * 2, 1, 1));
        Spawn(wallPrefab, new Vector2(0, -arenaSize), new Vector3(arenaSize * 2, 1, 1));
        Spawn(wallPrefab, new Vector2(arenaSize, 0), new Vector3(1, arenaSize * 2, 1));
        Spawn(wallPrefab, new Vector2(-arenaSize, 0), new Vector3(1, arenaSize * 2, 1));

        // Маленькие препятствия
        int obstacleCount = 12 + LevelManager.Instance.levelIndex * 2;

        for (int i = 0; i < obstacleCount; i++)
        {
            Vector2 pos;
            int safety = 0;

            do
            {
                pos = Random.insideUnitCircle * 7f;
                safety++;
            }
            while (
                Player.Instance != null &&
                Vector2.Distance(pos, Player.Instance.transform.position) < 2f &&
                safety < 20
            );

            var o = Instantiate(obstaclePrefab, pos, Quaternion.identity);

            float scale = Random.Range(0.25f, 0.4f);
            o.transform.localScale = Vector3.one * scale;

            spawned.Add(o);
        }
    }

    void Spawn(GameObject prefab, Vector2 pos, Vector3 scale)
    {
        var go = Instantiate(prefab, pos, Quaternion.identity);
        go.transform.localScale = scale;
        spawned.Add(go);
    }

    void Clear()
    {
        foreach (var g in spawned)
            if (g != null)
                Destroy(g);

        spawned.Clear();
    }
}
