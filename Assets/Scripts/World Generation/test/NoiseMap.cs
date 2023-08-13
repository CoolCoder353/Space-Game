using UnityEngine;

public enum MapType
{
    Noise,
    Color
}

public class NoiseMap : MonoBehaviour
{
    // Input data for our noise generator
    [SerializeField] public int width;
    [SerializeField] public int height;
    [SerializeField] public float scale;

    [SerializeField] public int octaves;
    [SerializeField] public float persistence;
    [SerializeField] public float lacunarity;

    [SerializeField] public int seed;
    [SerializeField] public Vector2 offset;

    [SerializeField] public MapType type = MapType.Noise;

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        // Generate a map
        float[] noiseMap = NoiseMapGenerator.GenerateNoiseMap(width, height, seed, scale, octaves, persistence, lacunarity, offset);

        // Pass the map to the renderer
        NoiseMapRenderer mapRenderer = FindObjectOfType<NoiseMapRenderer>();
        mapRenderer.RenderMap(width, height, noiseMap, type);
    }
}