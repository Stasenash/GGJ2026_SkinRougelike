using UnityEngine;

[System.Serializable]
public class MaskData
{
    public MaskType type;
    public Color color;

    public float moveSpeedMul = 1f;
    public float damageMul = 1f;
    public float maxHpMul = 1f;
    public float attackRadiusMul = 1f;

    public float dodgeChance = 0f;     // Rabbit
    public bool knockbackOnHit = false; // Goat
}
