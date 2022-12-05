using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGenerator
{
    public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCenter)
    {
        float[,] values = NoiseMapGenerator.GenerateNoiseMap(width, height, settings.noiseSettings, sampleCenter);
        
        AnimationCurve heightCurve_threadSafe = new AnimationCurve(settings.heightCurve.keys);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                values[x, z] *= heightCurve_threadSafe.Evaluate(values[x, z]) * settings.heightMultiplier;
                
                if(values[x, z] > maxValue)
                {
                    maxValue = values[x, z];
                }

                if(values[x, z] < minValue)
                {
                    minValue = values[x, z];
                }
            }
        }
        
        return new HeightMap(values, minValue, maxValue);
    }
}

public struct HeightMap
{
    public readonly float[,] values;
    public readonly float minValue;
    public readonly float maxValue;

    public HeightMap(float[,] values, float minValue, float maxValue)
    {
        this.values = values;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}