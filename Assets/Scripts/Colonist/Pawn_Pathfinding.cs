using System;
using System.Collections.Generic;
using System.Diagnostics;
using WorldGeneration;

// A struct for the tile node
public struct TileNode
{
    // A field to store the tile itself
    public Tile tile;

    // A field to store the priority of the tile
    public int priority;

    // A constructor that takes the tile and the priority as parameters and assigns them to the fields
    public TileNode(Tile tile, int priority)
    {
        this.tile = tile;
        this.priority = priority;
    }
}

// A class for the pathfinder
public class Pathfinder
{
    // A private field to store the world as a two-dimensional array of tiles
    private Tile[,] world;

    // A constructor that takes the world as a parameter and assigns it to the field
    public Pathfinder(Tile[,] world)
    {
        this.world = world;
    }

    // A method that finds the shortest path between two tiles using the A* algorithm
    public List<Tile> FindPath(Tile start, Tile end)
    {
        // Check if the start and end tiles are valid and walkable, return null if not
        if (start == null || end == null || !start.Walkable() || !end.Walkable())
        {

            return null;
        }

        // Create a list to store the path
        List<Tile> path = new List<Tile>();

        // Create a priority queue to store the frontier tiles, using the priority as the comparison key
        PriorityQueue<TileNode> frontier = new PriorityQueue<TileNode>(world.Length, (x, y) => x.priority.CompareTo(y.priority));

        // Create a dictionary to store the cost of reaching each tile from the start tile
        Dictionary<Tile, int> costSoFar = new Dictionary<Tile, int>();

        // Create a dictionary to store the previous tile of each tile in the path
        Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();

        // Enqueue the start tile with zero priority
        frontier.Enqueue(new TileNode(start, 0));

        // Set the cost of reaching the start tile to zero
        costSoFar[start] = 0;

        // Set the previous tile of the start tile to null
        cameFrom[start] = null;

        // Loop while the frontier is not empty
        while (frontier.Count > 0)
        {
            // Dequeue the tile with the lowest priority from the frontier
            Tile currentTile = frontier.Dequeue().tile;

            // Check if the current tile is the end tile, break if so
            if (currentTile == end)
            {
                break;
            }

            // Get the neighbors of the current tile that are within the bounds of the world and walkable
            HashSet<Tile> neighbors = GetNeighbors(currentTile);

            // Loop through each neighbor
            foreach (Tile next in neighbors)
            {
                // Calculate the new cost of reaching the neighbor by adding the current cost and the walk speed of the neighbor
                int newCost = costSoFar[currentTile] + next.WalkSpeed();

                // Check if the neighbor is not in the cost dictionary or if its new cost is lower than its previous cost
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    // Update or add the neighbor's cost in the cost dictionary
                    costSoFar[next] = newCost;

                    // Calculate the priority of the neighbor by adding its cost and its heuristic distance to the end tile
                    int priority = newCost + Heuristic(next, end);

                    // Enqueue the neighbor with its priority in the frontier queue
                    frontier.Enqueue(new TileNode(next, priority));

                    // Update or add the previous tile of the neighbor in the came from dictionary
                    cameFrom[next] = currentTile;
                }
            }
        }

        // Check if a path was found, return null if not
        if (!cameFrom.ContainsKey(end))
        {
            return null;
        }

        // Trace back from the end tile to the start tile using
        // Trace back from the end tile to the start tile using the came from dictionary, adding each tile to the path list
        Tile current = end;
        while (current != null)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        // Reverse the path list so that it goes from start to end
        path.Reverse();

        // Return the path list
        return path;
    }

    /*
        // A method that optimizes the path by removing unnecessary turns and checking for better neighbors
        public void OptimizePath(List<Tile> path)
        {
            // Check if the path is null or empty, return if so
            if (path == null || path.Count == 0)
            {
                return;
            }

            // Get the start and end tiles from the path
            Tile start = path[0];
            Tile end = path[path.Count - 1];

            // Check if the start and end tiles are still valid and walkable, return if not
            if (!start.Walkable() || !end.Walkable())
            {
                return;
            }

            // Loop through the path from start to end, excluding the last tile
            for (int i = 0; i < path.Count - 1; i++)
            {
                // Get the current and next tiles from the path
                Tile currentTile = path[i];
                Tile next = path[i + 1];

                // Check if the next tile is still valid and walkable, return if not
                if (!next.Walkable())
                {
                    return;
                }


                // Check if there is a better neighbor for the current tile than the next tile, using the same logic as in the FindPath method
                HashSet<Tile> neighbors = GetNeighbors(currentTile);
                foreach (Tile neighbor in neighbors)
                {
                    int newCost = costSoFar[currentTile] + neighbor.WalkSpeed();
                    int priority = newCost + Heuristic(neighbor, end);
                    if (priority < next.Priority)
                    {
                        // If there is a better neighbor, replace the next tile with it in the path and update its cost and priority
                        path[i + 1] = new TileNode(neighbor, priority);
                        costSoFar[neighbor] = newCost;
                        break;
                    }
                }
            }
        }
        */

    // A helper method that returns the heuristic distance between two tiles using the Manhattan distance formula
    private int Heuristic(Tile a, Tile b)
    {
        // Get the positions of the tiles in the world
        (int ax, int ay) = FindTile(world, a);

        (int bx, int by) = FindTile(world, b);


        // Return the absolute difference of their x and y coordinates
        return Math.Abs(ax - bx) + Math.Abs(ay - by);
    }

    // A helper method that returns the neighbors of a tile that are within the bounds of the world and walkable
    private HashSet<Tile> GetNeighbors(Tile tile)
    {
        // Create a hash set to store the neighbors
        HashSet<Tile> neighbors = new HashSet<Tile>();

        // Get the position of the tile in the world
        (int x, int y) = FindTile(world, tile);


        // Loop through the four directions: up, down, left, right
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                // Skip the diagonal directions and the tile itself using bitwise operations
                if ((dx & dy) != 0)
                {
                    continue;
                }

                // Calculate the neighbor's position
                int nx = x + dx;
                int ny = y + dy;

                // Check if the neighbor's position is within the bounds of the world
                if (nx >= 0 && nx < world.GetLength(0) && ny >= 0 && ny < world.GetLength(1))
                {
                    // Get the neighbor from the world
                    Tile neighbor = world[nx, ny];

                    // Check if the neighbor is walkable, add it to the hash set if so
                    if (neighbor.Walkable())
                    {
                        neighbors.Add(neighbor);
                    }
                }
            }
        }

        // Return the hash set of neighbors
        return neighbors;
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
