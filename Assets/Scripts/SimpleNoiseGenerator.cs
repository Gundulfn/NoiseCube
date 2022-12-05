using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SimpleNoiseGenerator
{
    public static float[,] GenerateSimpleNoiseMap(int mapWidth, int mapHeight)
    {
        float[,] simpleNoiseMap = new float[mapWidth, mapHeight];

        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                simpleNoiseMap[x, z] = Random.Range(0, 2) / 10f;
            }
        }

        return simpleNoiseMap;
    }
}

public static class GaussianBlur
{
    public static float[,] BlurNoiseMap(float[,] noiseMap, float weight)
    {
        int length = noiseMap.GetLength(0);
        int foff = (length - 1) / 2;
        
        float noiseValuesSum = 0;
        float distance = 0;
        float constant = 1f / (2 * Mathf.PI * weight * weight);

        for (int y = -foff; y <= foff; y++)
        {
            for (int x = -foff; x <= foff; x++)
            {
                distance = ((y * y) + (x * x)) / (2 * weight * weight);
                noiseMap[y + foff, x + foff] = constant * Mathf.Exp(-distance);
                noiseValuesSum += noiseMap[y + foff, x + foff];
            }
        }

        for (int y = 0; y < length; y++)
        {
            for (int x = 0; x < length; x++)
            {
                noiseMap[y, x] = noiseMap[y, x] / noiseValuesSum;
            }
        }

        return noiseMap;
    }
}