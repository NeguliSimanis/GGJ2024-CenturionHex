using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Board
{
    public Tile [,] Tiles = new Tile[7,7];
    public Tile GetTile(int x, int y)
    {
        return Tiles[ x, y ];
    }

    public Tile [] GetAdjacentTiles(int x, int y)
    {
        Tile[] adjacentTiles = new Tile[] {null, null, null, null, null, null};

        // tiles in higher row
        if (x < 6)
        {
            adjacentTiles[0] = GetTile(x + 1, y);
            Debug.Log("found tile on " + (x + 1) + "." + (y));
            if (y > 0)
            {
                adjacentTiles[1] = GetTile(x + 1, y - 1);
                Debug.Log("found tile on " + (x + 1) + "." + (y - 1));
            }
        }

        // tiles in same row
        if (y < 6)
        {
            adjacentTiles[2] = GetTile(x, y + 1);
            Debug.Log("found tile on " + (x) + "." + (y + 1));
        }
        if (y > 0)
        {
            adjacentTiles[3] = GetTile(x, y - 1);
            Debug.Log("found tile on " + (x) + "." + (y-1));
        }

        // tiles in lower row
        if (x > 0)
        {
            adjacentTiles[4] = GetTile(x - 1, y);
            Debug.Log("found tile on " + (x - 1) + "." + y);
            if (y < 6)
            {
                adjacentTiles[5] = GetTile(x - 1, y + 1);
                Debug.Log("found tile on " + (x - 1) + "." + y+1);
            }
        }

        return adjacentTiles;
    }

    public Board()
    {
        Tile.TileType[,] BoardTileTypes =
        {
            {
                Tile.TileType.ttVoid, Tile.TileType.ttVoid, Tile.TileType.ttSlow, Tile.TileType.ttBuildable, Tile.TileType.ttBuildable, Tile.TileType.ttEmpty, Tile.TileType.ttSenate
            },
            {
                Tile.TileType.ttVoid, Tile.TileType.ttMoney, Tile.TileType.ttSlow, Tile.TileType.ttBuildable, Tile.TileType.ttBuildable, Tile.TileType.ttBuildable, Tile.TileType.ttEmpty
            },
            {
                Tile.TileType.ttSlow, Tile.TileType.ttSlow, Tile.TileType.ttBuildable, Tile.TileType.ttSlow, Tile.TileType.ttSlow, Tile.TileType.ttBuildable, Tile.TileType.ttBuildable
            },
            {
                Tile.TileType.ttBuildable, Tile.TileType.ttBuildable, Tile.TileType.ttSlow, Tile.TileType.ttCenter, Tile.TileType.ttSlow, Tile.TileType.ttBuildable, Tile.TileType.ttBuildable
            },
            {
                Tile.TileType.ttBuildable, Tile.TileType.ttBuildable, Tile.TileType.ttSlow, Tile.TileType.ttSlow, Tile.TileType.ttBuildable, Tile.TileType.ttSlow, Tile.TileType.ttSlow
            },
            {
                Tile.TileType.ttEmpty, Tile.TileType.ttBuildable, Tile.TileType.ttBuildable, Tile.TileType.ttBuildable, Tile.TileType.ttSlow, Tile.TileType.ttMoney, Tile.TileType.ttVoid
            },
            {
                Tile.TileType.ttSenate, Tile.TileType.ttEmpty, Tile.TileType.ttBuildable, Tile.TileType.ttBuildable, Tile.TileType.ttSlow, Tile.TileType.ttVoid, Tile.TileType.ttVoid
            }
        };
        for (int y = 0; y < 7; y++)
        {
            for(int x = 0; x < 7; x++)
            {
                Tiles[x,y] = new Tile();
                Tiles[x, y].tileType = BoardTileTypes[y, x];
            }
        }
    }

    public void LoadFromNetwork(ByteArray data)
    {
        for (int y = 0; y < 7; y++)
        {
            for (int x = 0; x < 7; x++)
            {
                Tiles[x,y].LoadFromNetwork(data);
            }
        }
    }

    public void Reset()
    {
        for (int y = 0; y < 7; y++)
        {
            for (int x = 0; x < 7; x++)
            {
                Tiles[x, y].Reset();
            }
        }
    }

    // 
    // Function to calculate the shortest distance between two given tiles
    public int ShortestDistance(Tile startTile, Tile endTile)
    {
        // Create a priority queue for open tiles
        PriorityQueue<Tile> openSet = new PriorityQueue<Tile>();

        // Create a dictionary to keep track of the distances
        Dictionary<Tile, int> distance = new Dictionary<Tile, int>();

        // Initialize distance of all tiles to infinity
        foreach (Tile tile in Tiles)
        {
            distance[tile] = int.MaxValue;
        }

        // Initialize distance of start tile to 0
        distance[startTile] = 0;

        // Add start tile to the open set
        openSet.Enqueue(startTile, 0);

        while (openSet.Count > 0)
        {
            Tile current = openSet.Dequeue();

            if (current == endTile)
            {
                // If we've reached the end tile, return its distance
                return distance[current];
            }

            // Loop through neighbors of the current tile
            foreach (Tile neighbor in GetNeighbors(current))
            {
                // Calculate the tentative distance from start to neighbor
                int tentativeDistance = distance[current] + 1; // Assuming each step has a distance of 1

                if (tentativeDistance < distance[neighbor])
                {
                    // If this path to the neighbor is shorter, update the distance
                    distance[neighbor] = tentativeDistance;

                    // Add neighbor to open set with priority as the distance + heuristic (if using A* algorithm)
                    openSet.Enqueue(neighbor, tentativeDistance);
                }
            }
        }

        // If end tile is unreachable, return -1
        return -1;
    }

    // Function to get neighboring tiles of a given tile
    List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();

        // Implement logic to get neighboring tiles based on hexagonal grid topology
        // You may need to define rules based on your specific grid layout

        return neighbors;
    }
}

public class PriorityQueue<T>
{
    private SortedDictionary<int, Queue<T>> _dictionary = new SortedDictionary<int, Queue<T>>();

    public void Enqueue(T item, int priority)
    {
        if (!_dictionary.ContainsKey(priority))
        {
            _dictionary[priority] = new Queue<T>();
        }
        _dictionary[priority].Enqueue(item);
    }

    public T Dequeue()
    {
        var firstPair = _dictionary.First();
        var queue = firstPair.Value;
        var result = queue.Dequeue();
        if (queue.Count == 0)
        {
            _dictionary.Remove(firstPair.Key);
        }
        return result;
    }

    public int Count
    {
        get
        {
            int count = 0;
            foreach (var queue in _dictionary.Values)
            {
                count += queue.Count;
            }
            return count;
        }
    }
}
