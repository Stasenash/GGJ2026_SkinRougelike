using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager Instance;

    private Color[] palette;

    void Awake()
    {
        Instance = this;
        GeneratePalette();
    }

    void GeneratePalette()
    {
        palette = new Color[4];
        Color baseColor = Random.ColorHSV(0,1,0.5f,1,0.5f,1);

        for (int i = 0; i < palette.Length; i++)
            palette[i] = baseColor * Random.Range(0.6f, 1.2f);
    }

    public Color GetEnemyColor()
    {
        return palette[Random.Range(0, palette.Length)];
    }
}
