using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 1) Choose map size in editor
/// 2) Generate map in editor without play mode
/// 3) Mark tiles as inactive in editor
/// 4) When game starts, 
/// </summary>
public class SP_MapGenerator : MonoBehaviour
{
    public static SP_MapGenerator instance;

    [Header("MAP SETUP")]
    public int rows;
    public int columns;
    public Transform mapParent;

    [Header("TILE SETUP")]
    public GameObject tilePrefab;
    public float tileWidth = 1.15f;
    public float tileHeight = 1f;
    public float columnOffsetX = 0.5f;

    private void Awake()
    {
        instance = this;
    }

    public void GenerateMap()
    {
        DestroyOldMap();

        // go through all rows
        for (int x = 0; x < rows; x++)
        {
            // go through all columns
            for (int y = 0; y < columns; y++)
            {
                // create tile
                GameObject newTileObj = Instantiate(tilePrefab, mapParent);

                // initialize position
                Vector3 tilePos = newTileObj.transform.localPosition;
                tilePos.x += (tileWidth * x);
                tilePos.z += (tileHeight * y);

                // adjust position - even column means shift down, odd column means stay
                if (!SP_Utility.IsEvenNumber(y))
                {
                    tilePos.x -= columnOffsetX;
                }
                
                // set position
                newTileObj.transform.localPosition = tilePos;

                // set tile coordinates 
                SP_Tile newTile = newTileObj.GetComponent<SP_Tile>();
                newTile.x = x;
                newTile.y = y;
                newTile.debugText.text = x.ToString() + "." + y.ToString();
            }
        }
    }

    public void DestroyOldMap()
    {
        for (int i = mapParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(mapParent.GetChild(0).gameObject);
        }
    }
}
