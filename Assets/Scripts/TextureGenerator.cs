using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class TextureGenerator
{
    public static Texture2D GenerateTextureFromColorMap(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();

        return texture;
    }

    public static Texture2D GenerateTextureFromHeightMap(HeightMap heightMap)
    {
        int width = heightMap.values.GetLength(0);
        int height = heightMap.values.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(heightMap.minValue, heightMap.maxValue, heightMap.values[x, y]));
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();

        //Save texture as png
        byte[] bytes = texture.EncodeToPNG();
        var dirPath = Application.dataPath + "/../SaveImages/";
        if(!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + "Image" + ".png", bytes);
                
        return texture;
    }
}
