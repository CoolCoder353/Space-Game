using System;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMapRenderer : MonoBehaviour
{
    [SerializeField] public SpriteRenderer spriteRenderer = null;

    // Determining the coloring of the map depending on the height
    [Serializable]
    public struct TerrainLevel
    {
        public string name;
        public float height;
        public Color color;
    }
    [SerializeField] public List<TerrainLevel> terrainLevel = new List<TerrainLevel>();

    // Depending on the type, we draw noise either in black and white or in color
    public void RenderMap(int width, int height, float[,] noiseMap, MapType type)
    {
        if (type == MapType.Noise)
        {
            ApplyColorMap(width, height, GenerateNoiseMap(width, height, noiseMap));
        }
        else if (type == MapType.Color)
        {
            ApplyColorMap(width, height, GenerateColorMap(width, height, noiseMap));
        }
    }

    // Create texture and sprite to display
    private void ApplyColorMap(int width, int height, Color[] colors)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(colors);
        texture.Apply();

        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f); ;
    }

    // Convert an array with noise data into an array of black and white colors, for transmission to the texture
    private Color[] GenerateNoiseMap(int width, int height, float[,] noiseMap)
    {
        Color[] colorMap = new Color[width * height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }

        }
        return colorMap;
    }

    // Convert an array with noise data into an array of colors, depending on the height, for transmission to the texture
    private Color[] GenerateColorMap(int width, int height, float[,] noiseMap)
    {
        Color[] colorMap = new Color[noiseMap.Length];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Base color with the highest value range
                colorMap[y * width + x] = terrainLevel[terrainLevel.Count - 1].color;
                foreach (var level in terrainLevel)
                {
                    // If the noise falls into a lower range, then use it
                    if (noiseMap[x, y] < level.height)
                    {
                        colorMap[y * width + x] = level.color;
                        break;
                    }
                }
            }

        }



        return colorMap;
    }
}