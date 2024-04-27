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
    public float tileHeight = 1.15f;
    public float realTileWidth = 1.7f;
    public float columnOffsetX = 0.5f;

    public float xOffset = 0.25f;
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
                float zXcomponent = x * realTileWidth * 0.5f;
                float zYcomponent = y * realTileWidth * 0.5f;
                tilePos.z += zXcomponent + zYcomponent; ///(tileWidth * x);

                // pa labi +z
                // pa kreisi -z
                /// uz augšu -x
                /// uz leju +x
                float xXcomponent = (tileHeight - xOffset) * x;

                float xYcomponent = (tileHeight - xOffset) * -y;
                if (!SP_Utility.IsEvenNumber(y))
                    xYcomponent += (tileHeight - xOffset);
                tilePos.x += xXcomponent + xYcomponent;
                //tilePos.z += (0.5 * tileHeight *y) + ;

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
