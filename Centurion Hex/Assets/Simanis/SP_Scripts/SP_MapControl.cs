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
}
