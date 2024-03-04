using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ColorManager
{
    private Color[] cellColors;
    private const int maxColors = 12;

    public ColorManager()
    {
        cellColors = new Color[maxColors];
        for (int i = 0; i < maxColors; ++i)
        {
            cellColors[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        }
    }

    public Color getColor(int i)
    {
        return cellColors[i];
    }
}