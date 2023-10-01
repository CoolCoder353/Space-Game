using System;
using System.Collections.Generic;
using WorldGeneration;
using UnityEngine;

// A struct for the tile node
public struct TileNode
{
    // A field to store the tile itself
    public Tile tile;

    // A field to store the priority of the tile
    public float priority;

    // A constructor that takes the tile and the priority as parameters and assigns them to the fields
    public TileNode(Tile tile, float priority)
    {
        this.tile = tile;
        this.priority = priority;
    }
}

// A class for the pathfinder
public static class Pathfinder
{

    // A method that finds the shortest path between two tiles using the A* algorithm
    public static List<Tile> FindPath(Tile start, Tile end, Tile[,] world)
    {
        // Check if the start and end tiles are valid and walkable, return null if not
        if (start == null || end == null || !start.walkable || !end.walkable)
        {
            Debug.LogWarning("Start or end tile is null or not walkable");

            //Try to find the closest tile to the end tile that is walkable.
            //If there is no walkable tile, return null.

            //Get the neighbours of the end tile.
            HashSet<Tile> neighbours = GetNeighbors(end, world);
            //Loop through the neighbours.
            foreach (Tile neighbour in neighbours)
            {
                //Check if the neighbour is walkable.
                if (neighbour.walkable)
                {
                    //If it is, return a path from the start tile to the neighbour.
                    return FindPath(start, neighbour, world);
                }
            }


            return null;
        }

        if (World.Contains(start, world) == false || World.Contains(end, world) == false)
        {
            Debug.LogWarning("Start or end tile is not in the world");
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
        frontier.Enqueue(new TileNode(start, 0f));

        // Set the cost of reaching the start tile to zero
        costSoFar[start] = 0;

        // Set the previous tile of the start tile to null
        cameFrom[start] = null;

        DateTime startTime = System.DateTime.Now;
        // Loop while the frontier is not empty
        while (frontier.Count > 0 && (System.DateTime.Now - startTime).TotalMinutes < 1)
        {
            // Dequeue the tile with the lowest priority from the frontier
            Tile currentTile = frontier.Dequeue().tile;

            // Check if the current tile is the end tile, break if so
            if (currentTile == end)
            {
                break;
            }

            // Get the neighbors of the current tile that are within the bounds of the world and walkable
            HashSet<Tile> neighbors = GetNeighbors(currentTile, world);
            int x = currentTile.position.x;
            int y = currentTile.position.y;
            ////Debug.Log($"Tile ({x},{y}) has {neighbors.Count} neighbours.");

            // Loop through each neighbor
            foreach (Tile next in neighbors)
            {
                // Calculate the new cost of reaching the neighbor by adding the current cost and the walk speed of the neighbor
                int newCost = costSoFar[currentTile] + next.walkSpeed;

                // Check if the neighbor is not in the cost dictionary or if its new cost is lower than its previous cost
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    // Update or add the neighbor's cost in the cost dictionary
                    costSoFar[next] = newCost;

                    // Calculate the priority of the neighbor by adding its cost and its heuristic distance to the end tile
                    float priority = newCost + Heuristic(next, end);


                    // Enqueue the neighbor with its priority in the frontier queue
                    frontier.Enqueue(new TileNode(next, priority));

                    // Update or add the previous tile of the neighbor in the came from dictionary
                    cameFrom[next] = currentTile;
                }
            }
        }

        if ((System.DateTime.Now - startTime).TotalMinutes >= 1)
        {
            Debug.LogWarning("Pathfinding timed out");
            return null;
        }
        // Check if a path was found, return null if not
        if (!cameFrom.ContainsKey(end))
        {
            Debug.LogWarning("No path found");
            return null;
        }

        // Trace back from the end tile to the start tile using the came from dictionary, adding each tile to the path list
        Tile current = end;
        while (current != null)
        {
            path.Add(current);

            // Check if the current tile is in the came from dictionary, break if not
            if (!cameFrom.ContainsKey(current))
            {
                break;
            }

            // Get the previous tile of the current tile from the came from dictionary
            current = cameFrom[current];
        }

        // Reverse the path list so that it goes from start to end
        path.Reverse();


        // Return the path list
        return path;
    }


    // A helper method that returns the heuristic distance between two tiles using the Manhattan distance formula
    private static float Heuristic(Tile a, Tile b)
    {
        // Get the positions of the tiles in the world


        Vector2 point1 = new(a.position.x, a.position.y);
        Vector2 point2 = new(b.position.x, b.position.y);

        return -EuclideanDistance(point1, point2);
    }

    public static float EuclideanDistance(Vector2 point1, Vector2 point2)
    {
        return Vector2.Distance(point1, point2);
    }
    public static float ChebyshevDistance(Vector2 point1, Vector2 point2)
    {
        float distance = 0;
        distance = Math.Max(Math.Abs(point1.x - point2.x), Math.Abs(point1.y - point2.y));
        return distance;
    }


    // A helper method that returns the neighbors of a tile that are within the bounds of the world and walkable
    private static HashSet<Tile> GetNeighbors(Tile tile, Tile[,] world)
    {
        // Create a hash set to store the neighbors
        HashSet<Tile> neighbors = new HashSet<Tile>();

        // Get the position of the tile directly from its property
        int x = tile.position.x;
        int y = tile.position.y;

        // Loop through the eight directions: up, down, left, right, and diagonals
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                ////Debug.Log($"Tile ({x},{y}) has a neigbour.");
                // Skip the tile itself using bitwise operations
                if (dx == 0 && dy == 0)
                {
                    continue;
                }
                ////Debug.Log($"Tile ({x},{y}) has a neigbour that is not itself.");
                // Calculate the neighbor's position
                int nx = x + dx;
                int ny = y + dy;

                // Check if the neighbor's position is within the bounds of the world
                if (nx >= 0 && nx < world.GetLength(0) && ny >= 0 && ny < world.GetLength(1))
                {
                    ////Debug.Log($"Tile ({x},{y}) has a neigbour within the bounds of the world.");
                    // Get the neighbor from the world
                    Tile neighbor = world[nx, ny];

                    ////Debug.Log($"Found neighbour for tile ({x},{y}) at ({nx},{ny})");
                    // Check if the neighbor is walkable, add it to the hash set if so
                    if (neighbor.walkable)
                    {
                        ////Debug.Log("Neighbour is walkable");
                        // Check if the neighbor is diagonal
                        if ((dx & dy) != 0)
                        {
                            ////Debug.Log("Neighbour is a diagonal");
                            // Get the two adjacent tiles that form a corner with the neighbor
                            Tile corner1 = world[x + dx, y];
                            Tile corner2 = world[x, y + dy];

                            // Check if both corner tiles are blocked, skip the neighbor if so
                            if (!corner1.walkable && !corner2.walkable)
                            {
                                ////Debug.Log("Neighbour not acceptable for some reason");
                                continue;
                            }
                        }

                        // Add the neighbor to the hash set
                        neighbors.Add(neighbor);
                    }
                }
            }
        }

        // Return the hash set of neighbors
        return neighbors;
    }
}
