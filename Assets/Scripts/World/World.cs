using UnityEngine;
using System.Collections.Generic;

namespace WorldGeneration
{
    public class World : MonoBehaviour
    {
        public WorldSettings settings;

        public int seed;
        public GameObject tileParent;
        private string xmlFileLocation = "Assets/Resources/Xml/";
        private Tile[,] map;
        private float[,] noise;
        public Dictionary<string, float> tileIdIndex = new Dictionary<string, float>();
        private Dictionary<string, TileData> tileBase = new Dictionary<string, TileData>();

        private void Awake()
        {
            Random.InitState(seed);

            noise = Perlin.Noise.GenerateNoiseMap(settings.worldSize.x, settings.worldSize.y, seed, settings.noiseScale, settings.noiseOctaves, settings.noisePersistence, settings.noiseLacunarity, settings.noiseOffset);

            tileBase = Xml.LoadTileData(xmlFileLocation);

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
                    map[x, y] = new Tile(worldPos, pos, data.health, data.fertilty);
                }
            }
        }

        private TileData IdentifyTile(float index)
        {
            TileData selectedTile = null;
            foreach (KeyValuePair<string, float> kvp in tileIdIndex)
            {
                if (index > kvp.Value)
                {
                    selectedTile = tileBase[kvp.Key];
                }
                else
                {
                    break;
                }
            }
            return selectedTile;
        }
    }
}