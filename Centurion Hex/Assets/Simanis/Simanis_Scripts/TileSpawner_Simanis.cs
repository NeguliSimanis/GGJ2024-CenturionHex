using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner_Simanis : MonoBehaviour
{

    public static TileSpawner_Simanis instance;
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
    public List<BuildingVisual_Simanis> allBuildings = new List<BuildingVisual_Simanis>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //SpawnTiles();
    }

    private void RotateBoard()
    {
        if (centurionGame.PlayingAsRed)
            return;
        tileParent.parent = tileCenter.transform;
        Quaternion currentRotation = tileCenter.transform.rotation;

        // Set the rotation on the y-axis to 180 degrees
        currentRotation.eulerAngles = new Vector3(0, 180, 0);

        // Apply the new rotation to the object
        tileCenter.transform.rotation = currentRotation;
    }

    public void UncoverTile()
    {
        foreach(TileVisual_Simanis tileVisual in allTiles)
        {
            if (tileVisual.tile == centurionGame.lastTileCovered)
                tileVisual.DiscoverTile();
        }
    }

    public void ColorGreyInactiveUnits()
    {
        foreach (CharacterVisual_Simanis charVisual in allCharacters)
        {
            // GENERAL MOVE
            if (CenturionGame.Instance.GeneralMove && charVisual.IsMyUnit()
                && HUD_Simanis.instance.IsMyTurn())
            {
                // NORMAL COLOR WAR UNITS
                if (charVisual.IsWarUnit())
                    charVisual.ColorUnitGrey(false);

                // COLOR GREY CIVIL UNITS
                else
                    charVisual.ColorUnitGrey(true);
            }
            // GOVERNOR MOVE
            else if (!CenturionGame.Instance.GeneralMove && charVisual.IsMyUnit()
                && HUD_Simanis.instance.IsMyTurn())
            {
                //  COLOR GREY WAR UNITS
                if (charVisual.IsWarUnit())
                    charVisual.ColorUnitGrey(true);

                // NORMAL COLOR CIVIL UNITS
                else
                    charVisual.ColorUnitGrey(false);
            }
            else
            {
                charVisual.ColorUnitGrey(false);
            }
        }
    }

    private void CleanOldBoard()
    {
        foreach(BuildingVisual_Simanis building in allBuildings)
        {
            Destroy(building.gameObject);
        }
        allBuildings.Clear();
        foreach (TileVisual_Simanis tile in allTiles)
        {
            Destroy(tile.gameObject);
        }
        allTiles.Clear();
        foreach (CharacterVisual_Simanis character in allCharacters)
        {
            Destroy(character.gameObject);
        }
        allCharacters.Clear();
        
    }

    public void SpawnTiles()
    {   
        centurionBoard = centurionGame.Board;

        CleanOldBoard();

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

        // SPAWN TILE VISUALS - Loop through the 2D array
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

        // COLOR UNITS GREY/NORMAL DEPENDING ON ROUND PHASE
        ColorGreyInactiveUnits();

        // align the rows
        int oldAddedX = 0;// -1 removed | 0 -undefined | +1 added
        for (int i = 0; i < rows; i++)
        {
            Vector3 oldPos = rowParents[i].transform.position;
            float newPosX = oldPos.x + rowOffsetX * i;
            rowParents[i].transform.position = new Vector3(newPosX, oldPos.y,
                oldPos.z + rowOffsetZ*i);
        }
        RotateBoard();
    }

    public void SpawnCharacterOnTile(Tile tile, Transform parent,
        TileVisual_Simanis tileVisual, int x, int y)
    {
        if (tile.currentCharacter == null)
            return;
        
        GameObject newChar = Instantiate(characterPrefab, parent);
        newChar.transform.localPosition = Vector3.zero;
        newChar.transform.localRotation = Quaternion.identity;
        newChar.transform.localScale = Vector3.one;
        tileVisual.DiscoverTile();
        CharacterVisual_Simanis characterVisual = newChar.GetComponent<CharacterVisual_Simanis>();
        characterVisual.character = tile.currentCharacter;
        characterVisual.SetCharacterVisuals(tile.currentCharacter.type, this);
        characterVisual.SetSpeedUI();
        characterVisual.xCoord = x;
        characterVisual.yCoord = y;
        characterVisual.SetLifeUI();
        allCharacters.Add(characterVisual);

        Debug.Log("spawnin " + tile.currentCharacter.type);
    }

    public void PlayVictoryPointAnimation()
    {
        foreach(TileVisual_Simanis tileVisual in allTiles)
        {
           if (tileVisual.tile.tileType == Tile.TileType.ttCenter)
            {
                tileVisual.SpawnVictoryPointGain();
            }
        }
    }

    public void PlayGoldTileAnimation()
    {
        foreach (TileVisual_Simanis tileVisual in allTiles)
        {
            if (tileVisual.tile == centurionGame.lastTileCovered)
            {
                tileVisual.SpawnGoldGain();
            }
        }
    }

    /// <summary>
    /// Called by CenturionGame.cs event set in editor
    /// </summary>
    public void PlaceCharacter()
    {
        Character characterToPlace = centurionGame.lastPlacedCharacter;

        bool isMyChar = false;

        if (characterToPlace.Team.Type == Team.TeamType.ttBlue
            && !CenturionGame.Instance.PlayingAsRed)
            isMyChar = true;
        if (characterToPlace.Team.Type == Team.TeamType.ttRed
            && CenturionGame.Instance.PlayingAsRed)
            isMyChar = true;


        if (isMyChar)
        {
           // RaycastInteract target = HUD_Simanis.instance.cardPlacementTarget;
            Destroy(HUD_Simanis.instance.cardPrefabBeingPlayed);
            HUD_Simanis.instance.ClearHighlights();
        }
            Tile charTile = CenturionGame.Instance.Board.GetTile(characterToPlace.x, characterToPlace.y);
            TileVisual_Simanis charTileVisual = GetTileVisual(charTile);
            SpawnCharacterOnTile(
               tile: charTile,
               parent: charTileVisual.unitTransformPos,
               tileVisual: charTileVisual,
               x: characterToPlace.x,
               y: characterToPlace.y
               );
    }


    /// <summary>
    /// Called by CenturionGame.cs event set in editor
    /// </summary>
    public void PlaceBuilding()
    {
        Debug.Log("trying to place building");
        Building buildingToPlace = centurionGame.lastPlacedBuilding;

        bool isMyBuilding = false;

        if (buildingToPlace.Team.Type == Team.TeamType.ttBlue
            && !CenturionGame.Instance.PlayingAsRed)
            isMyBuilding = true;
        if (buildingToPlace.Team.Type == Team.TeamType.ttRed
            && CenturionGame.Instance.PlayingAsRed)
            isMyBuilding = true;

        if (isMyBuilding)
        {
            Destroy(HUD_Simanis.instance.cardPrefabBeingPlayed);
            HUD_Simanis.instance.ClearHighlights();
        }
            Tile buildTile = CenturionGame.Instance.Board.GetTile(buildingToPlace.x, buildingToPlace.y);
            TileVisual_Simanis buildTileVisual = GetTileVisual(buildTile);
            SpawnBuildingOnTile(
                tile: buildTile,
                parent: buildTileVisual.buildingTransformPos,
                tileVisual: buildTileVisual,
                x: buildingToPlace.x,
                y: buildingToPlace.y
                );
    }

    public void SpawnBuildingOnTile(Tile tile, Transform parent,
        TileVisual_Simanis tileVisual, int x, int y)
    {
        if (tile.currentBuilding == null)
            return;
        tileVisual.DiscoverTile();
        GameObject newBuild = Instantiate(buildingPrefab, parent);

        BuildingVisual_Simanis buildingVisual = newBuild.GetComponent<BuildingVisual_Simanis>();
        buildingVisual.building = tile.currentBuilding;
        buildingVisual.xCoord = x;
        buildingVisual.yCoord = y;
        buildingVisual.SetBuildingVisuals(tile.currentBuilding.Type, this);
        allBuildings.Add(buildingVisual);
    }

    public void SpawnTileVisual(Vector3 spawnPos, Tile tile, int row, int collumn, Transform parent)
    {   
        GameObject newTileObject = Instantiate(tilePrefab, parent);
        newTileObject.transform.position = spawnPos;

        TileVisual_Simanis tileVisual = newTileObject.GetComponent<TileVisual_Simanis>();
        tileVisual.tile = tile;
        if (!centurionGame.PlayingAsRed)
        {
            tileVisual.FlipTile();
        }
        tileVisual.SetTileCoords(xC: row, yC: collumn, debug: false);
        tileVisual.SetTileVisuals(this);
        allTiles.Add(tileVisual);

        SpawnCharacterOnTile(tile, tileVisual.unitTransformPos, tileVisual,
            x: row, y: collumn);
        SpawnBuildingOnTile(tile, newTileObject.transform, tileVisual,
            x:row, y: collumn);
        //tileVisual.ShowMessage(row + "." + collumn + "." + tile.tileType);
    }

    public TileVisual_Simanis GetTileVisual(Tile tile)
    {
        foreach (TileVisual_Simanis tileVisual in allTiles)
        {
            if (tileVisual.tile == tile)
                return tileVisual;
        }
        return null;
    }
}
