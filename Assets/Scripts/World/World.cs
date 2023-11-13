using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
namespace WorldGeneration
{
    public class World : MonoBehaviour
    {
        public WorldSettings settings;

        public int seed;
        public GameObject tileParent;
        private string xmlFileLocation = "Assets/Resources/Xml/";

        private string tileFileLocation = "Tiles/";
        private Tile[,] map;
        private float[,] noise;

        public Dictionary<string, float> tileIdIndex = new Dictionary<string, float>();
        private Dictionary<string, TileData> tileBase = new Dictionary<string, TileData>();

        private void Awake()
        {
            //TODO: Make this dynamic or smarter in some way.
            tileIdIndex["Grass"] = 0f;
            tileIdIndex["Rock"] = 0.5f;


            Random.InitState(seed);
            Debug.Log($"Set world seed to {seed}.");
            noise = Noise.Worley.GenerateNoiseMap(settings.worldSize.x, settings.worldSize.y, seed, settings.numPoints, settings.noiseScale, settings.distanceMultiplier);

            tileBase = Xml.LoadTileData(xmlFileLocation);

            Logger.LogDictionary(tileBase);
            Logger.LogDictionary(tileIdIndex);
            GenerateTiles();
            SmoothTiles(settings.smoothIterations);
        }

        private void GenerateTiles()
        {
            map = new Tile[settings.worldSize.x, settings.worldSize.y];
            for (int x = 0; x < settings.worldSize.x; x++)
            {
                for (int y = 0; y < settings.worldSize.y; y++)
                {
                    Vector2 worldPos = new Vector2(x * settings.tileScale, y * settings.tileScale);
                    Vector2Int pos = new Vector2Int(x, y);
                    TileData data = IdentifyTile(noise[x, y]);
                    Tile currentTile = new Tile(worldPos, pos, data.health, data.fertilty);
                    Sprite sprite = LoadSprite(data);
                    GameObject floorObject = CreateTileObject(sprite, data, worldPos);
                    currentTile.SetData("tileType", data.tileType);
                    currentTile.SetData("floorObject", floorObject);

                    map[x, y] = currentTile;

                }
            }
        }
        public void SmoothTiles(int smoothingIterations)
        {
            if (smoothingIterations <= 0) { return; }
            Tile[,] smoothedMap = new Tile[settings.worldSize.x, settings.worldSize.y];
            Tile[,] currentMap = map;

            for (int i = 0; i < smoothingIterations; i++)
            {
                for (int x = 0; x < settings.worldSize.x; x++)
                {
                    for (int y = 0; y < settings.worldSize.y; y++)
                    {
                        int neighborCount;
                        string mostCommonTileType;
                        Tile currentTile = currentMap[x, y];


                        GetNeighborStats(currentMap, x, y, out neighborCount, out mostCommonTileType);

                        string tileType = (string)currentTile.GetData("tileType");
                        if (neighborCount <= 3)
                        {
                            tileType = GetRandomNeighborTileType(currentMap, x, y);
                        }

                        // Update the tile's data and GameObject in smoothedMap

                        SpriteRenderer spriteRenderer = ((GameObject)currentTile.GetData("floorObject")).GetComponent<SpriteRenderer>();
                        spriteRenderer.sprite = LoadSprite(tileBase[tileType]);
                        currentTile.SetData("tileType", tileType);

                        smoothedMap[x, y] = currentTile;



                    }
                }

                // Swap the maps for the next iteration
                Tile[,] tempMap = currentMap;
                currentMap = smoothedMap;
                smoothedMap = tempMap;
            }

            map = currentMap;
        }
        public string GetRandomNeighborTileType(Tile[,] map, int x, int y)
        {
            List<string> neighborTileTypes = new List<string>();

            // Check each neighboring tile
            for (int offsetX = -1; offsetX <= 1; offsetX++)
            {
                for (int offsetY = -1; offsetY <= 1; offsetY++)
                {
                    // Skip the current tile and diagonal neighbors
                    if ((offsetX == 0 && offsetY == 0) || (offsetX != 0 && offsetY != 0))
                        continue;

                    int neighborX = x + offsetX;
                    int neighborY = y + offsetY;

                    // Ensure the neighboring tile is within the map bounds
                    if (neighborX >= 0 && neighborX < settings.worldSize.x && neighborY >= 0 && neighborY < settings.worldSize.y)
                    {
                        Tile neighborTile = map[neighborX, neighborY];
                        string neighborTileType = (string)neighborTile.GetData("tileType");
                        neighborTileTypes.Add(neighborTileType);
                    }
                }
            }

            // Return a random neighbor tile type
            if (neighborTileTypes.Count > 0)
            {
                int randomIndex = Random.Range(0, neighborTileTypes.Count);
                return neighborTileTypes[randomIndex];
            }

            // If no neighbors found, return a default tile type
            return "DefaultTileType";
        }
        private void GetNeighborStats(Tile[,] currentMap, int x, int y, out int neighborCount, out string mostCommonTileType)
        {

            int maxX = currentMap.GetLength(0);
            int maxY = currentMap.GetLength(1);

            int sameTypeCount = 0;
            Dictionary<string, int> tileTypeCounts = new Dictionary<string, int>();

            int[] offsets = { -1, 0, 1 };

            for (int offsetX = 0; offsetX < offsets.Length; offsetX++)
            {
                for (int offsetY = 0; offsetY < offsets.Length; offsetY++)
                {
                    int nx = x + offsets[offsetX];
                    int ny = y + offsets[offsetY];

                    // Skip the center tile
                    if (offsetX == 1 && offsetY == 1)
                        continue;

                    if (nx >= 0 && nx < maxX && ny >= 0 && ny < maxY)
                    {
                        string tileType = (string)currentMap[nx, ny].GetData("tileType");

                        sameTypeCount += tileType == (string)currentMap[x, y].GetData("tileType") ? 1 : 0;

                        if (tileTypeCounts.ContainsKey(tileType))
                        {
                            tileTypeCounts[tileType]++;
                        }
                        else
                        {
                            tileTypeCounts[tileType] = 1;
                        }
                    }
                }
            }

            neighborCount = sameTypeCount;

            // Find the most common tile type among the neighbors
            mostCommonTileType = "";
            int maxCount = 0;

            foreach (KeyValuePair<string, int> kvp in tileTypeCounts)
            {
                if (kvp.Value > maxCount)
                {
                    maxCount = kvp.Value;
                    mostCommonTileType = kvp.Key;
                }
            }
        }


        private Sprite LoadSprite(TileData data)
        {
            Sprite result = Resources.Load<Sprite>(tileFileLocation + data.spritePath);
            if (result == null) { Debug.LogWarning($"Could not find the sprite for {data.tileType} at {tileFileLocation + data.spritePath}"); }
            return result;
        }
        private GameObject CreateTileObject(Sprite sprite, TileData data, Vector2 position, float zOffset = 0)
        {
            Vector3 objectPos = new(position.x, position.y, zOffset);
            GameObject tile = Instantiate(settings.emptyTile, objectPos, Quaternion.identity, tileParent.transform);
            tile.name = $"{data.tileType} ({position.x},{position.y})";
            SpriteRenderer spriteRenderer;
            if (!tile.TryGetComponent<SpriteRenderer>(out spriteRenderer))
            {
                spriteRenderer = tile.AddComponent<SpriteRenderer>();

            }
            spriteRenderer.sprite = sprite;

            return tile;

        }
        private TileData IdentifyTile(float index)
        {
            TileData selectedTile = new();
            bool setTile = false;
            foreach (KeyValuePair<string, float> kvp in tileIdIndex)
            {
                if (index >= kvp.Value)
                {
                    if (tileBase.ContainsKey(kvp.Key))
                    {
                        selectedTile = tileBase[kvp.Key];
                        setTile = true;
                    }
                    else
                    {
                        // Handle the case when the key is not present in tileBase
                        Debug.LogWarning($"Key '{kvp.Key}' is not present in tileBase dictionary");
                    }
                }
                else
                {
                    if (!setTile)
                    {
                        Debug.LogWarning($"Could not set tile for, index '{index}'");
                    }

                }
            }
            return selectedTile;
        }
    }
}