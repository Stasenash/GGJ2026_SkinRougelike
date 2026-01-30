using UnityEngine;

public class ArenaGenerator : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject obstaclePrefab;

    void Start()
    {
        Generate();
    }

    void Generate()
    {
        float size = 10f;

        Instantiate(wallPrefab, new Vector2(0, size), Quaternion.identity).transform.localScale = new Vector3(size*2,1,1);
        Instantiate(wallPrefab, new Vector2(0, -size), Quaternion.identity).transform.localScale = new Vector3(size*2,1,1);
        Instantiate(wallPrefab, new Vector2(size, 0), Quaternion.identity).transform.localScale = new Vector3(1,size*2,1);
        Instantiate(wallPrefab, new Vector2(-size, 0), Quaternion.identity).transform.localScale = new Vector3(1,size*2,1);

        for (int i = 0; i < 10; i++)
        {
            Vector2 pos = Random.insideUnitCircle * 7f;
            Instantiate(obstaclePrefab, pos, Quaternion.identity);
        }
    }
}
