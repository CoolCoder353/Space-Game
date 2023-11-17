using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
using System.Collections;
namespace WorldGeneration
{
    public class World : MonoBehaviour
    {
        public WorldSettings settings;

        public int seed;
        public GameObject tileParent;

        public GameObject temperatureParent;

        public GameObject rockParent;

        private float outdoorTemperature = 25f;
        private string xmlFileLocation = "Assets/Resources/Xml/";

        private string tileFileLocation = "Tiles/";
        private Tile[,] map;
        private float[,] noise;

        public Dictionary<string, float> tileIdIndex = new Dictionary<string, float>();
        private Dictionary<string, TileData> tileBase = new Dictionary<string, TileData>();
        private static readonly int[,] Directions = new int[,]
        {
            { -1, 0 },
            { 1, 0 },
            { 0, -1 },
            { 0, 1 },
            { -1, -1 },
            { -1, 1 },
            { 1, -1 },
            { 1, 1 }
        };


        private bool isTemperatureVisualizationOn = false;
        private Gradient temperatureGradient;

        private void Awake()
        {
            //TODO: Make this dynamic or smarter in some way.

            tileIdIndex["rock"] = 0f;
            tileIdIndex["grass"] = 0.3f;

            Random.InitState(seed);
            Debug.Log($"Set world seed to {seed}.");

            float[,] WorleyNoise = Noise.Worley.GenerateNoiseMap(settings.worldSize.x, settings.worldSize.y, seed, settings.numPoints, settings.noiseScale, settings.distanceMultiplier);

            float[,] PerlinNoise = Noise.Perlin.GenerateNoiseMap(settings.worldSize.x, settings.worldSize.y, seed, settings.noiseScale, settings.octaves, settings.persistance, settings.lacunarity, Vector2.zero);

            noise = Noise.Combination.CombineTwo(WorleyNoise, PerlinNoise, settings.blendFactor);
            tileBase = Xml.LoadTileData(xmlFileLocation);



            GenerateTiles();
            GenerateLakes();
            SmoothTiles(settings.smoothIterations);


            StartCoroutine(Tick());
        }

        private IEnumerator Tick()
        {
            WaitForSecondsRealtime wait = new WaitForSecondsRealtime(1f / settings.worldTicksPerSecond);
            int tick = 0;
            int day = 0;
            int year = 0;
            float timeOfDay = 0;
            float timeOfYear = 0;
            int updateTemperatureEveryNTicks = 1; // Adjust this value as needed

            while (true)
            {
                tick++;

                timeOfDay += Mathf.Abs(1f / (settings.worldTicksPerSecond * settings.secondsPerDay));
                if (timeOfDay > 1) { Debug.LogWarning($"New day! Now at day {day}"); timeOfDay -= 1; day++; }

                timeOfYear += Mathf.Abs(1f / (settings.worldTicksPerSecond * settings.secondsPerYear));
                if (timeOfYear > 1) { Debug.LogWarning($"New year! Now at year {year}"); timeOfYear -= 1; year++; }

                if (tick % updateTemperatureEveryNTicks == 0)
                {
                    UpdateTemperature(timeOfDay, timeOfYear);
                }

                yield return wait;
            }
        }
        private void GenerateLakes()
        {
            if (settings.numOfLakes <= 0) { return; }
            for (int i = 0; i < settings.numOfLakes; i++)
            {

                Vector2Int start = SelectRandomCorner(settings.worldSize.x, settings.worldSize.y);
                Vector2Int end = SelectRandomCorner(settings.worldSize.x, settings.worldSize.y, start);

                Vector2Int current = start;
                while (current != end)
                {
                    current = MoveTowards(current, end, settings.lakeRandomness);

                    // Change everything in an x radius from this position to water
                    ChangeTilesToWater(current, settings.lakeSize);
                }

                // Change the end tile + x radius to water
                ChangeTilesToWater(end, settings.lakeSize);
            }
        }
        private void ChangeTilesToWater(Vector2Int center, int radius)
        {
            for (int x = Mathf.Max(0, center.x - radius); x <= Mathf.Min(settings.worldSize.x - 1, center.x + radius); x++)
            {
                for (int y = Mathf.Max(0, center.y - radius); y <= Mathf.Min(settings.worldSize.y - 1, center.y + radius); y++)
                {
                    if (Vector2Int.Distance(new Vector2Int(x, y), center) <= radius)
                    {
                        map[x, y].SetData("tileType", "water");
                        map[x, y].SetData("temperature", outdoorTemperature);

                        map[x, y].UpdateTileData(this, tileBase["water"]);
                    }
                }
            }
        }
        private void UpdateTemperature(float timeOfDay, float timeOfYear)
        {
            float time = Time.time;
            float dailyTemperature = settings.dailyTemperatureCurve.Evaluate(timeOfDay);
            float yearlyTemperature = settings.yearlyTemperatureCurve.Evaluate(timeOfYear);
            float outdoorTemperature = dailyTemperature + yearlyTemperature;
            //Debug.Log($"Outdoor temperature: {outdoorTemperature}");

            float[,] averageNeighborTemperatures = new float[settings.worldSize.x, settings.worldSize.y];

            for (int x = 0; x < settings.worldSize.x; x++)
            {
                for (int y = 0; y < settings.worldSize.y; y++)
                {
                    averageNeighborTemperatures[x, y] = GetAverageNeighborTemperature(x, y);
                }
            }

            for (int x = 0; x < settings.worldSize.x; x++)
            {
                for (int y = 0; y < settings.worldSize.y; y++)
                {
                    float newTemperature;

                    if (x == 0 || y == 0 || x == settings.worldSize.x - 1 || y == settings.worldSize.y - 1)
                    {
                        newTemperature = outdoorTemperature;
                    }
                    else
                    {
                        newTemperature = averageNeighborTemperatures[x, y];
                    }

                    if ((float)map[x, y].GetData("temperature") != newTemperature)
                    {
                        map[x, y].SetData("temperature", newTemperature);

                        GameObject overlay = map[x, y].GetData("temperatureOverlay") as GameObject;

                        float normalizedTemperature = (newTemperature + 100) / 200; // Normalize temperature to the range [0, 1]
                        Color color = settings.temperatureColourGradient.Evaluate(normalizedTemperature);
                        overlay.GetComponent<SpriteRenderer>().color = color;
                    }
                }
            }
        }
        private float GetAverageNeighborTemperature(int x, int y)
        {
            float totalTemperature = 0f;
            int count = 0;

            // Loop over the directions
            for (int i = 0; i < 4; i++)
            {
                int newX = x + Directions[i, 0];
                int newY = y + Directions[i, 1];

                // Check if the new coordinates are within the map boundaries
                if (newX >= 0 && newX < settings.worldSize.x && newY >= 0 && newY < settings.worldSize.y)
                {
                    totalTemperature += (float)map[newX, newY].GetData("temperature");
                    count++;
                }
            }

            return totalTemperature / count;
        }
        public void ToggleTemperatureVisualization()
        {
            isTemperatureVisualizationOn = !isTemperatureVisualizationOn;

            for (int x = 0; x < settings.worldSize.x; x++)
            {
                for (int y = 0; y < settings.worldSize.y; y++)
                {
                    Tile tile = map[x, y];
                    GameObject overlay = tile.GetData("temperatureOverlay") as GameObject;

                    if (isTemperatureVisualizationOn)
                    {

                        overlay.SetActive(true);
                    }
                    else
                    {
                        overlay.SetActive(false);
                    }
                }
            }
        }
        private bool IsSurroundedByRockTiles(int x, int y)
        {
            if (x > 0 && x < noise.GetLength(0) - 1 && y > 0 && y < noise.GetLength(1) - 1)
            {
                return IdentifyTile(noise[x + 1, y]).tileType == "Rock" &&
                       IdentifyTile(noise[x - 1, y]).tileType == "Rock" &&
                       IdentifyTile(noise[x, y + 1]).tileType == "Rock" &&
                       IdentifyTile(noise[x, y - 1]).tileType == "Rock";
            }
            return false;
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
                    GameObject floorObject = CreateTileObject(sprite, data.tileType, worldPos);

                    //Load the a square sprite for the overlay
                    Sprite overlaySprite = LoadSprite(tileFileLocation + "Overlay/temperature");
                    GameObject temperatureOverlay = CreateTileObject(overlaySprite, "TemperatureOverlay", worldPos, -1f, temperatureParent);

                    currentTile.SetData("tileType", data.tileType);
                    currentTile.SetData("floorObject", floorObject);
                    currentTile.SetData("temperature", outdoorTemperature);
                    currentTile.SetData("temperatureOverlay", temperatureOverlay);

                    map[x, y] = currentTile;


                    // Check if the current tile is a rock tile and if it's surrounded by rock tiles
                    if (data.tileType == "Rock" && IsSurroundedByRockTiles(x, y))
                    {
                        Sprite rockSprite = LoadSprite(tileFileLocation + "Rock/rock");
                        GameObject rockObject = CreateTileObject(rockSprite, "Rock", worldPos, -0.1f, rockParent);
                        currentTile.SetData("rockObject", rockObject);
                    }



                }
            }
        }
        private void SmoothTiles(int smoothingIterations)
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
                            //tileType = GetRandomNeighborTileType(currentMap, x, y);
                            tileType = mostCommonTileType;
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
        private string GetRandomNeighborTileType(Tile[,] map, int x, int y)
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
        private GameObject CreateTileObject(Sprite sprite, string name, Vector2 position, float zOffset = 0, GameObject parent = null)
        {
            Vector3 objectPos = new(position.x, position.y, zOffset);
            GameObject tile = Instantiate(settings.emptyTile, objectPos, Quaternion.identity, parent == null ? tileParent.transform : parent.transform);
            tile.name = $"{name} ({position.x},{position.y})";
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

        //________STATIC METHODS________\\
        public Sprite LoadSprite(TileData data)
        {
            Sprite result = Resources.Load<Sprite>(tileFileLocation + data.spritePath);
            if (result == null) { Debug.LogWarning($"Could not find the sprite for {data.tileType} at {tileFileLocation + data.spritePath}"); }
            return result;
        }
        public static Sprite LoadSprite(string path)
        {
            Sprite result = Resources.Load<Sprite>(path);
            if (result == null) { Debug.LogWarning($"Could not find the sprite at {path}"); }
            return result;
        }
        private static Vector2Int MoveTowards(Vector2Int current, Vector2Int target, float randomness)
        {
            Vector2 direction = ((Vector2)target - current).normalized;

            // Add randomness to the direction
            direction += new Vector2(UnityEngine.Random.Range(-randomness, randomness), UnityEngine.Random.Range(-randomness, randomness));

            // Calculate the new position
            Vector2 newPosition = current + direction;

            // Return the new position as a Vector2Int
            return Vector2Int.RoundToInt(newPosition);
        }
        public static Vector2Int SelectRandomCorner(int mapWidth, int mapHeight, Vector2Int excludedSide = default)
        {
            int randomSide = Random.Range(0, 4);

            Vector2Int randomPoint = Vector2Int.zero;

            switch (randomSide)
            {
                case 0: // Top
                    randomPoint = new Vector2Int(Random.Range(0, mapWidth), mapHeight);
                    if (randomPoint == excludedSide)
                    {
                        randomPoint = SelectRandomCorner(mapWidth, mapHeight, excludedSide);
                    }
                    break;
                case 1: // Right
                    randomPoint = new Vector2Int(mapWidth, Random.Range(0, mapHeight));
                    if (randomPoint == excludedSide)
                    {
                        randomPoint = SelectRandomCorner(mapWidth, mapHeight, excludedSide);
                    }
                    break;
                case 2: // Bottom
                    randomPoint = new Vector2Int(Random.Range(0, mapWidth), 0);
                    if (randomPoint == excludedSide)
                    {
                        randomPoint = SelectRandomCorner(mapWidth, mapHeight, excludedSide);
                    }
                    break;
                case 3: // Left
                    randomPoint = new Vector2Int(0, Random.Range(0, mapHeight));
                    if (randomPoint == excludedSide)
                    {
                        randomPoint = SelectRandomCorner(mapWidth, mapHeight, excludedSide);
                    }
                    break;
            }

            return randomPoint;
        }


    }
}