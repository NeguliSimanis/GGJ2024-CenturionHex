using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner_Simanis : MonoBehaviour
{

    public CenturionGame centurionGame;
    private Board centurionBoard;

    [Header("TILES")]
    public GameObject tileCenter;
    public GameObject tilePrefab;
    public Transform tileParent;
    public Transform empty;

    public float tileWidth = 1.15f;
    public float tileHeight = 1f;

    public float rowOffsetX = 0.5f;
    public float rowOffsetZ = -0.27f;
    public List<TileVisual_Simanis> allTiles = new List<TileVisual_Simanis>();

    [Header("CHARACTERS")]
    public GameObject characterPrefab;
    public List<CharacterVisual_Simanis> allCharacters = new List<CharacterVisual_Simanis>();

    [Header("Buildings")]
    public GameObject buildingPrefab;

    private void Start()
    {
       
        //SpawnTiles();
    }

    private void RotateBoard()
    {
        if (centurionGame.PlayingAsRed)
            return;
        Quaternion currentRotation = tileCenter.transform.rotation;

        // Set the rotation on the y-axis to 180 degrees
        currentRotation.eulerAngles = new Vector3(0, 180, 0);

        // Apply the new rotation to the object
        tileCenter.transform.rotation = currentRotation;
    }

    public void SpawnTiles()
    {
        RotateBoard();
        centurionBoard = centurionGame.Board;

        // Get the dimensions of the array
        int rows = centurionBoard.Tiles.GetLength(0);
        int cols = centurionBoard.Tiles.GetLength(1);


        List<GameObject> rowParents = new List<GameObject>();
        for (int i = 0; i < rows; i++)
        {
            GameObject newParent = Instantiate(empty.gameObject, tileParent);
            newParent.name = "Row " + i;
            rowParents.Add(newParent);
        }

        // Loop through the 2D array
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                // Access each element of the array
                Tile currTile = centurionBoard.Tiles[i, j];
                if (currTile.tileType != Tile.TileType.ttVoid)
                    SpawnTileVisual(new Vector3(i * tileHeight, 0, j* tileWidth), 
                        currTile, row: i, collumn: j,
                        parent: rowParents[j].transform);
                // Do something with the element (e.g., print it)
               // Debug.Log("Element at position (" + i + ", " + j + "): " + currTile);
            }
        }

        // align the rows
        int oldAddedX = 0;// -1 removed | 0 -undefined | +1 added
        for (int i = 0; i < rows; i++)
        {
            Vector3 oldPos = rowParents[i].transform.position;
            float newPosX = oldPos.x + rowOffsetX * i;

            //if (i > 3)
            //{
            //    bool addRow = false;
            //    if (oldAddedX == 0 || oldAddedX == +1)
            //    {
            //        newPosX = oldPos.x + rowOffsetX * (3) - rowOffsetX * (i - 3);
            //        oldAddedX = -1;
            //    }
            //    else
            //    {
            //        oldAddedX = 1;
            //        newPosX = oldPos.x + rowOffsetX * (3) + rowOffsetX * (i - 3);
            //    }
            //}
            rowParents[i].transform.position = new Vector3(newPosX, oldPos.y,
                oldPos.z + rowOffsetZ*i);
        }
    }

    public void SpawnCharacterOnTile(Tile tile, Vector3 spawnPos, Transform parent)
    {
        if (tile.currentCharacter == null)
            return;
        GameObject newChar = Instantiate(characterPrefab, parent);

        CharacterVisual_Simanis characterVisual = newChar.GetComponent<CharacterVisual_Simanis>();
        characterVisual.character = tile.currentCharacter;
        characterVisual.SetCharacterVisuals(tile.currentCharacter.type, this);
        allCharacters.Add(characterVisual);
        Debug.Log("spawnin " + tile.currentCharacter.type);
    }

    public void SpawnBuildingOnTile(Tile tile, Transform parent)
    {
        if (tile.currentBuilding == null)
            return;
        GameObject newBuild = Instantiate(buildingPrefab, parent);

        BuildingVisual_Simanis buildingVisual = newBuild.GetComponent<BuildingVisual_Simanis>();
        buildingVisual.building = tile.currentBuilding;
        buildingVisual.SetBuildingVisuals(tile.currentBuilding.Type);
    }

    public void SpawnTileVisual(Vector3 spawnPos, Tile tile, int row, int collumn, Transform parent)
    {   
        GameObject newTileObject = Instantiate(tilePrefab, parent);
        newTileObject.transform.position = spawnPos;

        TileVisual_Simanis tileVisual = newTileObject.GetComponent<TileVisual_Simanis>();
        tileVisual.tile = tile;
        tileVisual.xCoord = row;
        tileVisual.yCoord = collumn;
        tileVisual.SetTileVisuals(tile.tileType);
        allTiles.Add(tileVisual);

        SpawnCharacterOnTile(tile, spawnPos, newTileObject.transform);
        SpawnBuildingOnTile(tile, newTileObject.transform);
        //tileVisual.ShowMessage(row + "." + collumn + "." + tile.tileType);
    }
}
