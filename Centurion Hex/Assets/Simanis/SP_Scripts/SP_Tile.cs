using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public enum SP_TileType
{
    Desert,
    Grass,
    Swamp,
    Forest,
    Empty,
    Senate
}

[Serializable]
public class SP_TilePrefab
{
    public SP_TileType tileType;
    public GameObject tileObject;
}

[Serializable]
public class SP_Tile : MonoBehaviour
{
    [Header("COORDINATES")]
    public int x;
    public int y;
    public TextMeshProUGUI debugText;

    [Header("UNIT")]
    public Transform tileUnitParent;
    public SP_Unit myUnit = null;

    [Header("BUILDING")]
    public Transform tileBuildingParent;
    public SP_Building myBuilding = null;

    [Header("ENVIRONMENT")]
    public SP_TileType myEnvironment = SP_TileType.Empty;
    public bool isDiscovered = false;
    public bool isEmptyTile = false;
    public bool isSenateTile = false;
    public GameObject tileCover;
    public SP_TilePrefab[] regularTilePrefabs;
    public GameObject emptyTilePrefab;
    public GameObject senateTilePrefab;


    public void DiscoverTile()
    {
        if (isDiscovered)
            return;

        // hide tile cover
        tileCover.SetActive(false);

        // turn off all prefabs 
        senateTilePrefab.SetActive(false);
        emptyTilePrefab.SetActive(false);
        foreach (SP_TilePrefab prefab in regularTilePrefabs)
        {
            prefab.tileObject.SetActive(false);
        }

        // turn on correct prefabs
        if (isSenateTile)
        {
            senateTilePrefab.SetActive(true);
        }
        if (isEmptyTile)
        {
            emptyTilePrefab.SetActive(true);
        }

        int environmentRoll = UnityEngine.Random.Range(0, regularTilePrefabs.Length);
        regularTilePrefabs[environmentRoll].tileObject.SetActive(true);
    }
        

    public void SelectTile()
    {
        if(myUnit != null)
        {
           // Debug.Log("my unit is not null!");
            if (SP_GameControl.instance.prevSelectedUnit != null)
            {
                SP_GameControl.instance.prevSelectedUnit.SelectUnit(false);
            }
            myUnit.SelectUnit(true);
        }
        else if (myBuilding != null)
        {
            myBuilding.SelectBuilding(true);
        }
        if (myUnit == null && myBuilding == null)
        {
            if (SP_GameControl.instance.prevSelectedUnit != null)
            {
                SP_Unit currUnit = SP_GameControl.instance.prevSelectedUnit;
                int currSpeed = currUnit.myStats.unitCurrSpeed;
                int distanceToThisTile = SP_MapControl.instance.DistanceBetweenTiles(
                    currUnit.x, currUnit.y, x, y);

                if(currUnit.isSleeping)
                {
                    SP_LevelAudioControl.instance.PlaySFX(SP_LevelAudioControl.instance.sleepingSFX);
                }
                else if (!SP_MapControl.instance.IsTileAdjacent(
                    currUnit.x, currUnit.y, x, y))
                {
                    Debug.Log("Tile not adjacent");
                    SP_LevelAudioControl.instance.PlaySFX(SP_LevelAudioControl.instance.outOfRangeSFX);
                }
                else if (distanceToThisTile <= currSpeed)
                {
                    SP_GameControl.instance.prevSelectedUnit.MoveUnit(x, y, selectTargetTileAfter: true,
                        speedCost: distanceToThisTile);
                }
                else
                {
                    Debug.Log("Insufficient Movement");
                    SP_LevelAudioControl.instance.PlayInsuffcientSpeedSFX();
                }
            }
        }
    }
}
