using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum Direction
{
    North,
    Northeast,
    Northwest,
    South,
    Southeast,
    Southwest,
    East,
    West
}

public class Board
{
    public class TileCoordinates
    {
        public int x;
        public int y;

        /// <summary>
        /// G - current movement cost from starting tile to current considered tile
        /// </summary>
        public int G;
        /// <summary>
        ///  H - estimated movement cost from current cosidered tile to target tile
        /// </summary>
        public int H;
        /// <summary>
        ///  F = G + H
        /// </summary>
        public int F;

        /// <summary>
        /// Closed list - contains all tiles that have already been considered.
        /// </summary>
        public bool isInClosedList = false;

        public TileCoordinates(int newX, int newY)
        {
            x = newX;
            y = newY;
        }

        public bool IsMyCoordinates(int checkX, int checkY)
        {
            if (checkX == x && checkY == y)
                return true;
            else
                return false;
        }
    }

    public Tile [,] Tiles = new Tile[7,7];
    public Tile GetTile(int x, int y)
    {
        return Tiles[ x, y ];
    }

    /// <summary>
    /// 
    /// Will include A* pathfinding based on this
    /// https://blog.theknightsofunity.com/pathfinding-on-a-hexagonal-grid-a-algorithm/
    /// 
    /// ---- TILE PROPERTIES ---- 
    /// G - current movement cost from starting tile to current considered tile
    /// 
    /// H - estimated movement cost from current cosidered tile to target tile
    /// on rectangular grid H = |x1 – x2| + |y1 – y2| 
    /// 
    /// F = G + H
    /// 
    /// --- TILE LISTS ---
    /// - Open list - will contain all tiles that can be considered as path points.
    /// - Closed list - will contain all tiles that have already been considered.
    /// 
    /// ---- ALGORITHM ---- 
    /// 1) Take a tile from the open list.
    /// 2) Add this tile to the closed list.
    /// 3) For every walkable adjacent tile of this tile:
    ///     - If an adjacent tile is in the closed list – ignore it.
    ///     - If an adjacent tile is not in the open list – add it to the open list.
    ///     - If an adjacent tile is already in the open list – check if its F parameter 
    ///       will be lower when we use the current path, if it is – update the adjacent tile parameters.
    /// 
    /// </summary>
    public int DistanceBetweenTiles(int x_tileA, int y_tileA,
        int x_tileB, int y_tileB)
    {
        int distance = 0;
        bool targetTileFound = false;


        /// will contain all tiles that can be considered as path points.
        List<TileCoordinates> openList = new List<TileCoordinates>();

        /// will contain all tiles that have already been considered.
        List<TileCoordinates> closedList = new List<TileCoordinates>();

        // STEP 1 - check the origin tile
        if (x_tileA == x_tileB && y_tileA == y_tileB)
            // destination is same as origin
            return 0;

        TileCoordinates originTile = new TileCoordinates(x_tileA, x_tileB);
        openList.Add(originTile);
        closedList.Add(originTile);
        originTile.isInClosedList = true;


        /// 2) Add this tile to the closed list.
        /// 3) For every walkable adjacent tile of this tile:
        ///     - If an adjacent tile is in the closed list – ignore it.
        ///     - If an adjacent tile is not in the open list – add it to the open list.
        ///     - If an adjacent tile is already in the open list – check if its F parameter 
        ///       will be lower when we use the current path, if it is – update the adjacent tile parameters.
        /// 

        // STEP 2 - GET ALL NEIGHBORS
        Tile[] adjacentTiles = GetAdjacentTiles(originTile.x, originTile.y);

        foreach (Tile adjacentTile in adjacentTiles)
        {
            // neighbor in closed list - ignore
            foreach(TileCoordinates tileCoordinate in closedList)
            {
                
            }    

            // neighbor not in open list - add to open list
            // neighbor in open list - check F. If it is better than current path, update adjacent tile parameteres
        }




        /*
         *  b X is bigger - tile is located to south/ southeast
         *  b X is smaller - tile is located to north/ northwest
         *  
         *  b Y is bigger - tile is located north/ northeast
         *  b Y is smaller - tile is located south/ southwest
         */

        // 

        #region old stuff
        //List<TileCoordinates> checkedCoordinates = new List<TileCoordinates>();

        //// is it the same tile?
        //if (x_tileA == x_tileB && y_tileA == y_tileB)
        //    return 0;
        //else
        //    checkedCoordinates.Add(new TileCoordinates(x_tileA, x_tileB));

        //// not same tile, at least 1 distance
        //distance = 1;

        //// while b x bigger - check SOUTH
        //int xSouthCheck = x_tileA;
        //while (xSouthCheck <= x_tileB)
        //{
        //    // SOUTH
        //    TileCoordinates temp1 = new TileCoordinates(xSouthCheck + 1, y_tileA);
        //    checkedCoordinates.Add(temp1);
        //    temp1.distance = Mathf.Abs(xSouthCheck - x_tileA + 1);
        //    // FOUND TILE
        //    if (temp1.IsMyCoordinates(x_tileB, y_tileB))
        //    {
        //        targetTileFound = true;
        //        distance = temp1.distance;
        //        break;
        //    }
        //    xSouthCheck++;
        //}
        //// while b x bigger - check SOUTHEAST
        //int ySouthEastCheck = y_tileA;
        //while (!targetTileFound &&
        //    x_tileA < x_tileB &&
        //    ySouthEastCheck <= y_tileB)
        //{

        //    ySouthEastCheck++;
        //}
        #endregion

        return distance;
    }

    /// <summary>
    /// Hasn't been tested if works correctly
    /// </summary>
    /// <param name="x_tileA"></param>
    /// <param name="y_tileA"></param>
    /// <param name="x_tileB"></param>
    /// <param name="y_tileB"></param>
    /// <returns></returns>
    public bool AreTilesAdjacent(int x_tileA, int y_tileA,
        int x_tileB, int y_tileB)
    {
        bool isAdjacent = true;

        // tile is not adjacent - row too far
        if (Mathf.Abs(x_tileA - x_tileB) > 1)
        {
            Debug.Log(x_tileA + "." + y_tileA + " is not adjacent to " + x_tileB + "." + y_tileB);
            return false;
        }

        // tile is not adjacent - column too far
        if (Mathf.Abs(y_tileA - y_tileB) > 1)
        {
            Debug.Log(x_tileA + "." + y_tileA + " is not adjacent to " + x_tileB + "." + y_tileB);
            return false;
        }

        return isAdjacent;
    }

    public List<TileCoordinates> GetAdjacentTileCoordinates(int x, int y)
    {
        List<TileCoordinates> adjacentTiles = new List<TileCoordinates>();

        //// tiles in higher row
        if (x < 6)
        {
            TileCoordinates adjacent1 = new TileCoordinates(x + 1, y);
            adjacentTiles.Add(adjacent1);
            //if (y > 0)
            //{
            //    adjacentTiles[1] = GetTile(x + 1, y - 1);
            //    Debug.Log("found tile on " + (x + 1) + "." + (y - 1));
            //}
        }

        //// tiles in same row
        //if (y < 6)
        //{
        //    adjacentTiles[2] = GetTile(x, y + 1);
        //    Debug.Log("found tile on " + (x) + "." + (y + 1));
        //}
        //if (y > 0)
        //{
        //    adjacentTiles[3] = GetTile(x, y - 1);
        //    Debug.Log("found tile on " + (x) + "." + (y - 1));
        //}

        //// tiles in lower row
        //if (x > 0)
        //{
        //    adjacentTiles[4] = GetTile(x - 1, y);
        //    Debug.Log("found tile on " + (x - 1) + "." + y);
        //    if (y < 6)
        //    {
        //        adjacentTiles[5] = GetTile(x - 1, y + 1);
        //        Debug.Log("found tile on " + (x - 1) + "." + y + 1);
        //    }
        //}
        return adjacentTiles;
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

    
}
