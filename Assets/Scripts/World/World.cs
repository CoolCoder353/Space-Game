using UnityEngine;
using NaughtyAttributes;
namespace WorldGeneration
{
    public class World : MonoBehaviour
    {
        public WorldSettings settings;
        public int seed;




        private Color[] colorMap;
        private Sprite[] textureMap;

        private float[,] noiseMap;

        private Tile[,] tileMap;
        private Vector2 offset = Vector2.zero;

        //TODO: Show colours on screen when in debug mode.
        private void Start()
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
            if (Application.isPlaying)
            {
                if (tileMap == null) { Debug.LogWarning("Tilemap not defined"); return; }
                Debug.Log("Drawing Tiles");
                DrawDebugTiles();
            }
        }


        private void GenerateColourMap()
        {
            colorMap = new Color[settings.weights.Length];
            textureMap = new Sprite[settings.weights.Length];
            foreach (TileWeights weight in settings.weights)
            {
                TileType tile = weight.type;

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

                    tileMap[x, y] = new Tile(type, colour, texture);

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
            for (int y = 0; y < settings.height; y++)
            {
                for (int x = 0; x < settings.width; x++)
                {
                    Vector3 offset = new(x * settings.tileWidth, y * settings.tileHeight, 0);
                    GameObject tile = Instantiate(settings.tileObject, transform.position + offset, settings.tileObject.transform.rotation, gameObject.transform);
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