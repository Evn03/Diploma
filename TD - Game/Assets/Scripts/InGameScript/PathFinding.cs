using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private TileData[,] grid;
    private int width;
    private int height;

    public Pathfinding(TileData[,] grid, int width, int height)
    {
        this.grid = grid;
        this.width = width;
        this.height = height;
    }

    public List<TileData> FindPath(TileData start, TileData goal)
    {
        if (start == null || goal == null)
        {
            Debug.LogError("Pathfinding.FindPath: start или goal — null!");
            return new List<TileData>();
        }
        List<TileData> openSet = new List<TileData> { start };
        HashSet<TileData> closedSet = new HashSet<TileData>();

        Dictionary<TileData, TileData> cameFrom = new Dictionary<TileData, TileData>();
        Dictionary<TileData, float> gScore = new Dictionary<TileData, float>();
        Dictionary<TileData, float> fScore = new Dictionary<TileData, float>();

        foreach (TileData tile in grid)
        {
            gScore[tile] = float.MaxValue;
            fScore[tile] = float.MaxValue;
        }

        gScore[start] = 0;
        fScore[start] = Heuristic(start, goal);

        while (openSet.Count > 0)
        {
            TileData current = GetLowestFScore(openSet, fScore);
            if (current == goal)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (TileData neighbor in GetNeighbors(current))
            {
                if (!neighbor.isWalkable || closedSet.Contains(neighbor))
                    continue;

                float tentativeG = gScore[current] + Vector2.Distance(current.worldPosition, neighbor.worldPosition);

                if (tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    fScore[neighbor] = tentativeG + Heuristic(neighbor, goal);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return new List<TileData>();
    }

    private float Heuristic(TileData a, TileData b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private TileData GetLowestFScore(List<TileData> set, Dictionary<TileData, float> fScore)
    {
        TileData best = set[0];
        float bestScore = fScore[best];

        foreach (var tile in set)
        {
            if (fScore[tile] < bestScore)
            {
                best = tile;
                bestScore = fScore[tile];
            }
        }
        return best;
    }

    private List<TileData> GetNeighbors(TileData tile)
    {
        List<TileData> neighbors = new List<TileData>();
        int x = tile.x;
        int y = tile.y;

        AddIfValid(x + 1, y, neighbors);
        AddIfValid(x - 1, y, neighbors);
        AddIfValid(x, y + 1, neighbors);
        AddIfValid(x, y - 1, neighbors);

        return neighbors;
    }

    private void AddIfValid(int x, int y, List<TileData> list)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            TileData t = grid[x, y];
            if (t != null)
                list.Add(t);
        }
    }

    private List<TileData> ReconstructPath(Dictionary<TileData, TileData> cameFrom, TileData current)
    {
        List<TileData> path = new List<TileData>();
        while (cameFrom.ContainsKey(current))
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Add(current);
        path.Reverse();
        return path;
    }
}

