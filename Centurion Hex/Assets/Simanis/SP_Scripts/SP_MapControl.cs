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
        if (targetX > SP_MapGenerator.instance.rows || targetX < 0)
        {
            Debug.Log("xcoord not valid");
            return false;
        }
        if (targetY > SP_MapGenerator.instance.columns || targetX < 0)
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
