using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Tile [,] Tiles;
    public Tile GetTile(int x, int y)
    {
        return Tiles[ x, y ];
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
}
