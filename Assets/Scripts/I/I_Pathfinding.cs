using UnityEngine;
using System.Collections.Generic;
using WorldGeneration;
using System.Linq;
namespace PathFinding
{


    public class Node
    {
        public Vector2Int Position { get; set; }
        public int GCost { get; set; }
        public int HCost { get; set; }
        public Node ParentNode { get; set; }

        public int weight { get; set; }

        public Tile tileObject { get; set; }

        public Node(Tile tile)
        {
            SetNodeData(tile);
        }

        /// <summary>
        /// Set the data of the current node.
        /// </summary>
        public void SetNodeData(Tile tile)
        {
            Position = tile.gridPos;
            tileObject = tile;
            weight = (int)tile.GetData("movement_weight");
        }

        /// <summary>
        /// Calculate the heuristic cost (HCost) using Manhattan distance.
        /// </summary>
        public int CalculateHCost_Manhattan(Vector2Int endPosition)
        {
            return Mathf.Abs(Position.x - endPosition.x) + Mathf.Abs(Position.y - endPosition.y);
        }

        /// <summary>
        /// Calculate the heuristic cost (HCost) using Euclidean distance.
        /// </summary>
        public int CalculateHCost_Euclidean(Vector2Int endPosition)
        {
            return Mathf.RoundToInt(Mathf.Sqrt(Mathf.Pow(Position.x - endPosition.x, 2) + Mathf.Pow(Position.y - endPosition.y, 2)));
        }

        /// <summary>
        /// Calculate the heuristic cost (HCost) using Chebyshev distance.
        /// </summary>
        public int CalculateHCost_Chebyshev(Vector2Int endPosition)
        {
            return Mathf.Max(Mathf.Abs(Position.x - endPosition.x), Mathf.Abs(Position.y - endPosition.y));
        }

        /// <summary>
        /// Calculate the heuristic cost (HCost) using Octile distance.
        /// Octile distance is a more accurate heuristic for grid-based pathfinding where diagonal movement is allowed.
        /// It's a combination of Manhattan and Chebyshev distances.
        /// </summary>
        public float CalculateHCost_Octile(Vector2Int endPosition, float CostForMoving = 1, float CostForMovingDiagonaly = 1.41421356237f)
        {
            float dx = Mathf.Abs(Position.x - endPosition.x);
            float dy = Mathf.Abs(Position.y - endPosition.y);
            return CostForMoving * (dx + dy) + (CostForMovingDiagonaly - 2 * CostForMoving) * Mathf.Min(dx, dy);
        }


        /// <summary>
        /// Calculate the heuristic cost (HCost) using Squared Euclidean distance.
        /// This is a variant of Euclidean distance that avoids the expensive square root operation.
        /// It's not an admissible heuristic for A* (it can overestimate the cost), but it can be faster and still gives reasonable results in many cases.
        /// </summary>
        public int CalculateHCost_SquaredEuclidean(Vector2Int endPosition)
        {
            return Mathf.RoundToInt(Mathf.Pow(Position.x - endPosition.x, 2) + Mathf.Pow(Position.y - endPosition.y, 2));
        }

        /// <summary>
        /// Calculate the movement cost (GCost) from the start node to the current node.
        /// </summary>
        public int CalculateGCost(Node startNode)
        {
            return Mathf.Abs(Position.x - startNode.Position.x) + Mathf.Abs(Position.y - startNode.Position.y) * weight;
        }

        /// <summary>
        /// Get the neighbors of the current node from the tile map.
        /// </summary>
        public List<Node> GetNeighbors(Tile[,] tileMap, bool allowDiagonalMovement)
        {
            List<Node> neighbors = new List<Node>();
            int maxX = tileMap.GetLength(0) - 1;
            int maxY = tileMap.GetLength(1) - 1;

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue; // Skip the current node itself
                    if (!allowDiagonalMovement && Mathf.Abs(x) == Mathf.Abs(y)) continue; // Skip diagonals if not allowed

                    int neighborX = Position.x + x;
                    int neighborY = Position.y + y;

                    if (neighborX >= 0 && neighborX <= maxX && neighborY >= 0 && neighborY <= maxY)
                    {
                        neighbors.Add(new Node(tileMap[neighborX, neighborY]));
                    }
                }
            }

            return neighbors;
        }

        /// <summary>
        /// Check if the current node is walkable.
        /// </summary>
        public bool IsWalkable()
        {
            return (bool)tileObject.GetData("isWalkable");
        }
    }

    public static class Path
    {
        /// <summary>
        /// Find the shortest path from the start node to the end node using the A* algorithm.
        /// </summary>
        public static List<Node> FindPath(Tile[,] tiles, Tile startTile, Tile endTile, bool allowDiagonalMovement)
        {

            Node startNode = new Node(startTile);
            Node endNode = new Node(endTile);

            HashSet<Node> openList = new HashSet<Node>();
            HashSet<Node> closedList = new HashSet<Node>();

            openList.Add(startNode);

            while (openList.Count > 0)
            {
                Node currentNode = openList.Min();

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                if (currentNode == endNode)
                {
                    // Path found, trace back from end node to start node
                    List<Node> path = new List<Node>();
                    Node node = endNode;

                    while (node != startNode)
                    {
                        path.Add(node);
                        node = node.ParentNode;
                    }

                    path.Reverse();

                    // If the path is empty, throw a warning and return
                    if (path.Count == 0)
                    {
                        Debug.LogWarning($"Path from {startTile.worldPos} to {endTile.worldPos} is empty");
                        return null;
                    }
                    return path;
                }

                List<Node> neighbors = currentNode.GetNeighbors(tiles, allowDiagonalMovement);

                foreach (Node neighbor in neighbors)
                {
                    if (!neighbor.IsWalkable() || closedList.Contains(neighbor))
                        continue;

                    int newGCost = currentNode.GCost + neighbor.CalculateGCost(startNode);

                    if (!openList.Contains(neighbor) || newGCost < neighbor.GCost)
                    {
                        neighbor.GCost = newGCost;
                        neighbor.HCost = neighbor.CalculateHCost_Manhattan(endNode.Position); // Change this line to use a different heuristic
                        neighbor.ParentNode = currentNode;

                        if (!openList.Contains(neighbor))
                            openList.Add(neighbor);
                    }
                }
            }

            // No path found
            return null;
        }
    }
}
