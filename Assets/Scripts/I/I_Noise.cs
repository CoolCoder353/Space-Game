using UnityEngine;
using System.Collections.Generic;
using System;
namespace Noise
{
    public static class Perlin
    {
        public static float[,] GenerateNoiseMap(int width, int height, int seed, float scale, int octaves, float persistence, float lacunarity, Vector2 offset)
        {
            // An array of vertex data, a one-dimensional view will help get rid of unnecessary cycles later
            float[,] noiseMap = new float[width, height];

            // Seed element
            System.Random rand = new System.Random(seed);

            // Octave shift to get a more interesting picture with overlapping
            Vector2[] octavesOffset = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                // Also use external position shift
                float xOffset = rand.Next(-100000, 100000) + offset.x;
                float yOffset = rand.Next(-100000, 100000) + offset.y;
                octavesOffset[i] = new Vector2(xOffset / width, yOffset / height);
            }

            if (scale < 0)
            {
                scale = 0.0001f;
            }

            // For a more visually pleasant zoom shift our view to the center
            float halfWidth = width / 2f;
            float halfHeight = height / 2f;

            // Generate points for a heightmap
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Set the values for the first octave
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;
                    float superpositionCompensation = 0;

                    // Octave Overlay Processing
                    for (int i = 0; i < octaves; i++)
                    {
                        // Calculate the coordinates to get from Noise Perlin
                        float xResult = (x - halfWidth) / scale * frequency + octavesOffset[i].x * frequency;
                        float yResult = (y - halfHeight) / scale * frequency + octavesOffset[i].y * frequency;

                        // Obtaining Altitude from the PRNG
                        float generatedValue = Mathf.PerlinNoise(xResult, yResult);
                        // Octave overlay
                        noiseHeight += generatedValue * amplitude;
                        // Compensate octave overlay to stay in a range [0,1]
                        noiseHeight -= superpositionCompensation;

                        // Calculation of amplitude, frequency and superposition compensation for the next octave
                        amplitude *= persistence;
                        frequency *= lacunarity;
                        superpositionCompensation = amplitude / 2;
                    }

                    // Save heightmap point
                    // Due to the superposition of octaves, there is a chance of going out of the range [0,1]
                    noiseMap[x, y] = Mathf.Clamp01(noiseHeight);
                }
            }

            return noiseMap;
        }
    }
    public static class Worley
    {

        public static float[,] GenerateNoiseMap(int width, int height, int seed, int numPoints, float scale, float distanceMultiplier)
        {
            float[,] noiseMap = new float[width, height];

            System.Random rand = new System.Random(seed); // Set the random seed

            // Generate random points
            Vector2[] points = new Vector2[numPoints];
            for (int i = 0; i < numPoints; i++)
            {
                points[i] = new Vector2((rand.Next(width) - width / 2f) / scale, (rand.Next(height) - height / 2f) / scale);
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float sampleX = (x - width / 2f) / scale;
                    float sampleY = (y - height / 2f) / scale;

                    float minDistance = float.MaxValue;

                    foreach (Vector2 point in points)
                    {
                        float distance = Vector2.Distance(point, new Vector2(sampleX, sampleY));

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                        }
                    }

                    noiseMap[x, y] = minDistance * distanceMultiplier;
                }
            }

            return noiseMap;
        }
    }

    public static class Combination
    {
        public static float[,] CombineTwo(float[,] mapOne, float[,] mapTwo, float blendFactor)
        {
            int width = mapOne.GetLength(0);
            int height = mapOne.GetLength(1);

            float[,] combinedNoiseMap = new float[width, height];

            // Blend the two noise maps together
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++) { combinedNoiseMap[x, y] = mapOne[x, y] * (1 - blendFactor) + mapTwo[x, y] * blendFactor; }
            }

            return combinedNoiseMap;
        }

    }


}