using UnityEngine;
using NaughtyAttributes;

namespace WorldGeneration
{
    [CreateAssetMenu]
    public class WorldSettings : ScriptableObject
    {
        [Foldout("Main")]
        public int width;
        [Foldout("Main")]
        public int height;

        [Foldout("Main")]
        public TileWeights[] weights;

        [Foldout("Main")]
        public GameObject tileObject;

        [Foldout("Noise")]
        public float scale;
        [Foldout("Noise")]
        public int octaves;
        [Foldout("Noise"), Range(0f, 1f)]
        public float persistence;
        [Foldout("Noise"), Range(1.5f, 3.5f)]
        public float lacunarity;

        [Foldout("Tile")]
        public float tileWidth;
        [Foldout("Tile")]
        public float tileHeight;
        [Foldout("Debug")]
        public bool debug;
        [Foldout("Debug")]
        public ColourType debugType;
        [Foldout("Debug")]
        public bool createTiles = true;


    }

    [System.Serializable]
    public class TileWeights
    {
        public string name;
        [Range(0, 1)]
        public float weight;
        public int movementSpeed;
        public Color debugColour;
        public Sprite texture;
        public TileType type;
    }

    [System.Serializable]
    public enum ColourType
    {
        texture,
        colour,
        noise
    }
}