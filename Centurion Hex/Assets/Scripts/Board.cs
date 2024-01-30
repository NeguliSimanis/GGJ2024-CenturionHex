using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
