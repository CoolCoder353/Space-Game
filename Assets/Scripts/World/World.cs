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
            tileIdIndex["Rock"] = 0.6f;


            Random.InitState(seed);
            Debug.Log($"Set world seed to {seed}.");
            noise = Perlin.Noise.GenerateNoiseMap(settings.worldSize.x, settings.worldSize.y, seed, settings.noiseScale, settings.noiseOctaves, settings.noisePersistence, settings.noiseLacunarity, settings.noiseOffset);

            tileBase = Xml.LoadTileData(xmlFileLocation);

            Logger.LogDictionary(tileBase);
            Logger.LogDictionary(tileIdIndex);
            GenerateTiles();
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

                    currentTile.data["floorObject"] = floorObject;

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