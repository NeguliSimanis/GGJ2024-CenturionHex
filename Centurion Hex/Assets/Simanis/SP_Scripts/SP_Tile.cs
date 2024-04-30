using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
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
    [Header("SLOW TILE")]
    public bool isSlowTile;
    public GameObject slowTileObject;

    [Header("JEWEL")]
    public bool containsJewel = false;
    public GameObject jewelAnimationLocation;
    public GameObject jewelAura;

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
    public bool allowSpawnStuffOnTile = true;
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


    private void Start()
    {
        if (containsJewel)
        {
            jewelAura.SetActive(true);
        }
        if (isSlowTile)
        {
            slowTileObject.SetActive(true);
        }
        if (UnityEngine.Random.Range(0,1f) < SP_GameControl.instance.slow_tile_chance)
        {
            isSlowTile = true;
            slowTileObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayJewelAnimation();
        }
    }

    public void DiscoverTile()
    {
        if (isDiscovered)
            return;

        isDiscovered = true;

        // hide tile cover
        tileCover.SetActive(false);
        slowTileObject.SetActive(false);

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

        if (containsJewel && myUnit.isAllyUnit)
        {
            return;
        }

        Roll_Chance_For_Something_On_Tile();
    }

    public void JewelCheck()
    {
        if (containsJewel && myUnit.isAllyUnit)
        {
            PlayJewelAnimation();
            DOVirtual.DelayedCall(1f, () =>
            {
                SP_GameControl.instance.ProcessJewelPickup();
            });
        }
    }

    private void PlayJewelAnimation()
    {
        Debug.Log("PLAY GEM ANIM");
        SP_JewelAnimation.instance.ShowJewelAnimation(jewelAnimationLocation, true);
    }

    private void Roll_Chance_For_Something_On_Tile()
    {
        if (!allowSpawnStuffOnTile)
            return;
        if (!SP_GameControl.instance.map_has_random_mines
            && !SP_GameControl.instance.map_has_random_gold)
            return;

        if (UnityEngine.Random.Range(0, 1f) > SP_GameControl.instance.tile_has_something_chance)
            return;

        // roll gold chance
        if (SP_GameControl.instance.map_has_random_gold)
        {

            Debug.Log("TODO: roll chance for gold and landmines");
        }
        // roll landmine chance
        else if (SP_GameControl.instance.map_has_random_mines)
        {
            if (UnityEngine.Random.Range(0, 1) < SP_GameControl.instance.tile_something_is_landmine_chance)
                ExplodeLandmine();

        }
    }

    private void ExplodeLandmine()
    {
        if (myUnit != null)
        {
            myUnit.HurtUnit(1, damagedByLandmine: true);
        }

        // audio
        SP_LevelAudioControl.instance.PlaySFX(SP_LevelAudioControl.instance.landmine_explosion_sfx,
            isVoiceLine: false,
            volumeMultiplier: 0.5f);

        // screen shake
        SP_CameraShake.instance.ShakeCamera();

        // explosion
        GameObject newExplosion = Instantiate(explosionPrefab, explosionLocation.transform);
        newExplosion.transform.localPosition = Vector3.zero;
        newExplosion.transform.localRotation = Quaternion.identity;
        newExplosion.transform.localScale = Vector3.one;
        newExplosion.SetActive(true);
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
            SP_LevelAudioControl.instance.PlaySFX(SP_LevelAudioControl.instance.civilUnitCannotAttackSFX, isVoiceLine: true);
        }
        // not attackers turn
        else if (attacker.isAllyUnit && !SP_GameControl.instance.isAllyTurn)
        {
            SP_LevelAudioControl.instance.PlaySFX(SP_LevelAudioControl.instance.sleepingSFX, isVoiceLine: true);
        }
        // attacker too far
        else if (attacker.myStats.attackRange < SP_MapControl.instance.DistanceBetweenTiles(attacker.x, attacker.y, x, y)) 
        {
            SP_LevelAudioControl.instance.PlaySFX(SP_LevelAudioControl.instance.outOfRangeSFX, isVoiceLine: true);
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
                SP_LevelAudioControl.instance.PlaySFX(SP_LevelAudioControl.instance.cannot_command_enemy_unit_sfx, isVoiceLine: true);
            }
            else if (currUnit.isSleeping)
            {
                SP_LevelAudioControl.instance.PlaySFX(SP_LevelAudioControl.instance.sleepingSFX, isVoiceLine: true);
            }
            else if (!SP_MapControl.instance.IsTileAdjacent(
                currUnit.x, currUnit.y, x, y))
            {
                Debug.Log("Tile not adjacent");
                SP_LevelAudioControl.instance.PlaySFX(SP_LevelAudioControl.instance.outOfRangeSFX, isVoiceLine: true);
            }
            else if (distanceToThisTile <= currSpeed)
            {
                int additionalSpeedCost = 0;
                if (isSlowTile && !isDiscovered)
                    additionalSpeedCost++;
                SP_GameControl.instance.prevSelectedUnit.MoveUnit(x, y, selectTargetTileAfter: true,
                    speedCost: distanceToThisTile + additionalSpeedCost);
            }
            else
            {
                Debug.Log("Insufficient Movement");
                SP_LevelAudioControl.instance.PlayInsuffcientSpeedSFX();
            }
        }
    }
}
