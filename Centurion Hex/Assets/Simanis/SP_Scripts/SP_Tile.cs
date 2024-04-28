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

    [Header("Info icon")]
    public GameObject infoIcon;

    [Header("EXPLOSIOn")]
    public GameObject explosionPrefab;
    public Transform explosionLocation;

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

        isDiscovered = true;

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
        myEnvironment = regularTilePrefabs[environmentRoll].tileType;

        Roll_Chance_For_Something_On_Tile();
    }

    private void Roll_Chance_For_Something_On_Tile()
    {
        if (!SP_GameControl.instance.map_has_random_mines)
            return;
    }

    private void ProcessUnitSelected()
    {
        if (SP_GameControl.instance.prevSelectedUnit != null)
        {
            SP_Unit prevUnit = SP_GameControl.instance.prevSelectedUnit;

            // prev selected enemy unit, chance selection to different unit
            if (!prevUnit.isAllyUnit)
            {
                prevUnit.SelectUnit(false);
                myUnit.SelectUnit(true);
            }
            
            // prev selected ally unit, attempt to attack target if enemy
            if (prevUnit.isAllyUnit)
            {
                if (!myUnit.isAllyUnit)
                {
                    AttemptAttackMyUnit(prevUnit);
                } 
                else
                {
                    prevUnit.SelectUnit(false);
                    myUnit.SelectUnit(true);
                }
            }
        }
        else
        {
            myUnit.SelectUnit(true);
        }
       
    }

    private void AttemptAttackMyUnit(SP_Unit attacker)
    {
        Debug.Log("attempting to attack");
        // attacker has 0 attack
        if (attacker.myStats.isCivilUnit && attacker.myStats.unitDamage <= 0)
        {
            SP_LevelAudioControl.instance.PlaySFX(SP_LevelAudioControl.instance.civilUnitCannotAttackSFX);
        }
        // not attackers turn
        else if (attacker.isAllyUnit && !SP_GameControl.instance.isAllyTurn)
        {
            SP_LevelAudioControl.instance.PlaySFX(SP_LevelAudioControl.instance.sleepingSFX);
        }
        // attacker too far
        else if (attacker.myStats.attackRange < SP_MapControl.instance.DistanceBetweenTiles(attacker.x, attacker.y, x, y)) 
        {
            SP_LevelAudioControl.instance.PlaySFX(SP_LevelAudioControl.instance.outOfRangeSFX);
        }
        else if (attacker.myStats.unitDamage <= 0)
        {
            Debug.Log("Cannot attack this is bullshit");
        }
        else if (attacker.myStats.hasAttackedThisTurn)

        {
            Debug.Log("Has attacked cannot attack anymore");
        }
        // attack success
        else
        {
            SP_GameControl.instance.customCursor.SetCursor(false, cursorAction: CursorAction.attack);
            attacker.myStats.hasAttackedThisTurn = true;

            attacker.PlayAttackAnimation(this);
        }
    }

    public void SelectTile()
    {
        if(myUnit != null)
        {
            ProcessUnitSelected();
        }
        else if (myBuilding != null)
        {
            myBuilding.SelectBuilding(true);
        }

        if (myUnit == null && myBuilding == null)
        {
            AttemptToMoveUnitHere();
        }
    }

    private void AttemptToMoveUnitHere()
    {
        if (SP_GameControl.instance.prevSelectedUnit != null)
        {
            SP_Unit currUnit = SP_GameControl.instance.prevSelectedUnit;
            int currSpeed = currUnit.myStats.unitCurrSpeed;
            int distanceToThisTile = SP_MapControl.instance.DistanceBetweenTiles(
                currUnit.x, currUnit.y, x, y);

            if (!currUnit.isAllyUnit)
            {
                SP_LevelAudioControl.instance.PlaySFX(SP_LevelAudioControl.instance.cannot_command_enemy_unit_sfx);
            }
            else if (currUnit.isSleeping)
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
