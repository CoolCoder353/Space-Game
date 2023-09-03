using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldGeneration;
using NaughtyAttributes;

public class Pathfinding_Test : MonoBehaviour
{

    [MinValue(1), MaxValue(100000)]
    public int amountOfTestCases = 100;

    private Pathfinder pathfinder;
    private World world;
    private Tile[,] map;

    private List<float> speed_Find = new List<float>();

    private List<float> speed_Opt = new List<float>();

    [SerializeField]
    public List<Tile> currentPath = new List<Tile>();

    private Tile start;
    private Tile end;

    private void Start()
    {
        world = (World)FindObjectOfType(typeof(World));
        map = world.GetMap();
        pathfinder = new Pathfinder(map);

        TestCases();

        ////Debug.Log("Average time to optimize a path is " + Average(speed_Opt) + " ms.");

    }



    [Button("Re-Test")]
    public void TestCases()
    {
        List<Tile> result = new List<Tile>();
        for (int i = 0; i > amountOfTestCases; i++)
        {

            var watch_FindPath = System.Diagnostics.Stopwatch.StartNew();

            result = pathfinder.FindPath(GetRandomTile(), GetRandomTile());

            watch_FindPath.Stop();
            speed_Find.Add(watch_FindPath.ElapsedMilliseconds);

            // Check if the path is null or empty
            if (result == null || result.Count == 0)
            {
                // Print no path found
                Debug.Log("No path found");
            }





        }
        ////Debug.Log("The last result given was " + result);
        Debug.Log("Average time to find a path is " + Average(speed_Find) + " ms.");
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (start != null)
            {
                Gizmos.color = Color.green;
                (int x, int y) = world.FindTile(start);
                Vector3 offset = new(x * world.settings.tileWidth, y * world.settings.tileHeight, 0);
                Vector3 size = new(world.settings.tileWidth, world.settings.tileHeight, 0);
                Gizmos.DrawCube(offset, size);
            }
            if (end != null)
            {
                Gizmos.color = Color.red;
                (int x, int y) = world.FindTile(end);
                Vector3 offset = new(x * world.settings.tileWidth, y * world.settings.tileHeight, 0);
                Vector3 size = new(world.settings.tileWidth, world.settings.tileHeight, 0);
                Gizmos.DrawCube(offset, size);
            }
            if (currentPath != null)
            {
                foreach (Tile tile in currentPath)
                {
                    Debug.Log("Drawing Path");
                    Gizmos.color = Color.cyan;
                    (int x, int y) = world.FindTile(tile);

                    Vector3 offset = new(x * world.settings.tileWidth, y * world.settings.tileHeight, 0);
                    Vector3 size = new(world.settings.tileWidth, world.settings.tileHeight, 0);
                    Gizmos.DrawCube(offset, size);
                }
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
                Debug.Log("Set start");
                start = tile;
                (int x, int y) = FindTile(map, tile);
                Debug.Log($"Start pos = ({x},{y})");
            }
            else if (end == null)
            {
                Debug.Log("Set end");
                end = tile;
                (int x, int y) = FindTile(map, tile);
                Debug.Log($"End pos = ({x},{y})");

                Debug.Log(start.Walkable() + " and " + end.Walkable());

                currentPath = pathfinder.FindPath(start, end);
                Debug.Log(currentPath);

            }
            else
            {
                Debug.Log("Reset");
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
            Debug.Log(mousePos);
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
    float Average(List<float> numbers)
    {
        // If the list is empty, return 0
        if (numbers.Count == 0)
        {
            return 0f;
        }

        // Declare a variable to store the sum of the numbers
        float sum = 0f;

        // Loop through the list and add each number to the sum
        foreach (float number in numbers)
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


