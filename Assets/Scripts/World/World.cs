using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Collections;
namespace WorldGeneration
{
    public class World : MonoBehaviour
    {
        public WorldSettings settings;
        public int seed;

        public GameObject floorParent;
        public GameObject hillParent;
        public GameObject itemsParent;

        public GameObject temperatureParent;

        public GameObject plantParent;

        private float outdoorsTemp = 25f;

        private Temperature[,] temperatures;
        private Plant[,] plants;
        private Floor[,] hills;

        private Item[,] items;

        private Floor[,] floor;

        private int currentTick = 0;
        private int lastTempTick = 0;
        private bool running = true;

        public System.Action<int, World> OnTickUpdate;

        public bool DamageTile(Tile tile, float amount)
        {
            if (Contains(tile, hills))
            {
                Floor ourFloor = hills[tile.position.x, tile.position.y];
                ourFloor.currentHealth -= amount;
                if (ourFloor.currentHealth <= 0)
                {
                    Destroy(ourFloor.tileObject);

                    VisualizeItem(ourFloor.itemOnDeath, ourFloor.itemAmountOnDeath, ourFloor);

                    tile.tileObject = null;
                    hills[tile.position.x, tile.position.y] = null;
                    floor[tile.position.x, tile.position.y].objectAbove = null;
                    return true;

                }
                return false;
            }
            else
            {
                Debug.LogError($"Floor at {tile.position} is not a damagable tile.");
            }
            return false;
        }

        private void VisualizeItem(Item itemOnDeath, int itemAmountOnDeath, Floor ourFloor)
        {
            if (itemOnDeath != null)
            {
                items[ourFloor.position.x, ourFloor.position.y] = itemOnDeath;
                items[ourFloor.position.x, ourFloor.position.y].currnetAmount = itemAmountOnDeath;

                GameObject itemObject = Instantiate(settings.itemPrefab, ourFloor.worldPosition, Quaternion.identity, itemsParent.transform);
                itemObject.transform.localScale = new Vector3(settings.tileScale, settings.tileScale, 0);
                itemObject.name = $"Item {ourFloor.position.x},{ourFloor.position.y}";
                SpriteRenderer spriteRenderer;
                if (!itemObject.TryGetComponent<SpriteRenderer>(out spriteRenderer))
                {
                    spriteRenderer = itemObject.AddComponent<SpriteRenderer>();

                }
                spriteRenderer.sprite = itemOnDeath.sprite;
                if (itemOnDeath.hasColour)
                {
                    spriteRenderer.color = itemOnDeath.itemColour;
                }
                if (spriteRenderer.sprite == null)
                {
                    Debug.LogError($"Sprite is null at {ourFloor.position.x},{ourFloor.position.y} for item {itemOnDeath.name}");
                }
            }
        }

        public static bool Contains(Tile tile, Tile[,] tiles)
        {
            foreach (Tile t in tiles)
            {
                if (t == tile)
                {
                    return true;
                }
            }
            return false;
        }

        public Floor GetFloorAtPosition(Vector2 position)
        {

            if (position.x < 0 || position.y < 0 || position.x > settings.worldSize.x * settings.tileScale || position.y > settings.worldSize.y * settings.tileScale)
            {
                return null;
            }

            int x = Mathf.RoundToInt(position.x / settings.tileScale);
            int y = Mathf.RoundToInt(position.y / settings.tileScale);
            if (x < 0 || x >= floor.GetLength(0) || y < 0 || y >= floor.GetLength(1))
            {
                return null;
            }
            return floor[x, y];
        }

        public Floor GetHillAtPosition(Vector2 position)
        {

            if (position.x < 0 || position.y < 0 || position.x > settings.worldSize.x * settings.tileScale || position.y > settings.worldSize.y * settings.tileScale)
            {
                return null;
            }

            int x = Mathf.RoundToInt(position.x / settings.tileScale);
            int y = Mathf.RoundToInt(position.y / settings.tileScale);
            if (x < 0 || x >= hills.GetLength(0) || y < 0 || y >= hills.GetLength(1))
            {
                return null;
            }
            return hills[x, y];
        }
        public Plant GetPlantAtPosition(Vector2 position)
        {

            if (position.x < 0 || position.y < 0 || position.x > settings.worldSize.x * settings.tileScale || position.y > settings.worldSize.y * settings.tileScale)
            {
                return null;
            }

            int x = Mathf.RoundToInt(position.x / settings.tileScale);
            int y = Mathf.RoundToInt(position.y / settings.tileScale);
            if (x < 0 || x >= plants.GetLength(0) || y < 0 || y >= plants.GetLength(1))
            {
                return null;
            }
            return plants[x, y];
        }

        public Temperature GetTemperatureAtPosition(Vector2 position)
        {
            if (position.x < 0 || position.y < 0 || position.x > settings.worldSize.x * settings.tileScale || position.y > settings.worldSize.y * settings.tileScale)
            {
                return null;
            }

            int x = Mathf.RoundToInt(position.x / settings.tileScale);
            int y = Mathf.RoundToInt(position.y / settings.tileScale);
            if (x < 0 || x >= temperatures.GetLength(0) || y < 0 || y >= temperatures.GetLength(1))
            {
                return null;
            }
            return temperatures[x, y];
        }
        public Floor[,] GetFloor()
        {
            return floor;
        }

        public Temperature[,] GetTemperature()
        {
            return temperatures;
        }
        private void Awake()
        {
            Random.InitState(seed);
            items = new Item[settings.worldSize.x, settings.worldSize.y];

            floor = GenerateFloor(settings, seed);
            GenerateLakes();
            StartSmoothFloor();
            VisualizeFloor(settings, floor);

            GenerateHills();
            VisualizeHills(settings, hills);


            GenerateTemperature();
            UpdateTemperature(outdoorsTemp);


            GeneratePlants();

            VisualizePlants(settings, plants);

            StartCoroutine(Loop());


        }



        //World Loop
        private IEnumerator Loop()
        {
            running = true;
            while (running)
            {
                if (currentTick >= lastTempTick + 30)//Every 3 seconds
                {
                    UpdateTemperature(currentTick * 0.0001f + outdoorsTemp);
                    lastTempTick = currentTick;
                }



                OnTickUpdate.Invoke(currentTick, this);

                currentTick++;
                yield return new WaitForSeconds(1 / settings.worldTicksPerSecond);
            }
        }
        private void UpdateTemperature(float backgroundTemp)
        {
            Debug.Log($"Background temp is now at {backgroundTemp}.");
            Temperature[,] newTemps = new Temperature[temperatures.GetLength(0), temperatures.GetLength(1)];
            for (int x = 0; x < temperatures.GetLength(0); x++)
            {
                for (int y = 0; y < temperatures.GetLength(1); y++)
                {
                    Temperature currentTemp = temperatures[x, y];
                    Temperature newTemp = new Temperature(currentTemp.position, currentTemp.worldPosition, currentTemp.canChange);
                    newTemp.value = currentTemp.value;
                    if (currentTemp.canChange)
                    {
                        List<Temperature> neighbours = GetNeighbours(temperatures, x, y);

                        newTemp.UpdateTemperature(neighbours);
                    }
                    else
                    {
                        newTemp.value = backgroundTemp;
                    }

                    currentTemp.tempObject.GetComponent<SpriteRenderer>().color = settings.temperatureColourGradient.Evaluate(newTemp.value);
                    newTemp.tempObject = currentTemp.tempObject;

                }
            }
            temperatures = newTemps;
        }

        private void GenerateTemperature()
        {
            temperatures = new Temperature[floor.GetLength(0), floor.GetLength(1)];
            for (int x = 0; x < temperatures.GetLength(0); x++)
            {
                for (int y = 0; y < temperatures.GetLength(1); y++)
                {
                    bool isOnEdge = IsOnEdge(x, y, temperatures.GetLength(0), temperatures.GetLength(1));
                    Temperature temp = new Temperature(new(x, y), new(x * settings.tileScale, y * settings.tileScale), isOnEdge);
                    temp.value = outdoorsTemp;

                    temperatures[x, y] = temp;

                    GameObject tempObject = Instantiate(settings.emptyTile, temp.worldPosition, Quaternion.identity, temperatureParent.transform);
                    tempObject.transform.localScale = new Vector3(settings.tileScale, settings.tileScale, 1);
                    tempObject.name = $"Temp {x},{y}";
                    temp.tempObject = tempObject;
                    SpriteRenderer spriteRenderer;
                    if (!tempObject.TryGetComponent<SpriteRenderer>(out spriteRenderer))
                    {
                        spriteRenderer = tempObject.AddComponent<SpriteRenderer>();
                    }
                    spriteRenderer.sprite = settings.tempDefaultSprite;
                }
            }
        }
        private void VisualizeHills(WorldSettings settings, Floor[,] hills)
        {
            for (int x = 0; x < hills.GetLength(0); x++)
            {
                for (int y = 0; y < hills.GetLength(1); y++)
                {
                    Floor currentFloor = hills[x, y];
                    if (currentFloor != null)
                    {
                        GameObject hill = Instantiate(settings.rockTile, currentFloor.worldPosition, Quaternion.identity, hillParent.transform);
                        hill.transform.localScale = new Vector3(settings.tileScale, settings.tileScale, 1);
                        hill.name = $"Hill {x},{y}";
                        currentFloor.tileObject = hill;

                        SpriteRenderer spriteRenderer;
                        if (!hill.TryGetComponent<SpriteRenderer>(out spriteRenderer))
                        {
                            spriteRenderer = hill.AddComponent<SpriteRenderer>();

                        }
                        spriteRenderer.sprite = Resources.Load<Sprite>($"Tiles/{currentFloor.floorType}");
                        spriteRenderer.sortingLayerID = SortingLayer.NameToID("Rock");
                        //TODO: Change the color of the rock based on the rock type.
                        spriteRenderer.color = currentFloor.rockType == RockType.Granite ? Color.red : Color.green;

                        currentFloor.itemOnDeath.SetColour(spriteRenderer.color);

                        if (spriteRenderer.sprite == null)
                        {
                            Debug.LogError($"Sprite is null at {x},{y} with a tile type of {currentFloor.floorType}");
                        }
                    }
                }
            }
        }
        public void RemovePlant(Vector2Int position)
        {
            Plant plant = plants[position.x, position.y];
            if (plant != null)
            {
                Destroy(plant.tileObject);
                plants[position.x, position.y] = null;
            }
        }
        void GeneratePlants()
        {
            plants = new Plant[floor.GetLength(0), floor.GetLength(1)];


            for (int x = 0; x < floor.GetLength(0); x++)
            {
                for (int y = 0; y < floor.GetLength(1); y++)
                {
                    Floor currentFloor = floor[x, y];
                    if (currentFloor.fertility > 0)
                    {
                        //Plant a random plant here based on weights from settings
                        foreach (PlantCutOff plantCutOff in settings.plantCutoffs)
                        {

                            if (Random.Range(0f, 1f) <= plantCutOff.probability)
                            {
                                //TODO: Make the health of the plant dynamic to the plant type.
                                int plantHealth = 25;

                                Plant plant = plantCutOff.plantType;
                                Plant plantx = new Plant(new(x, y), new(x * settings.tileScale, y * settings.tileScale), plant.plantName, plant.temperatureMin, plant.temperatureMax, plantHealth, plant.fertilityThreshold, plant.itemOnHarvest, plant.amountOfItemOnDeath);
                                plantx.SetPlantGrowthIndex(plant.plantGrowIndex);
                                OnTickUpdate += plantx.UpdatePlant;
                                plants[x, y] = plantx;
                                break;
                            }



                        }
                    }
                }
            }
        }


        void GenerateHills()
        {
            //Find the centers of the hills aka the clusters of rock tiles.
            hills = new Floor[floor.GetLength(0), floor.GetLength(1)];
            for (int x = 0; x < floor.GetLength(0); x++)
            {
                for (int y = 0; y < floor.GetLength(1); y++)
                {
                    Floor currentFloor = floor[x, y];
                    if (currentFloor.isRockTile)
                    {
                        //Check if the tile is surrounded by rock tiles.
                        List<Floor> neighbours = GetNeighbours(floor, x, y, out FloorType mostCommon, out int sameNeighbourCount);
                        if (sameNeighbourCount >= 8)
                        {
                            //TODO: Make the max health of the rock tile based the type of rock.
                            float maxHealth = 100f;

                            Sprite itemSprite = Resources.Load<Sprite>($"Items/Rock");
                            Item itemOnDeath = new Item("Rock", 1, itemSprite, true);

                            float fertility = 0;
                            currentFloor.fertility = fertility;
                            Floor rock = new Floor(currentFloor.position, currentFloor.worldPosition, currentFloor.floorType, currentFloor.rockType, maxHealth, 0, itemOnDeath, 1, fertility);
                            rock.objectBelow = floor[x, y];
                            currentFloor.objectAbove = rock;
                            hills[x, y] = rock;

                        }
                    }
                }
            }
        }
        void GenerateLakes()
        {
            for (int i = 0; i < settings.numOfLakes; i++)
            {
                //Pick a random side of the map, for the lake to start on.
                Vector2Int lakeStart = SelectRandomCorner(settings);

                //Pick a random direction for the lake to go in.
                Vector2Int lakeEnd = SelectRandomCorner(settings);

                //Travel from the start to the end with a degree of randomness, and set the tiles to water.
                Vector2Int currentPos = lakeStart;

                while (currentPos != lakeEnd)
                {

                    //Set the current tile and its neighbours to water.
                    for (int x = currentPos.x - settings.lakeSize; x <= currentPos.x + settings.lakeSize; x++)
                    {
                        for (int y = currentPos.y - settings.lakeSize; y <= currentPos.y + settings.lakeSize; y++)
                        {
                            if (x >= 0 && x < floor.GetLength(0) && y >= 0 && y < floor.GetLength(1))
                            {
                                Floor currentFloor = floor[x, y];
                                currentFloor.floorType = FloorType.Water;
                                currentFloor.rockType = RockType.None;
                                currentFloor.maxHealth = -1f;
                                currentFloor.currentHealth = -1f;
                                currentFloor.walkSpeed = 0;
                                currentFloor.fertility = 0f;

                            }
                        }
                    }

                    //Move to the next position with a degree of randomness.
                    currentPos = GetNextLakePosition(currentPos, lakeEnd, settings.lakeRandomness, settings.lakeSize);
                }
            }
        }
        Vector2Int GetNextLakePosition(Vector2Int currentPos, Vector2Int endPos, float wiggleRoom, int maxDistanceAway)
        {
            //Calculate the direction to the end position.
            Vector2 direction = endPos - currentPos;

            //Add some randomness to the direction.
            direction += new Vector2(Random.Range(-wiggleRoom, wiggleRoom), Random.Range(-wiggleRoom, wiggleRoom));


            //Clamp the direction to one tile in any direction.
            Vector2Int clampedDirection = new Vector2Int(Mathf.RoundToInt(Mathf.Clamp(direction.x, -maxDistanceAway, maxDistanceAway)), Mathf.RoundToInt(Mathf.Clamp(direction.y, -maxDistanceAway, maxDistanceAway)));
            //Move to the next position.
            return currentPos + clampedDirection;
        }

        void StartSmoothFloor()
        {

            int smoothAmount = 1;
            for (int i = 0; i < settings.smoothIterations; i++)
            {
                floor = SmoothFloor(floor, seed, smoothAmount);
                smoothAmount += settings.smoothChange;
            }




        }
        static Floor[,] GenerateFloor(WorldSettings settings, int seed)
        {
            Floor[,] start = new Floor[settings.worldSize.x, settings.worldSize.y];
            float[,] noiseMap = Perlin.Noise.GenerateNoiseMap(settings.worldSize.x, settings.worldSize.y, seed, settings.noiseScale, settings.noiseOctaves, settings.noisePersistence, settings.noiseLacunarity, settings.noiseOffset);

            for (int y = 0; y < settings.worldSize.y; y++)
            {
                for (int x = 0; x < settings.worldSize.x; x++)
                {
                    float currentHeight = noiseMap[x, y];
                    FloorType currentFloorType = FloorType.None;
                    foreach (CutOff cutOff in settings.floorTileTypeCutoffs)
                    {
                        if (currentHeight >= cutOff.cutOff)
                        {
                            currentFloorType = cutOff.tileType;
                        }
                    }
                    RockType rockType = RockType.None;
                    //TODO: Make walkspeed dynamic to floor type.
                    int walkSpeed = 1;

                    //TODO: Make the ferility dynamic to floor type.
                    float fertility = 1f;
                    if (currentFloorType == FloorType.Rock)
                    {
                        rockType = GetRockType(x, y, seed);
                        walkSpeed = 1;
                        fertility = 0f;
                    }
                    Vector2 worldPosition = new Vector2(x * settings.tileScale, y * settings.tileScale);


                    start[x, y] = new Floor(new Vector2Int(x, y), worldPosition, currentFloorType, rockType, -1f, walkSpeed, null, 0, fertility);
                }
            }
            return start;
        }

        static Floor[,] SmoothFloor(Floor[,] tiles, int seed, int leastAmountOfNeighbours)
        {
            Floor[,] smooth = new Floor[tiles.GetLength(0), tiles.GetLength(1)];

            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    Floor currentFloor = tiles[x, y];
                    FloorType mostCommon;
                    List<Floor> neighbours = GetNeighbours(tiles, x, y, out mostCommon, out int sameNeighbourCount);
                    if (mostCommon == FloorType.None) { Debug.LogError($"Most common tile type is None at {x},{y}"); }
                    RockType rockType = RockType.None;
                    if (mostCommon == FloorType.Rock)
                    {
                        rockType = GetRockType(x, y, seed);
                    }




                    //If there are more than 5 neighbours of the same type, then the tile is unchanged.
                    if (sameNeighbourCount >= leastAmountOfNeighbours)
                    {
                        mostCommon = currentFloor.floorType;
                    }
                    //TODO: Make walkspeed dynamic to floor type.
                    int walkSpeed = 1;

                    //TODO: Make fertility dynamic to floor type.
                    float fertility = (mostCommon == FloorType.Water || mostCommon == FloorType.Rock) ? 0f : 1f;

                    smooth[x, y] = new Floor(currentFloor.position, currentFloor.worldPosition, mostCommon, rockType, -1f, walkSpeed, null, 0, fertility);


                }
            }



            return smooth;

        }

        void VisualizePlants(WorldSettings settings, Plant[,] plants)
        {
            for (int x = 0; x < plants.GetLength(0); x++)
            {
                for (int y = 0; y < plants.GetLength(1); y++)
                {
                    Plant currentPlant = plants[x, y];

                    if (currentPlant != null)
                    {
                        ////Debug.Log($"Visualizing plant at {currentPlant.worldPosition}");
                        GameObject plant = Instantiate(settings.emptyTile, currentPlant.worldPosition, Quaternion.identity, plantParent.transform);
                        plant.transform.localScale = new Vector3(settings.tileScale, settings.tileScale, 1);
                        plant.name = $"Plant {x},{y}";
                        currentPlant.tileObject = plant;

                        SpriteRenderer spriteRenderer;
                        if (!plant.TryGetComponent<SpriteRenderer>(out spriteRenderer))
                        {
                            spriteRenderer = plant.AddComponent<SpriteRenderer>();

                        }
                        spriteRenderer.sprite = Resources.Load<Sprite>($"Plants/{currentPlant.plantName}");
                        spriteRenderer.sortingLayerID = SortingLayer.NameToID("Plants");
                        if (spriteRenderer.sprite == null)
                        {
                            Debug.LogError($"Sprite is null at {x},{y} with a plant name of {currentPlant.plantName} at 'Plants/{currentPlant.plantName}'");
                        }
                    }
                }
            }
        }
        void VisualizeFloor(WorldSettings settings, Floor[,] tiles)
        {
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    Floor currentFloor = tiles[x, y];
                    GameObject tileObject = Instantiate(settings.emptyTile, currentFloor.worldPosition, Quaternion.identity, floorParent.transform);
                    tileObject.transform.localScale = new Vector3(settings.tileScale, settings.tileScale, 1);
                    tileObject.name = $"Floor {x},{y}";
                    currentFloor.tileObject = tileObject;
                    SpriteRenderer spriteRenderer;
                    if (!tileObject.TryGetComponent<SpriteRenderer>(out spriteRenderer))
                    {
                        spriteRenderer = tileObject.AddComponent<SpriteRenderer>();

                    }
                    spriteRenderer.sprite = Resources.Load<Sprite>($"Tiles/{currentFloor.floorType}");
                    spriteRenderer.sortingLayerID = SortingLayer.NameToID("Floor");
                    if (spriteRenderer.sprite == null)
                    {
                        Debug.LogError($"Sprite is null at {x},{y} with a tile type of {currentFloor.floorType}");
                    }



                    currentFloor.tileObject = tileObject;
                }
            }
        }

        void ClearFloor(Floor[,] tiles)
        {
            foreach (Floor tile in tiles)
            {
                if (tile.tileObject != null)
                {
                    Destroy(tile.tileObject);
                }
            }

        }
        public static List<Floor> GetNeighbours(Floor[,] tiles, int x, int y, out FloorType mostCommon, out int sameNeighbourCount)
        {
            List<Floor> neighbours = new List<Floor>();
            Dictionary<FloorType, int> tileCounts = new Dictionary<FloorType, int>();
            sameNeighbourCount = 0;
            Floor currentFloor = tiles[x, y];
            FloorType currentFloorType = currentFloor.floorType;

            tileCounts[tiles[x, y].floorType] = 1;



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
                        Floor neighbour = tiles[checkX, checkY];
                        if (neighbour.floorType == currentFloorType)
                        {
                            sameNeighbourCount++;
                        }
                        if (tileCounts.ContainsKey(neighbour.floorType))
                        {
                            tileCounts[neighbour.floorType]++;
                        }
                        else
                        {
                            tileCounts[neighbour.floorType] = 1;
                        }

                        neighbours.Add(neighbour);
                    }
                }
            }
            // Find the FloorType with the highest count
            FloorType mostCommonFloorType = FloorType.None;
            int highestCount = 0;

            foreach (KeyValuePair<FloorType, int> pair in tileCounts)
            {
                if (pair.Value > highestCount)
                {
                    mostCommonFloorType = pair.Key;
                    highestCount = pair.Value;
                }
            }
            mostCommon = mostCommonFloorType;


            return neighbours;
        }

        public static List<Plant> GetNeighbours(Plant[,] plants, int x, int y, out string mostCommon, out int sameNeighbourCount)
        {
            List<Plant> neighbours = new List<Plant>();
            Dictionary<string, int> tileCounts = new Dictionary<string, int>();
            sameNeighbourCount = 0;
            Plant currentFloor = plants[x, y];
            string currentFloorType = "";
            if (currentFloor == null)
            {
                tileCounts[""] = 1;
            }
            else
            {
                currentFloorType = currentFloor.plantName;
                tileCounts[currentFloor.plantName] = 1;
            }
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

                    if (checkX >= 0 && checkX < plants.GetLength(0) && checkY >= 0 && checkY < plants.GetLength(1))
                    {
                        Plant neighbour = plants[checkX, checkY];
                        if (neighbour == null)
                        {
                            if (tileCounts.ContainsKey(""))
                            {
                                tileCounts[""]++;
                            }
                            else
                            {
                                tileCounts[""] = 1;
                            }
                            continue;
                        }
                        if (neighbour.plantName == currentFloorType)
                        {
                            sameNeighbourCount++;
                        }
                        if (tileCounts.ContainsKey(neighbour.plantName))
                        {
                            tileCounts[neighbour.plantName]++;
                        }
                        else
                        {
                            tileCounts[neighbour.plantName] = 1;
                        }

                        neighbours.Add(neighbour);
                    }
                }
            }
            // Find the FloorType with the highest count
            string mostCommonFloorType = "";
            int highestCount = 0;

            foreach (KeyValuePair<string, int> pair in tileCounts)
            {
                if (pair.Value > highestCount)
                {
                    mostCommonFloorType = pair.Key;
                    highestCount = pair.Value;
                }
            }
            mostCommon = mostCommonFloorType;


            return neighbours;
        }

        public static List<Temperature> GetNeighbours(Temperature[,] objects, int x, int y)
        {
            List<Temperature> result = new List<Temperature>();

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

                    if (checkX >= 0 && checkX < objects.GetLength(0) && checkY >= 0 && checkY < objects.GetLength(1))
                    {
                        result.Add(objects[checkX, checkY]);
                    }
                }

            }
            return result;
        }
        private static bool IsOnEdge(int x, int y, int width, int height)
        {
            return x == 0 || x == width - 1 || y == 0 || y == height - 1;
        }

        private static RockType GetRockType(int x, int y, int seed)
        {
            return RockType.Granite;
        }
        private static Vector2Int SelectRandomCorner(WorldSettings settings)
        {
            int side = UnityEngine.Random.Range(0, 4);
            Vector2Int corner = new Vector2Int();
            switch (side)
            {
                case 0:
                    //Top
                    corner = new Vector2Int(UnityEngine.Random.Range(0, settings.worldSize.x), settings.worldSize.y - 1);
                    break;
                case 1:
                    //Right
                    corner = new Vector2Int(settings.worldSize.x - 1, UnityEngine.Random.Range(0, settings.worldSize.y));
                    break;
                case 2:
                    //Bottom
                    corner = new Vector2Int(UnityEngine.Random.Range(0, settings.worldSize.x), 0);
                    break;
                case 3:
                    //Left
                    corner = new Vector2Int(0, UnityEngine.Random.Range(0, settings.worldSize.y));
                    break;
                default:
                    Debug.LogError($"Side is not between 0 and 3, it is {side}");
                    break;
            }
            return corner;
        }

        public void SpawnPlant(Vector2Int newPos, Plant plant)
        {
            //if the new pos is within the bounds of the world.
            if (newPos.x >= 0 && newPos.x < plants.GetLength(0) && newPos.y >= 0 && newPos.y < plants.GetLength(1))
            {
                //if there is no plant at the new position.
                if (plants[newPos.x, newPos.y] == null)
                {
                    //TODO: Make the health of the plant dynamic to the plant type.
                    int plantHealth = 25;
                    Vector2 worldPos = new Vector2(newPos.x * settings.tileScale, newPos.y * settings.tileScale);
                    Plant plantx = new(newPos, worldPos, plant.plantName, plant.temperatureMin, plant.temperatureMax, plantHealth, plant.fertilityThreshold, plant.itemOnHarvest, plant.amountOfItemOnDeath);

                    plantx.SetPlantGrowthIndex(plant.plantGrowIndex);
                    OnTickUpdate += plantx.UpdatePlant;

                    plants[newPos.x, newPos.y] = plantx;


                    GameObject plantGame = Instantiate(settings.emptyTile, worldPos, Quaternion.identity, plantParent.transform);
                    plantGame.transform.localScale = new Vector3(settings.tileScale, settings.tileScale, 1);
                    plantGame.name = $"Plant {newPos.x},{newPos.y}";
                    plantx.tileObject = plantGame;

                    SpriteRenderer spriteRenderer;
                    if (!plantGame.TryGetComponent<SpriteRenderer>(out spriteRenderer))
                    {
                        spriteRenderer = plantGame.AddComponent<SpriteRenderer>();

                    }
                    spriteRenderer.sprite = Resources.Load<Sprite>($"Plants/{plantx.plantName}");
                    spriteRenderer.sortingLayerID = SortingLayer.NameToID("Plants");
                    if (spriteRenderer.sprite == null)
                    {
                        Debug.LogError($"Sprite is null at {newPos.x},{newPos.y} with a plant name of {plantx.plantName}");
                    }
                }
            }
        }
    }
}
