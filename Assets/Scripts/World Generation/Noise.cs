using UnityEngine;

namespace Perlin
{
    static class Noise
    {
        public static float[,] Generate(int xSize, int ySize, float scale, int seed)
        {
            float realscale = scale;
            float[,] noiseMap = new float[xSize, ySize];
            for (int y = 0; y < ySize; y++)
            {
                for (int x = 0; x < xSize; x++)
                {

                    float xCoord = (float)x / xSize * scale + seed;
                    float yCoord = (float)x / ySize * scale + seed;


                    noiseMap[x, y] = Mathf.PerlinNoise(xCoord, yCoord);

                    Debug.Log(x + ":" + y + " got coords " + xCoord + ":" + yCoord + " which gave " + noiseMap[x, y]);
                }
            }

            return noiseMap;

        }
    }
}


