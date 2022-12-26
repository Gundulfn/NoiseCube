using UnityEngine;

public static class NoiseMapGenerator
{
    public enum NormalizeMode { Local, Global };

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCenter)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];
        float frequency = 1;
        float amplitude = 1;

        //Genlik ve süreklilik parametrelerine bağlı haritanın üretilebilecek maksimum yükseklik değeri
        float maxPossibleHeight = 1f;

        //Oktav haritaların tanımlanması
        Vector2[] octaveOffsets = new Vector2[settings.octaves];

        //RTD değeriyle rastgele sayılar üreten obje
        System.Random randomNumberGenerator = new System.Random(settings.seed);

        //Oktav haritalarının ve "maxPossibleHeight" değerinin atanması
        for (int i = 0; i < settings.octaves; i++)
        {
            octaveOffsets[i] = new Vector2(
              randomNumberGenerator.Next(-10000, 10000) + settings.offset.x + sampleCenter.x, randomNumberGenerator.Next(-10000, 10000) - settings.offset.y - sampleCenter.y);

            maxPossibleHeight += amplitude;
            amplitude *= settings.persistence;
        }

        //Haritanın üretim sırasındaki maksimum ve minimum yükseklik değerlerini kaydetmek için
        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        //Harita üretimi başlangıcı
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                frequency = 1;
                amplitude = 1;
                float noiseHeight = 0;

                for (int i = 0; i < settings.octaves; i++)
                {
                    //PGH'ın x ve y koordinatlarını belirleme
                    float sampleX = (x - mapWidth + octaveOffsets[i].x) / settings.scale * frequency;
                    float sampleY = (y - mapHeight + octaveOffsets[i].y) / settings.scale * frequency;

                    //[0, 1] arasında perlin gürültüsü değeri üretme
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);

                    noiseHeight += perlinValue * amplitude;

                    amplitude *= settings.persistence;
                    frequency *= settings.lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }

                if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;

                //Normalleştirme modu "Global" seçeneği için yapılan işlem
                if (settings.normalizeMode == NormalizeMode.Global)
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / 0.9f);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        //Normalleştirme modu "Local" seçeneği için yapılan işlem
        if (settings.normalizeMode == NormalizeMode.Local)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight,
                                                       maxLocalNoiseHeight,
                                                       noiseMap[x, y]);
                }
            }
        }

        return noiseMap;
    }

    public static float Perlin3D(float x, float y, float z)
    {
        float xy = Mathf.PerlinNoise(x, y);
        float yz = Mathf.PerlinNoise(y, z);
        float xz = Mathf.PerlinNoise(x, z);

        float yx = Mathf.PerlinNoise(y, x);
        float zy = Mathf.PerlinNoise(z, y);
        float zx = Mathf.PerlinNoise(z, x);

        float xyz = xy + yz + xz + yx + zy + zx;


        return xyz / 6f;
    }
}

[System.Serializable]
public class NoiseSettings
{
    public NoiseMapGenerator.NormalizeMode normalizeMode = NoiseMapGenerator.NormalizeMode.Global;

    public int seed = 1;
    public Vector2 offset = Vector2.zero;

    public float scale = 25;
    public int octaves = 2;

    [Range(0.1f, 1)]
    public float persistence = .5f;
    public float lacunarity = 2;

    public NoiseSettings() { }
    public void ValidateValues()
    {
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistence = Mathf.Clamp01(persistence);
    }
}