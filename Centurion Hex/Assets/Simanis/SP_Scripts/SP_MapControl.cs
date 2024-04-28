using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_MapControl : MonoBehaviour
{
    public static SP_MapControl instance;
    public SP_Tile[,] tileMap;

    private void Awake()
    {
        instance = this;
    }

    public void InitializeMap()
    {
        tileMap = new SP_Tile[SP_MapGenerator.instance.rows, SP_MapGenerator.instance.columns];
        foreach (Transform tileObj in SP_MapGenerator.instance.mapParent)
        {
            SP_Tile tile = tileObj.gameObject.GetComponent<SP_Tile>();
            tileMap[tile.x, tile.y] = tile;
            //Debug.Log("added tile " + tile.x + "." + tile.y);
        }
    }

    /// <summary>
    /// Does such coordinate exist on map
    /// </summary>
    /// <param name="targetX"></param>
    /// <param name="targetY"></param>
    /// <returns></returns>
    public bool IsCoordinateValid(int targetX, int targetY)
    {
        bool isValid = true;
        if (targetX >= SP_MapGenerator.instance.rows || targetX < 0)
        {
            Debug.Log("xcoord not valid");
            return false;
        }
        if (targetY >= SP_MapGenerator.instance.columns || targetY < 0)
        {
            Debug.Log("Y coord not valid");
            return false;
        }
        return isValid;
    }

    public bool IsTileEmpty(SP_Tile tileToCheck)
    {
        bool isValid = true;
        if (tileToCheck.myUnit != null)
            return false;
        if (tileToCheck.myBuilding != null)
            return false;
        return isValid;
    }

    public SP_Tile RandomAdjacentAllyTile(int xOrigin, int yOrigin)
    {

        SP_Tile randomTile = null;

        List<SP_Tile> validTiles = new List<SP_Tile>();

        // north of a
        if (IsCoordinateValid(xOrigin - 1, yOrigin + 1))
        {
            validTiles.Add(tileMap[xOrigin - 1, yOrigin + 1]);
        }

        // northeast of a
        if (IsCoordinateValid(xOrigin, yOrigin + 1))
        {
            validTiles.Add(tileMap[xOrigin, yOrigin + 1]);
        }

        // southeast of a
        if (IsCoordinateValid(xOrigin + 1, yOrigin))
        {
            validTiles.Add(tileMap[xOrigin + 1, yOrigin]);
        }

        // sout of a
        if (IsCoordinateValid(xOrigin + 1, yOrigin - 1))
        {
            validTiles.Add(tileMap[xOrigin + 1, yOrigin - 1]);
        }

        // southwest of a
        if (IsCoordinateValid(xOrigin, yOrigin - 1))
        {
            validTiles.Add(tileMap[xOrigin, yOrigin - 1]);
        }

        // northwest of a
        if (IsCoordinateValid(xOrigin - 1, yOrigin))
        {
            validTiles.Add(tileMap[xOrigin - 1, yOrigin]);
        }

        List<SP_Tile> validAllyTiles = new List<SP_Tile>();
        foreach (SP_Tile tile in validTiles)
        {
            if (tile.myUnit != null && tile.myUnit.isAllyUnit)
            {
                validAllyTiles.Add(tile);
            }
            if (tile.myBuilding != null && tile.myBuilding.isAllyBuilding)
            {
                validAllyTiles.Add(tile);
            }
        }

        if (validAllyTiles.Count >= 1)
        {
            int randomRoll = Random.Range(0, validAllyTiles.Count);
            randomTile = validAllyTiles[randomRoll];
        }

        return randomTile;
    }

    public bool HasEmptyAdjacentTiles(int xOrigin, int yOrigin)
    {
        SP_Tile randomTile = tileMap[0, 0];

        List<SP_Tile> validTiles = new List<SP_Tile>();
        bool HasEmptyAdjacentTiles = false;
        // north of a
        if (IsCoordinateValid(xOrigin - 1, yOrigin + 1))
        {
            validTiles.Add(tileMap[xOrigin - 1, yOrigin + 1]);
        }

        // northeast of a
        if (IsCoordinateValid(xOrigin, yOrigin + 1))
        {
            validTiles.Add(tileMap[xOrigin, yOrigin + 1]);
        }

        // southeast of a
        if (IsCoordinateValid(xOrigin + 1, yOrigin))
        {
            validTiles.Add(tileMap[xOrigin + 1, yOrigin]);
        }

        // sout of a
        if (IsCoordinateValid(xOrigin + 1, yOrigin - 1))
        {
            validTiles.Add(tileMap[xOrigin + 1, yOrigin - 1]);
        }

        // southwest of a
        if (IsCoordinateValid(xOrigin, yOrigin - 1))
        {
            validTiles.Add(tileMap[xOrigin, yOrigin - 1]);
        }

        // northwest of a
        if (IsCoordinateValid(xOrigin - 1, yOrigin))
        {
            validTiles.Add(tileMap[xOrigin - 1, yOrigin]);
        }

        List<SP_Tile> validEmptyTiles = new List<SP_Tile>();
        foreach (SP_Tile tile in validTiles)
        {
            if (tile.myBuilding == null && tile.myUnit == null)
            {
                return true;
            }
        }
        
        return HasEmptyAdjacentTiles;
    }

    public SP_Tile GetRandomAdjacentTile(int xOrigin, int yOrigin, bool mustBeEmptyTile = true)
    {
        SP_Tile randomTile = tileMap[0,0];

        List<SP_Tile> validTiles = new List<SP_Tile>();

        // north of a
        if (IsCoordinateValid(xOrigin - 1, yOrigin + 1))
        {
            validTiles.Add(tileMap[xOrigin - 1, yOrigin + 1]);
        }

        // northeast of a
        if (IsCoordinateValid(xOrigin, yOrigin + 1))
        {
            validTiles.Add(tileMap[xOrigin, yOrigin + 1]);
        }

        // southeast of a
        if (IsCoordinateValid(xOrigin + 1, yOrigin))
        {
            validTiles.Add(tileMap[xOrigin + 1, yOrigin]);
        }

        // sout of a
        if (IsCoordinateValid(xOrigin + 1, yOrigin - 1))
        {
            validTiles.Add(tileMap[xOrigin + 1, yOrigin - 1]);
        }

        // southwest of a
        if (IsCoordinateValid(xOrigin, yOrigin - 1))
        {
            validTiles.Add(tileMap[xOrigin, yOrigin - 1]);
        }

        // northwest of a
        if (IsCoordinateValid(xOrigin -1, yOrigin))
        {
            validTiles.Add(tileMap[xOrigin - 1, yOrigin]);
        }

        if (mustBeEmptyTile)
        {
            List<SP_Tile> validEmptyTiles = new List<SP_Tile>();
            foreach (SP_Tile tile in validTiles)
            {
                if (tile.myBuilding == null && tile.myUnit == null)
                {
                    validEmptyTiles.Add(tile);
                }
            }
            int randomRoll = Random.Range(0, validEmptyTiles.Count);
            randomTile = validEmptyTiles[randomRoll];
        }
        else
        {
            int randomRoll = Random.Range(0, validTiles.Count);
            randomTile = validTiles[randomRoll];
        }

        return randomTile;
    }

    public bool IsTileAdjacent(int x_tileA, int y_tileA, int x_tileB, int y_tileB)
    {
        bool isAdjacent = false;

        // north of a
        if (x_tileA - 1 == x_tileB
            && y_tileA + 1 == y_tileB)
            return true;

        // northeast of a
        if (x_tileA == x_tileB &&
            y_tileA + 1 == y_tileB)
            return true;

        // southeast of a
        if (x_tileA + 1 == x_tileB &&
            y_tileA == y_tileB)
            return true;

        // sout of a
        if (x_tileA + 1 == x_tileB &&
            y_tileA - 1 == y_tileB)
            return true;

        // southwest of a
        if (x_tileA == x_tileB &&
            y_tileA - 1 == y_tileB)
            return true;

        // northwest of a
        if (x_tileA - 1 == x_tileB &&
            y_tileA == y_tileB)
            return true;

        return isAdjacent;
    }

    public int DistanceBetweenTiles(int x_tileA, int y_tileA, int x_tileB, int y_tileB)
    {
        int distance = 0;

        /*
         * distance 
         * - y constant = xDiff
         * - x constant = yDiff 
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

        return distance;
    }
}
