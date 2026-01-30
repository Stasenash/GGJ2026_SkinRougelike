using UnityEngine;
using System.Collections.Generic;

public class MaskDatabase : MonoBehaviour
{
    public static MaskDatabase Instance;

    public List<MaskData> masks = new();

    void Awake()
    {
        Instance = this;
        GenerateMasks();
    }

    void GenerateMasks()
{
    masks.Clear();

    Color baseColor = Random.ColorHSV(0, 1, 0.6f, 1, 0.6f, 1);

    masks.Add(new MaskData {
        type = MaskType.Fox,
        color = baseColor * 0.9f,
        moveSpeedMul = 1.3f
    });

    masks.Add(new MaskData {
        type = MaskType.Wolf,
        color = baseColor * 1.0f,
        damageMul = 1.4f
    });

    masks.Add(new MaskData {
        type = MaskType.Bear,
        color = baseColor * 0.8f,
        maxHpMul = 1.5f
    });

    masks.Add(new MaskData {
        type = MaskType.Rabbit,
        color = baseColor * 1.1f,
        dodgeChance = 0.25f
    });

    masks.Add(new MaskData {
        type = MaskType.Owl,
        color = baseColor * 0.95f,
        attackRadiusMul = 1.4f
    });

    masks.Add(new MaskData {
        type = MaskType.Goat,
        color = baseColor * 1.05f,
        knockbackOnHit = true
    });
}


    public MaskData Get(MaskType type)
    {
        return masks.Find(m => m.type == type);
    }

    public MaskData GetRandom()
    {
        return masks[Random.Range(0, masks.Count)];
    }
}
