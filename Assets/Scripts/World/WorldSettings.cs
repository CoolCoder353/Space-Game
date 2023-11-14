using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

namespace WorldGeneration
{
    [CreateAssetMenu]
    public class WorldSettings : ScriptableObject
    {
        public Vector2Int worldSize;

        public int worldTicksPerSecond = 10;

        public int secondsPerDay = 100;

        public int ticksPerDay => worldTicksPerSecond * ticksPerDay;

        public int daysPerYear = 100;

        public int secondsPerYear => secondsPerDay * daysPerYear;

        public int ticksPerYear => secondsPerYear * worldTicksPerSecond;

        [Foldout("Tile Settings")]
        public float tileScale = 1;



        [Foldout("Tile Settings")]
        public GameObject emptyTile;

        [Foldout("Tile Settings")]
        public GameObject rockTile;


        [Foldout("Noise Settings")]
        [Range(0.1f, 1000f)]
        public float noiseScale = 1;
        [Foldout("Noise Settings")]
        [Range(2, 32)]
        public int numPoints = 8;
        [Foldout("Noise Settings")]
        [Range(0.5f, 2f)]
        public float distanceMultiplier = 1;

        [Foldout("Noise Settings")]
        [Range(1, 8)]
        public int octaves = 4;
        [Foldout("Noise Settings")]
        [Range(0f, 1f)]
        public float persistance = 0.5f;

        [Foldout("Noise Settings")]
        [Range(1, 4)]
        public float lacunarity = 2;
        [Foldout("Noise Settings")]
        [Range(0f, 1f)]
        public float blendFactor = 0.5f;


        [Foldout("Floor Generation Settings")]
        public List<CutOff> floorTileTypeCutoffs = new List<CutOff>();

        [Foldout("Tile Smooth Settings")]
        public int smoothIterations = 1;

        [Foldout("Tile Smooth Settings")]
        public int smoothChange = 1;

        [Foldout("Lake Settings")]
        public int lakeSize = 5;

        [Foldout("Lake Settings")]
        public int numOfLakes = 1;

        [Foldout("Lake Settings")]
        public float lakeRandomness = 0.5f;

        [Foldout("Item Settings")]
        public GameObject itemPrefab;

        [Foldout("Plant Settings")]
        public int growthIterations = 1;

        [Foldout("Plant Settings")]
        public int growthChange = 1;

        [Foldout("Temperature Settings")]
        public Gradient temperatureColourGradient;
        [Foldout("Temperature Settings")]
        public AnimationCurve yearlyTemperatureCurve;

        [Foldout("Temperature Settings")]
        public AnimationCurve dailyTemperatureCurve;


    }

    [System.Serializable]
    public class CutOff
    {
        public FloorType tileType;
        [Range(0, 1)]
        public float cutOff;
    }

    public enum FloorType
    {
        grass,
        rock,
        crap
    }


}