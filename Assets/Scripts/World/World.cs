using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
namespace WorldGeneration
{
    public class World : MonoBehaviour
    {
        public WorldSettings settings;
        public int seed;

        public GameObject floorParent;
        public GameObject hillParent;

        private Tile[,] hills;
        private Tile[,] floor;

        public void DamageTile(Tile tile, float amount)
        {
            if (Contains(tile, hills))
            {
                Tile ourTile = hills[tile.position.x, tile.position.y];
                ourTile.currentHealth -= amount;
                if (ourTile.currentHealth <= 0)
                {
                    tile.tileObject = null;
                    hills[tile.position.x, tile.position.y] = null;
                    Destroy(ourTile.tileObject);
                }
            }
            else
            {
                Debug.LogError($"Tile at {tile.position} is not a damagable tile.");
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

        public Tile GetFloorTileAtPosition(Vector2 position)
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

        public Tile GetHillTileAtPosition(Vector2 position)
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

        public Tile[,] GetFloor()
        {
            return floor;
        }


        private void Awake()
        {
            Random.InitState(seed);
            floor = GenerateFloor(settings, seed);
            GenerateLakes();
            StartSmoothFloor();
            VisualizeFloor(settings, floor);

            GenerateHills();
            VisualizeHills(settings, hills);



        }

        private void VisualizeHills(WorldSettings settings, Tile[,] hills)
        {
            for (int x = 0; x < hills.GetLength(0); x++)
            {
                for (int y = 0; y < hills.GetLength(1); y++)
                {
                    Tile currentTile = hills[x, y];
                    if (currentTile != null)
                    {
                        GameObject tileObject = Instantiate(settings.rockTile, currentTile.worldPosition, Quaternion.identity, hillParent.transform);
                        tileObject.transform.localScale = new Vector3(settings.tileScale, settings.tileScale, 1);
                        SpriteRenderer spriteRenderer;
                        if (!tileObject.TryGetComponent<SpriteRenderer>(out spriteRenderer))
                        {
                            spriteRenderer = tileObject.AddComponent<SpriteRenderer>();

                        }
                        spriteRenderer.sprite = Resources.Load<Sprite>($"Tiles/{currentTile.tileType}");
                        //TODO: Change the color of the rock based on the rock type.
                        spriteRenderer.color = currentTile.rockType == RockType.Granite ? Color.gray : Color.white;

                        if (spriteRenderer.sprite == null)
                        {
                            Debug.LogError($"Sprite is null at {x},{y} with a tile type of {currentTile.tileType}");
                        }
                    }
                }
            }
        }

        void GenerateHills()
        {
            //Find the centers of the hills aka the clusters of rock tiles.
            hills = new Tile[floor.GetLength(0), floor.GetLength(1)];
            for (int x = 0; x < floor.GetLength(0); x++)
            {
                for (int y = 0; y < floor.GetLength(1); y++)
                {
                    Tile currentTile = floor[x, y];
                    if (currentTile.isRockTile)
                    {
                        //Check if the tile is surrounded by rock tiles.
                        List<Tile> neighbours = GetNeighbours(floor, x, y, out TileType mostCommon, out int sameNeighbourCount);
                        if (sameNeighbourCount >= 8)
                        {
                            //TODO: Make the max health of the rock tile based the type of rock.
                            float maxHealth = 100f;

                            Tile rock = new Tile(currentTile.position, currentTile.worldPosition, currentTile.tileType, currentTile.rockType, maxHealth);
                            rock.objectBelow = currentTile;
                            currentTile.objectAbove = rock;
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

                Debug.Log($"Lake {i} starts at {lakeStart} and ends at {lakeEnd}");

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
                                Tile currentTile = floor[x, y];
                                currentTile.tileType = TileType.Water;
                                currentTile.rockType = RockType.None;
                                currentTile.maxHealth = -1f;
                                currentTile.currentHealth = -1f;
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
                Debug.Log($"Starting smooth iteration {i}");
                floor = SmoothFloor(floor, seed, smoothAmount);
                smoothAmount += settings.smoothChange;
            }




        }
        static Tile[,] GenerateFloor(WorldSettings settings, int seed)
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
                    start[x, y] = new Tile(new Vector2Int(x, y), worldPosition, currentTileType, rockType, -1f);
                }
            }
            return start;
        }

        static Tile[,] SmoothFloor(Tile[,] tiles, int seed, int leastAmountOfNeighbours)
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
                    if (sameNeighbourCount >= leastAmountOfNeighbours)
                    {
                        mostCommon = currentTile.tileType;
                    }
                    smooth[x, y] = new Tile(currentTile.position, currentTile.worldPosition, mostCommon, rockType, -1f);


                }
            }



            return smooth;

        }


        void VisualizeFloor(WorldSettings settings, Tile[,] tiles)
        {
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    Tile currentTile = tiles[x, y];
                    GameObject tileObject = Instantiate(settings.emptyTile, currentTile.worldPosition, Quaternion.identity, floorParent.transform);
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
                if (tile.tileObject != null)
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
    }
}
