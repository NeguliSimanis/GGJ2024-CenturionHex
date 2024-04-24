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
        //public bool isInClosedList = false;
        /// <summary>
        /// Open list - contains all tiles that can be considered as path points.
        /// </summary>
        public bool isInOpenList = false;
        // is this tiles distance checked
        public bool distanceChecked = false;

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

    
    public int DistanceBetweenTiles(int x_tileA, int y_tileA,
        int x_tileB, int y_tileB)
    {
        int distance = 0;
        
        /*
         * distance 
         * - x constant = yDiff
         * - y constant = xDiff
         * 
         * - both x & y change in same directioj = 
         * 
         * = both increase/decrease (identical increase * 2) + max(remainingXdiff, remainingYDiff) <- THIS WORKS???
         */

        int xDiff = x_tileA - x_tileB;
        int yDiff = y_tileA - y_tileB;

        int xAbs = Mathf.Abs(xDiff);
        int yAbs = Mathf.Abs(yDiff);

        int horizontalDistance = 0; // 1 horizontal distance = 2 tiles 
        int verticalDistance = 0;   // 1 vertical distance   = 1 tile

        // check for horizontal distance
        if (xDiff < 0 && yDiff < 0 ||
            xDiff > 0 && yDiff > 0)
        {
            //Debug.Log("there's horizontal movement component");
            horizontalDistance = Mathf.Min(xAbs, yAbs);
        }

        // calculate vertical distance
        verticalDistance = Mathf.Max(xAbs, yAbs) - horizontalDistance;

        // total distance
        distance = verticalDistance + horizontalDistance * 2;
        Debug.Log("Vertical dist " + verticalDistance + ". Horizontal distance " + horizontalDistance + ". Total distance: " + distance + " tiles");


            #region old stuff
            /*
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
            bool targetTileFound = false;
            /// will contain all tiles that can be considered as path points.
            List<TileCoordinates> openList = new List<TileCoordinates>();

            /// will contain all tiles that have already been considered.
            List<TileCoordinates> closedList = new List<TileCoordinates>();

            // path to destination
            List<TileCoordinates> path = new List<TileCoordinates>();

            // STEP 1 - check the origin tile
            if (x_tileA == x_tileB && y_tileA == y_tileB)
                // destination is same as origin
                return 0;

            TileCoordinates originTile = new TileCoordinates(x_tileA, x_tileB);
            openList.Add(originTile);
            closedList.Add(originTile);
            path.Add(originTile);


            /// 2) Add this tile to the closed list.
            /// 3) For every walkable adjacent tile of this tile:
            ///     - If an adjacent tile is in the closed list – ignore it.
            ///     - If an adjacent tile is not in the open list – add it to the open list.
            ///     - If an adjacent tile is already in the open list – check if its F parameter 
            ///       will be lower when we use the current path, if it is – update the adjacent tile parameters.
            /// 

            // STEP 2 - GET ALL NEIGHBORS
            List<TileCoordinates> adjacentCoordinates = GetAdjacentTileCoordinates(originTile.x, originTile.y);

            foreach (TileCoordinates adjacentCoordinate in adjacentCoordinates)
            {

                bool isInClosedList = false;
                bool isInOpenList = false;

                // neighbor in closed list - ignore
                foreach (TileCoordinates closedCoordinate in closedList)
                {
                    if (closedCoordinate.x == adjacentCoordinate.x 
                        && closedCoordinate.y == adjacentCoordinate.y)
                    {
                        isInClosedList = true;
                        break;
                    }
                }
                if (isInClosedList)
                    break;

                // check if neighbor is in open list
                foreach (TileCoordinates openCoordinate in openList)
                {
                    if (openCoordinate.x == adjacentCoordinate.x
                        && openCoordinate.x == adjacentCoordinate.y)
                    {
                        isInOpenList = true;
                        break;
                    }
                }
                // neighbor not in open list - add to open list
                if (!isInOpenList)
                {
                    openList.Add(adjacentCoordinate);
                    break;
                }

                // neighbor in open list - check F. If it is better than current path, update adjacent tile parameteres
            }

            /*
             *  b X is bigger - tile is located to south/ southeast
             *  b X is smaller - tile is located to north/ northwest
             *  
             *  b Y is bigger - tile is located north/ northeast
             *  b Y is smaller - tile is located south/ southwest
             */
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
            if (y > 0)
            {
                TileCoordinates adjacent2 = new TileCoordinates(x + 1, y - 1);
                adjacentTiles.Add(adjacent2);
            }
        }

        // tiles in same row
        if (y < 6)
        {
            TileCoordinates adjacent3 = new TileCoordinates(x, y + 1);
            adjacentTiles.Add(adjacent3);
        }
        if (y > 0)
        {
            TileCoordinates adjacent4 = new TileCoordinates(x, y - 1);
            adjacentTiles.Add(adjacent4);
        }

        // tiles in lower row
        if (x > 0)
        {
            TileCoordinates adjacent5 = new TileCoordinates(x - 1, y);
            adjacentTiles.Add(adjacent5);
            if (y < 6)
            {
                TileCoordinates adjacent6 = new TileCoordinates(x - 1, y + 1);
                adjacentTiles.Add(adjacent6);
            }
        }
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
