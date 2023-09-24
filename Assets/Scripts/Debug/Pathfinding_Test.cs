using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldGeneration;
using NaughtyAttributes;

public class Pathfinding_Test : MonoBehaviour
{

    [MinValue(1), MaxValue(100000)]
    public int amountOfTestCases = 100;

    private World world;
    private Tile[,] map;

    public List<long> speed_Find = new List<long>();

    private List<float> speed_Opt = new List<float>();

    [SerializeField]
    public List<Tile> currentPath = new List<Tile>();

    private long totalTime = 0;
    private Tile start;
    private Tile end;

    private void Start()
    {
        world = (World)FindObjectOfType(typeof(World));
        map = world.GetMap();

        ////TestCases();

        ////Debug.Log("Average time to optimize a path is " + Average(speed_Opt) + " ms.");

    }



    [Button("Re-Test")]
    public void TestCases()
    {
        totalTime = 0;
        List<Tile> result = new List<Tile>();
        for (int i = 0; i > amountOfTestCases; i++)
        {

            var watch_FindPath = System.Diagnostics.Stopwatch.StartNew();

            result = Pathfinder.FindPath(GetRandomTile(), GetRandomTile(), map);
            Debug.Log(result);
            speed_Find.Add(watch_FindPath.ElapsedMilliseconds);
            totalTime += watch_FindPath.ElapsedMilliseconds;

            watch_FindPath.Stop();


            // Check if the path is null or empty
            if (result == null || result.Count == 0)
            {
                // Print no path found
                Debug.Log("No path found");
            }





        }
        ////Debug.Log("The last result given was " + result);
        Debug.Log($"Completed {amountOfTestCases} in {totalTime} ms, averaging {Average(speed_Find)} ms per testcase");
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {

            if (currentPath != null)
            {
                foreach (Tile tile in currentPath)
                {
                    Gizmos.color = Color.cyan;
                    (int x, int y) = tile.Position;

                    Vector3 offset = new(x * world.settings.tileWidth, y * world.settings.tileHeight, 0);
                    Vector3 size = new(world.settings.tileWidth, world.settings.tileHeight, 0);
                    Gizmos.DrawCube(offset, size);
                }
            }
            if (start != null)
            {
                Gizmos.color = Color.green;
                (int x, int y) = start.Position;
                Vector3 offset = new(x * world.settings.tileWidth, y * world.settings.tileHeight, 0);
                Vector3 size = new(world.settings.tileWidth, world.settings.tileHeight, 0);
                Gizmos.DrawCube(offset, size);
            }
            if (end != null)
            {
                Gizmos.color = Color.red;
                (int x, int y) = end.Position;
                Vector3 offset = new(x * world.settings.tileWidth, y * world.settings.tileHeight, 0);
                Vector3 size = new(world.settings.tileWidth, world.settings.tileHeight, 0);
                Gizmos.DrawCube(offset, size);
            }
        }

    }

    private void Update()
    {
        Tile tile = GetClickedTile();

        if (tile != null)
        {
            if (start == null)
            {
                ////  Debug.Log("Set start");
                start = tile;
                (int x, int y) = FindTile(map, tile);
                ////  Debug.Log($"Start pos = ({x},{y})");
            }
            else if (end == null)
            {
                //// Debug.Log("Set end");
                end = tile;
                (int x, int y) = FindTile(map, tile);
                ////   Debug.Log($"End pos = ({x},{y})");

                ////   Debug.Log(start.Walkable() + " and " + end.Walkable());

                currentPath = Pathfinder.FindPath(start, end, map);

                ////  Debug.Log(currentPath);

            }
            else
            {
                ////Debug.Log("Reset");
                start = null;
                end = null;
                currentPath = new List<Tile>();
            }
        }
    }
    // A method that fires when the mouse clicks and returns the tile the mouse clicked on
    Tile GetClickedTile()
    {
        // If the mouse button is pressed
        if (Input.GetMouseButtonDown(0))
        {

            // Get the mouse position in world space
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            ////Debug.Log(mousePos);
            // Declare a small tolerance value
            float tolerance = world.settings.tileWidth;

            // Loop through the rows of the tiles array
            for (int i = 0; i < map.GetLength(0); i++)
            {
                // Loop through the columns of the tiles array
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    // Get the current tile
                    Tile tile = map[i, j];

                    // Calculate the world position of the tile using your code
                    float xpos = i * world.settings.tileWidth;
                    float ypos = j * world.settings.tileHeight;

                    // Create a vector from the tile position
                    Vector3 tilePos = new Vector3(xpos, ypos, -10f);

                    // If the distance between the mouse position and the tile position is less than the tolerance, return the tile
                    if (Vector3.Distance(mousePos, tilePos) < tolerance)
                    {

                        return tile;
                    }
                }
            }

        }

        // If no tile is clicked, return null
        return null;
    }


    private Tile GetRandomTile()
    {
        int x = Random.Range(0, map.GetLength(0));
        int y = Random.Range(0, map.GetLength(1));
        return map[x, y];
    }

    // A method that takes a list of floats and returns the average value of that list
    long Average(List<long> numbers)
    {
        // If the list is empty, return 0
        if (numbers.Count == 0)
        {
            return 0;
        }

        // Declare a variable to store the sum of the numbers
        long sum = 0;

        // Loop through the list and add each number to the sum
        foreach (long number in numbers)
        {
            sum += number;
        }

        // Divide the sum by the count of the list and return the result
        return sum / numbers.Count;
    }

    public (int, int) FindTile(Tile[,] tileMap, Tile x)
    {
        // Loop through the rows of the array
        for (int i = 0; i < tileMap.GetLength(0); i++)
        {
            // Loop through the columns of the array
            for (int j = 0; j < tileMap.GetLength(1); j++)
            {
                // If the current tile is equal to x, return its indices
                if (tileMap[i, j] == x)
                {
                    return (i, j);
                }
            }
        }

        // If x is not found, return (-1, -1)
        return (-1, -1);
    }

}


