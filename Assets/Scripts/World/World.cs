using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using System;
namespace WorldGeneration
{
    public class World : MonoBehaviour
    {
        public WorldSettings settings;
        public int seed;

        private Tile[,] floor;


        private void Awake()
        {
            floor = GenerateFloor(settings, seed);
            Debug.Log($"Floor size: {floor.GetLength(0)}x{floor.GetLength(1)}");
            VisualizeFloor(settings, floor);

        }
        [Button("Smooth Floor")]
        public void SmoothFloor()
        {
            ClearFloor(floor);
            floor = SmoothFloor(floor);
            VisualizeFloor(settings, floor);
        }
        Tile[,] GenerateFloor(WorldSettings settings, int seed)
        {
            Tile[,] start = new Tile[settings.worldSize.x, settings.worldSize.y];
            float[,] noiseMap = Perlin.Noise.GenerateNoiseMap(settings.worldSize.x, settings.worldSize.y, seed, settings.noiseScale, settings.noiseOctaves, settings.noisePersistence, settings.noiseLacunarity, settings.noiseOffset);

            for (int y = 0; y < settings.worldSize.y; y++)
            {
                for (int x = 0; x < settings.worldSize.x; x++)
                {
                    float currentHeight = noiseMap[x, y];
                    TileType currentTileType = TileType.None;
                    foreach (CutOff cutOff in settings.floorTileTypeCutoffs)
                    {
                        if (currentHeight >= cutOff.cutOff)
                        {
                            currentTileType = cutOff.tileType;
                        }
                    }
                    RockType rockType = RockType.None;
                    if (currentTileType == TileType.Rock)
                    {
                        rockType = GetRockType(x, y, seed);
                    }
                    Vector2 worldPosition = new Vector2(x * settings.tileScale, y * settings.tileScale);
                    start[x, y] = new Tile(new Vector2(x, y), worldPosition, currentTileType, rockType);
                }
            }
            return start;
        }

        Tile[,] SmoothFloor(Tile[,] tiles)
        {
            Tile[,] smooth = new Tile[tiles.GetLength(0), tiles.GetLength(1)];

            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    Tile currentTile = tiles[x, y];
                    TileType mostCommon;
                    List<Tile> neighbours = GetNeighbours(tiles, x, y, out mostCommon, out int sameNeighbourCount);
                    if (mostCommon == TileType.None) { Debug.LogError($"Most common tile type is None at {x},{y}"); }
                    RockType rockType = RockType.None;
                    if (mostCommon == TileType.Rock)
                    {
                        rockType = GetRockType(x, y, seed);
                    }




                    //If there are more than 5 neighbours of the same type, then the tile is unchanged.
                    if (sameNeighbourCount >= 5)
                    {
                        mostCommon = currentTile.tileType;
                    }
                    smooth[x, y] = new Tile(currentTile.position, currentTile.worldPosition, mostCommon, rockType);


                }
            }

            //Replace all tiles that are alone with the most common tile type of its neighbours.
            for (int x = 0; x < smooth.GetLength(0); x++)
            {
                for (int y = 0; y < smooth.GetLength(1); y++)
                {
                    Tile currentTile = smooth[x, y];
                    TileType mostCommon;
                    List<Tile> neighbours = GetNeighbours(smooth, x, y, out mostCommon, out int sameNeighbourCount);
                    RockType rockType = RockType.None;
                    if (mostCommon == TileType.Rock)
                    {
                        rockType = GetRockType(x, y, seed);
                    }
                    if (sameNeighbourCount <= 3)
                    {
                        smooth[x, y] = new Tile(currentTile.position, currentTile.worldPosition, mostCommon, rockType);
                    }
                }
            }

            return smooth;

        }

        private RockType GetRockType(int x, int y, int seed)
        {
            return RockType.Granite;
        }

        void VisualizeFloor(WorldSettings settings, Tile[,] tiles)
        {
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    Tile currentTile = tiles[x, y];
                    GameObject tileObject = Instantiate(settings.emptyTile, currentTile.worldPosition, Quaternion.identity);
                    tileObject.transform.localScale = new Vector3(settings.tileScale, settings.tileScale, 1);
                    SpriteRenderer spriteRenderer;
                    if (!tileObject.TryGetComponent<SpriteRenderer>(out spriteRenderer))
                    {
                        spriteRenderer = tileObject.AddComponent<SpriteRenderer>();

                    }
                    spriteRenderer.sprite = Resources.Load<Sprite>($"Tiles/{currentTile.tileType}");

                    if (spriteRenderer.sprite == null)
                    {
                        Debug.LogError($"Sprite is null at {x},{y} with a tile type of {currentTile.tileType}");
                    }



                    currentTile.tileObject = tileObject;
                }
            }
        }

        void ClearFloor(Tile[,] tiles)
        {
            foreach (Tile tile in tiles)
            {
                if (tile.tileObject == null)
                {
                    Debug.LogWarning($"Tile object is null at {tile.position.x},{tile.position.y}");
                }
                else
                {
                    Destroy(tile.tileObject);
                }
            }

        }
        public static List<Tile> GetNeighbours(Tile[,] tiles, int x, int y, out TileType mostCommon, out int sameNeighbourCount)
        {
            List<Tile> neighbours = new List<Tile>();
            Dictionary<TileType, int> tileCounts = new Dictionary<TileType, int>();
            sameNeighbourCount = 0;
            Tile currentTile = tiles[x, y];
            TileType currentTileType = currentTile.tileType;

            tileCounts[tiles[x, y].tileType] = 1;



            //Find neighbours using bitwise operations.
            for (int nx = -1; nx <= 1; nx++)
            {
                for (int ny = -1; ny <= 1; ny++)
                {
                    if (nx == 0 && ny == 0)
                    {
                        continue;
                    }

                    int checkX = x + nx;
                    int checkY = y + ny;

                    if (checkX >= 0 && checkX < tiles.GetLength(0) && checkY >= 0 && checkY < tiles.GetLength(1))
                    {
                        Tile neighbour = tiles[checkX, checkY];
                        if (neighbour.tileType == currentTileType)
                        {
                            sameNeighbourCount++;
                        }
                        if (tileCounts.ContainsKey(neighbour.tileType))
                        {
                            tileCounts[neighbour.tileType]++;
                        }
                        else
                        {
                            tileCounts[neighbour.tileType] = 1;
                        }

                        neighbours.Add(neighbour);
                    }
                }
            }
            // Find the TileType with the highest count
            TileType mostCommonTileType = TileType.None;
            int highestCount = 0;

            foreach (KeyValuePair<TileType, int> pair in tileCounts)
            {
                if (pair.Value > highestCount)
                {
                    mostCommonTileType = pair.Key;
                    highestCount = pair.Value;
                }
            }
            mostCommon = mostCommonTileType;


            return neighbours;
        }

    }
}
