using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using Unity.VisualScripting;
namespace WorldGeneration
{
    public class World : MonoBehaviour
    {
        public WorldSettings settings;
        public int seed;
        public GameObject tileParent;

        private GameObject[,] chunks;
        private Color[] colorMap;
        private Sprite[] textureMap;

        private Dictionary<TileType, int> speedMap = new Dictionary<TileType, int>();

        private float[,] noiseMap;

        private Tile[,] tileMap;
        private Vector2 offset = Vector2.zero;

        private Pawn[] pawnsInWorld;

        public Tile[,] GetMap()
        {
            return tileMap;
        }

        public int GetWorldSize()
        {
            if (settings.width != settings.height)
            {
                throw new System.Exception("Width and Height not equal");
            }
            return settings.width;
        }

        public Tile GetTileAtPosition(Vector3 pos)
        {
            Vector2Int coords = GetTileCoordsAtPosition(pos);
            return tileMap[coords.x, coords.y];
        }

        public Vector2Int GetTileCoordsAtPosition(Vector3 pos)
        {
            int x = Mathf.RoundToInt(pos.x / settings.tileWidth);
            int y = Mathf.RoundToInt(pos.y / settings.tileHeight);
            if (x < 0 || x >= settings.width || y < 0 || y >= settings.height)
            {
                Debug.LogWarning($"Tile out of bounds for position ({pos.x}, {pos.y})");
                return Vector2Int.zero;
            }
            return new Vector2Int(x, y);
        }

        public void hideAll()
        {
            foreach (Transform child in tileParent.transform)
            {
                child.gameObject.SetActive(false);
            }

        }

        public void showRange(Vector2 center, Vector2 size)
        {
            foreach (Transform child in tileParent.transform)
            {
                if (child.position.x >= center.x - size.x / 2 && child.position.x <= center.x + size.x / 2 &&
                    child.position.y >= center.y - size.y / 2 && child.position.y <= center.y + size.y / 2)
                {
                    child.gameObject.SetActive(true);
                }
            }
        }
        public void showRange(Vector3 center, Vector2 size)
        {
            showRange(new Vector3(center.x, center.y, 0), size);
        }
        public void showRangeInChunks(Vector2 center, Vector2 size)
        {
            foreach (Transform child in tileParent.transform)
            {
                if (child.position.x >= center.x - size.x / 2 && child.position.x <= center.x + size.x / 2 &&
                    child.position.y >= center.y - size.y / 2 && child.position.y <= center.y + size.y / 2)
                {
                    child.gameObject.SetActive(true);
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        public void SetTile(Vector3 pos, TileType type)
        {
            Vector2Int coords = GetTileCoordsAtPosition(pos);
            Tile newTile = new Tile(type, speedMap[type], colorMap[(int)type], textureMap[(int)type], coords.x, coords.y, coords.x * settings.tileWidth, coords.y * settings.tileHeight);
            Debug.Log($"Setting tile at {coords.x}, {coords.y} from {tileMap[coords.x, coords.y].GetTileType()} to {newTile.GetTileType()}");
            tileMap.SetValue(newTile, coords.x, coords.y);
            UpdateTileObject(coords, newTile);
        }

        private void UpdateTileObject(Vector2Int coords, Tile newTile)
        {
            GameObject chunk = chunks[coords.x / settings.chunkWidth, coords.y / settings.chunkHeight];
            GameObject tileObject = chunk.transform.GetChild(coords.x % settings.chunkWidth * settings.chunkHeight + coords.y).gameObject;
            SetTileImage(tileObject, coords.x, coords.y);
        }

        //TODO: Show colours on screen when in debug mode.
        private void Awake()
        {
            if (seed == 0)
            {
                seed = Random.Range(1, int.MaxValue);
            }
            noiseMap = Perlin.Noise.GenerateNoiseMap(settings.width, settings.height, seed, settings.scale, settings.octaves, settings.persistence, settings.lacunarity, offset);
            Debug.Log(noiseMap);
            GenerateColourMap();
            Debug.Log(colorMap);
            GenerateMap();
            Debug.Log(tileMap);
            if (settings.createTiles) { CreateTiles(); }
        }

        [Button("Regenerate Map")]
        public void ReGenerateMap()
        {
            noiseMap = Perlin.Noise.GenerateNoiseMap(settings.width, settings.height, seed, settings.scale, settings.octaves, settings.persistence, settings.lacunarity, offset);
            Debug.Log(noiseMap);
            GenerateColourMap();
            Debug.Log(colorMap);
            GenerateMap();
            Debug.Log(tileMap);
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying && settings.debug)
            {
                if (tileMap == null) { Debug.LogWarning("Tilemap not defined"); return; }
                Debug.Log("Drawing Tiles");
                DrawDebugTiles();
            }
        }

        // A method that takes a 2D array of tile objects and a tile object x, and returns the row and column indices of x in the array, or (-1, -1) if x is not found




        private void GenerateColourMap()
        {
            colorMap = new Color[settings.textures.Length];
            textureMap = new Sprite[settings.textures.Length];
            foreach (TileTextures weight in settings.textures)
            {
                TileType tile = weight.type;

                speedMap[tile] = weight.movementSpeed;

                colorMap[(int)tile] = weight.debugColour;
                textureMap[(int)tile] = weight.texture;
            }
        }
        private void GenerateMap()
        {
            tileMap = new Tile[settings.width, settings.height];
            // Generate points for a heightmap
            for (int y = 0; y < settings.height; y++)
            {
                for (int x = 0; x < settings.width; x++)
                {
                    //TODO: Determine debug colour for tile

                    TileType type = DetermineTileType(noiseMap[x, y]);
                    Color colour = Color.magenta;
                    if (settings.debugType == ColourType.colour)
                    {
                        colour = colorMap[(int)type];
                    }
                    else if (settings.debugType == ColourType.noise)
                    {
                        colour = new Color(noiseMap[x, y], noiseMap[x, y], noiseMap[x, y]);
                    }

                    Sprite texture = textureMap[(int)type];

                    tileMap.SetValue(new Tile(type, speedMap[type], colour, texture, x, y, x * settings.tileWidth, y * settings.tileHeight), x, y);

                }
            }

        }
        private void DrawDebugTiles()
        {
            Vector3 size = new Vector3(settings.tileWidth, settings.tileHeight, 1);
            for (int y = 0; y < settings.height; y++)
            {
                for (int x = 0; x < settings.width; x++)
                {
                    Tile tile = tileMap[x, y];
                    Gizmos.color = tile.GetColor();
                    Vector3 offset = new(x * settings.tileWidth, y * settings.tileHeight, 0);
                    Gizmos.DrawCube(transform.position + offset, size);
                    ////Debug.Log("Just finished drawing cube at " + (transform.position + offset) + " of a size of " + size + " with the colour " + tile.GetColor());
                }
            }
        }

        private void CreateTiles()
        {
            chunks = new GameObject[Mathf.CeilToInt(settings.width / settings.chunkWidth) + 1, Mathf.CeilToInt(settings.height / settings.chunkHeight) + 1];
            for (int j = 0; j < Mathf.CeilToInt(settings.height / settings.chunkHeight) + 1; j++)
            {
                for (int i = 0; i < Mathf.CeilToInt(settings.width / settings.chunkWidth) + 1; i++)
                {


                    GameObject chunk = new GameObject($"Chunk_({i},{j})");
                    chunk.transform.SetParent(tileParent.transform);
                    chunk.transform.position = new(settings.tileWidth * i * settings.chunkWidth, settings.tileHeight * j * settings.chunkHeight);

                    chunks[i, j] = chunk;
                    ////Debug.Log($"Chunk ({i},{j}) is now at point {i}, {j}");
                }
            }


            for (int y = 0; y < settings.height; y++)
            {
                for (int x = 0; x < settings.width; x++)
                {
                    ////GameObject chunk = tileParent;

                    int i = Mathf.FloorToInt(x / settings.chunkWidth);
                    int j = Mathf.FloorToInt(y / settings.chunkHeight);

                    ////Debug.Log($"Tile should be in chunk ({i},{j})");

                    GameObject chunk = chunks[i, j];
                    Vector3 offset = new(x * settings.tileWidth, y * settings.tileHeight, 0);
                    GameObject tile = Instantiate(settings.tileObject, transform.position + offset, settings.tileObject.transform.rotation, chunk.transform);
                    tile.transform.localScale = new Vector3(settings.tileWidth, settings.tileHeight, 1);
                    SetTileImage(tile, x, y);


                }
            }
        }

        private void SetTileImage(GameObject tileObject, int x, int y)
        {
            Tile tile = tileMap[x, y];
            SpriteRenderer renderer = tileObject.GetComponent<SpriteRenderer>();
            if (settings.debugType == ColourType.colour)
            {
                renderer.color = tile.GetColor();
            }
            else if (settings.debugType == ColourType.texture)
            {
                renderer.sprite = tile.GetTexture();
            }
            else
            {
                Debug.LogWarning("Ahhhhh. I dunno what to do?");
            }
        }

        public Tile GetLeftTile(int x, int y)
        {
            return tileMap[x + 1, y];
        }
        public Tile GetRightTile(int x, int y)
        {
            return tileMap[x - 1, y];
        }
        public Tile GetTopTile(int x, int y)
        {
            return tileMap[x, y - 1];
        }
        public Tile GetBottomTile(int x, int y)
        {
            return tileMap[x, y + 1];
        }

        public Tile GetTopLeftTile(int x, int y)
        {
            return tileMap[x + 1, y - 1];
        }
        public Tile GetTopRightTile(int x, int y)
        {
            return tileMap[x - 1, y - 1];
        }
        public Tile GetBottomLeftTile(int x, int y)
        {
            return tileMap[x - 1, y + 1];
        }
        public Tile GetBottomRightTile(int x, int y)
        {
            return tileMap[x + 1, y + 1];
        }

        private TileType DetermineTileType(float value)
        {
            TileType result = TileType.Space;
            foreach (TileWeights weight in settings.weights)
            {
                if (value < weight.weight)
                {
                    result = weight.type;
                    break;
                }
            }
            return result;
        }




    }
}