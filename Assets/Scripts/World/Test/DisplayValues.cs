using UnityEngine;
using WorldGeneration;
using NaughtyAttributes;
public class DisplayValues : MonoBehaviour
{
    public Texture2D texture; // Assign a texture to the object in the Unity Editor
    public WorldSettings settings;
    public int seed;

    private void Start()
    {
        Generate();
    }

    [Button]
    public void Generate()
    {
        texture = new Texture2D(settings.worldSize.x, settings.worldSize.y);
        float[,] WorleyNoise = Noise.Worley.GenerateNoiseMap(settings.worldSize.x, settings.worldSize.y, seed, settings.numPoints, settings.noiseScale, settings.distanceMultiplier);

        float[,] PerlinNoise = Noise.Perlin.GenerateNoiseMap(settings.worldSize.x, settings.worldSize.y, seed, settings.noiseScale, settings.octaves, settings.persistance, settings.lacunarity, Vector2.zero);

        float[,] noise = Noise.Combination.CombineTwo(WorleyNoise, PerlinNoise, settings.blendFactor);
        Display(noise);

    }
    public void Display(float[,] values)
    {
        int width = values.GetLength(0);
        int height = values.GetLength(1);

        // Create a new texture or use an existing one
        if (texture == null || texture.width != width || texture.height != height)
        {
            texture = new Texture2D(width, height);
        }

        // Set the pixel colors based on the float values
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float value = values[x, y];
                Color color = new Color(value, value, value); // Use grayscale colors based on the value

                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply(); // Apply the pixel changes to the texture

        // Assign the texture to the object's material
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.mainTexture = texture;
        }
        else
        {
            Debug.LogWarning("Renderer component not found on the object.");
        }
    }
}