using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

namespace WorldGeneration
{
    [CreateAssetMenu]
    public class WorldSettings : ScriptableObject
    {
        public Vector2Int worldSize;

        [Foldout("Tile Settings")]
        public float tileScale = 1;

        [Foldout("Tile Settings")]
        public GameObject emptyTile;


        [Foldout("Noise Settings")]
        public float noiseScale = 1;
        [Foldout("Noise Settings")]
        public int noiseOctaves = 1;
        [Foldout("Noise Settings")]
        [Range(0, 1)]
        public float noisePersistence = 1;
        [Foldout("Noise Settings")]
        public float noiseLacunarity = 1;
        [Foldout("Noise Settings")]
        public Vector2 noiseOffset;


        [Foldout("Floor Generation Settings")]
        public List<CutOff> floorTileTypeCutoffs = new List<CutOff>();




    }

    [System.Serializable]
    public class CutOff
    {
        public TileType tileType;
        [Range(0, 1)]
        public float cutOff;
    }

}