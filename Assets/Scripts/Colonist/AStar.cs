using System;
using System.Collections.Generic;
using WorldGeneration;
using UnityEngine;
public static class AStar
{
    private static int ManhattanDistance(Tile tile1, Tile tile2)
    {
        return System.Math.Abs(tile1.Position.Item1 - tile2.Position.Item1) + System.Math.Abs(tile1.Position.Item2 - tile2.Position.Item2);
    }

    public static List<Tile> FindPath(Tile startTile, Tile endTile, Tile[,] world)
    {
        var openSet = new SortedSet<(int fScore, Tile tile)>(Comparer<(int fScore, Tile tile)>.Create((a, b) => a.fScore.CompareTo(b.fScore)));
        var closedSet = new HashSet<Tile>();
        var gScore = new Dictionary<Tile, int> { { startTile, 0 } };
        var fScore = new Dictionary<Tile, int> { { startTile, ManhattanDistance(startTile, endTile) } };

        openSet.Add((fScore[startTile], startTile));

        while (openSet.Count > 0)
        {
            var currentTile = openSet.Min.tile;

            if (currentTile == endTile)
            {
                var path = new List<Tile>();
                while (gScore.ContainsKey(currentTile))
                {
                    path.Add(currentTile);
                    if (gScore[currentTile] == 0)
                    {
                        currentTile = null;
                    }
                }
                path.Reverse();
                return path;
            }

            openSet.Remove((fScore[currentTile], currentTile));
            closedSet.Add(currentTile);

            foreach (var (xOffset, yOffset) in new[] { (0, 1), (0, -1), (1, 0), (-1, 0) })
            {
                var neighborPos = (currentTile.Position.Item1 + xOffset, currentTile.Position.Item2 + yOffset);

                if (!(neighborPos.Item1 >= 0 && neighborPos.Item1 < world.GetLength(0) && neighborPos.Item2 >= 0 && neighborPos.Item2 < world.GetLength(1)))
                {
                    continue;
                }

                var neighborTile = world[neighborPos.Item1, neighborPos.Item2];

                if (!neighborTile.Walkable() || closedSet.Contains(neighborTile))
                {
                    continue;
                }

                var tentativeGScore = gScore[currentTile] + ManhattanDistance(currentTile, neighborTile) + neighborTile.WalkSpeed();

                if (!gScore.ContainsKey(neighborTile) || tentativeGScore < gScore[neighborTile])
                {
                    gScore[neighborTile] = tentativeGScore;
                    fScore[neighborTile] = tentativeGScore + ManhattanDistance(neighborTile, endTile);
                    openSet.Add((fScore[neighborTile], neighborTile));
                }
            }
        }

        return new List<Tile>();
    }
}