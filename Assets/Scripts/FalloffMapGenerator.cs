using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FalloffMapGenerator
{
    public static float[,] GenerateFalloffMap(int size)
    {
        float[,] falloffMap = new float[size, size];

        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size * 2 - 1;
                float z = j / (float)size * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(z));
                falloffMap[i, j] = Evaluate(value);
            }

        return falloffMap;
    }

    static float Evaluate(float value)
    {
        float a = 3;
        float b = 3.2f;

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow( (b - b * value), a) );
    }
}
