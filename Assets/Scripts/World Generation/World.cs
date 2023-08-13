using System;
using Unity.Mathematics;
using UnityEngine;

public class World : MonoBehaviour
{

    public int size = 15;
    public float squareSize = 0.5f;
    public int seed = int.MinValue;
    public float noiseScale = 0.4f;

    private Tile[,] grid;

    void Start()
    {
        if (seed == int.MinValue)
        {
            seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }
        grid = new Tile[size, size];
        float[,] noiseMap = GenerateNoiseMap(size, seed, noiseScale);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                TileType type = DetermineTileType(noiseMap[x, y]);
                Tile tile = new(type);
                grid[x, y] = tile;

            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Tile tile = grid[x, y];
                if (tile.GetTileType() == TileType.grass)
                {
                    Gizmos.color = Color.green;
                }
                else if (tile.GetTileType() == TileType.water)
                {
                    Gizmos.color = Color.blue;
                }
                Vector3 pos = new(x, y, 0);
                Gizmos.DrawCube(pos + transform.position, Vector3.one);
            }
        }
    }

    //TODO: Make some smart logic for determining this crap
    private TileType DetermineTileType(float noiseValue)
    {
        TileType type = TileType.water;
        if (noiseValue > 0.4f)
        {
            type = TileType.grass;
        }

        return type;
    }

    private float[,] GenerateNoiseMap(int size, int seed, float scale)
    {

        return Perlin.Noise.Generate(size, size, scale, seed);
    }


    void Update()
    {

    }
}
