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

        float size = 10f;

        Spawn(wallPrefab, new Vector2(0, size), new Vector3(size*2,1,1));
        Spawn(wallPrefab, new Vector2(0, -size), new Vector3(size*2,1,1));
        Spawn(wallPrefab, new Vector2(size, 0), new Vector3(1,size*2,1));
        Spawn(wallPrefab, new Vector2(-size, 0), new Vector3(1,size*2,1));

        int obstacles = 4 + LevelManager.Instance.levelIndex * 2;

        int level = LevelManager.Instance.levelIndex;
        for (int i = 0; i < obstacles; i++)
        {
            Vector2 pos = Random.insideUnitCircle.normalized * Random.Range(2f, 7f);
            Spawn(obstaclePrefab, pos, Vector3.one * Random.Range(1f + level * 0.2f, 1.5f + level * 0.3f));
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
            Destroy(g);

        spawned.Clear();
    }
}

