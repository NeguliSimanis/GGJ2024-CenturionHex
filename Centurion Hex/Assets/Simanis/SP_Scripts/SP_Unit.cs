using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public enum SP_UnitType
{
    Surveyor,
    Scout,
    Legionary,
    Sniper,
    Raptor,
    Witch,
    Barista,
    Merchant,
    Madman,
    Diplomat,
    Princess,
    Engineer,
    Undefined
}

[Serializable]
public class SP_UnitStats
{
    public SP_UnitType unitType;
    public int unitMaxSpeed;
    public int unitCurrSpeed;
    public int unitAttack;
    public int unitMaxLife;
    public int unitCurrLife;
    public int unitCost = 2;

    public SP_UnitStats(SP_UnitType myType)
    {
        unitType = myType;

        switch(unitType)
        {
            case SP_UnitType.Scout:
                unitAttack = 1;
                unitMaxSpeed = 3;
                unitMaxLife = 2;
                break;
            case SP_UnitType.Surveyor:
                unitAttack = 0;
                unitMaxSpeed = 3;
                unitMaxLife = 1;
                break;
        }

        unitCurrLife = unitMaxLife;
        unitCurrSpeed = unitMaxSpeed;
    }
}

public class SP_Unit : MonoBehaviour
{
    public bool isMyUnit = true;

    [Header("Coordinates")]
    public int x;
    public int y;
    public SP_Tile parentTile = null;



    [Header("sleep")]
    public GameObject sleepAnimation;
    public bool isSleeping = false;

    [Header("UNIT STATS")]
    public SP_UnitType myType;
    public SP_UnitStats myStats;

    [Header("UNIT STATS HUD")]
    public Transform speedParent;
    public GameObject speedIcon;
    public Transform lifeParent;
    public GameObject lifeIcon;

    [Header("MOVE ANIMATION")]
    public float moveDuration = 1f;
    public Ease moveEase;
    private bool isMoveAnimating = false;

    [Header("ANIMATIONS")]
    public GameObject selectAnimation;

    // idle animation
    public float scaleSpeed = 1.0f; // Adjust the speed as needed
    public float maxScaleY = 1.08f;
    public float minScaleY = 1.0f;
    public Transform scaleAnimationTarget;
    private bool scalingUp = true;

    private void Update()
    {
        float newYScale = scaleAnimationTarget.transform.localScale.y;

        if (scalingUp)
        {
            newYScale += Time.deltaTime * scaleSpeed;
            if (newYScale >= maxScaleY)
            {
                newYScale = maxScaleY;
                scalingUp = false;
            }
        }
        else
        {
            newYScale -= Time.deltaTime * scaleSpeed;
            if (newYScale <= minScaleY)
            {
                newYScale = minScaleY;
                scalingUp = true;
            }
        }

        // Apply the new scale
        scaleAnimationTarget.transform.localScale =
            new Vector3(scaleAnimationTarget.transform.localScale.x,
            newYScale,
            scaleAnimationTarget.transform.localScale.z);
    }

    public void InitializeUnit(SP_UnitType unitType = SP_UnitType.Undefined)
    {
        if (unitType == SP_UnitType.Undefined)
            myStats = new SP_UnitStats(myType);
        else
            myStats = new SP_UnitStats(unitType);

        SetLifeUI();
        SetSpeedUI();
        Debug.Log("unit initialzied");
    }

    public void SelectUnit(bool select)
    {
       // Debug.Log("SELECTED UNIT!");
        selectAnimation.SetActive(select);
        SP_GameControl.instance.prevSelectedUnit = this;
    }

    public void SetLifeUI()
    {
        int remainingLife = myStats.unitCurrLife;

        int lifeDisplayed = 0;
        foreach (Transform life in lifeParent)
        {
            if (life.gameObject.activeInHierarchy)
                lifeDisplayed++;
        }

        for (int i = 0; i < lifeDisplayed; i++)
        {
            if (lifeDisplayed > remainingLife)
            {
                GameObject lifeObject = lifeParent.GetChild(i).gameObject;
                if (lifeObject.activeInHierarchy)
                {
                    lifeDisplayed--;
                }
                lifeObject.SetActive(false);
            }

        }
        while (lifeDisplayed < remainingLife
            && lifeDisplayed < 50)
        {
            GameObject newLifeIcon = Instantiate(lifeIcon, lifeParent);
            lifeDisplayed++;
        }
    }

    public void SetSpeedUI(bool isNewRoundPhase = false)
    {
        int remainingSpeed = myStats.unitCurrSpeed;

        int speedDisplayed = 0;
        foreach (Transform speed in speedParent)
        {
            if (speed.gameObject.activeInHierarchy)
                speedDisplayed++;
        }

        //Debug.Log("speedDisplayed" + speedDisplayed + ". remaining speed " + remainingSpeed);
        for (int i = 0; i < speedParent.childCount; i++)
        {
            if (speedDisplayed > remainingSpeed)
            {
                GameObject speedObject = speedParent.GetChild(i).gameObject;
                if (speedObject.activeInHierarchy)
                {
                    speedDisplayed--;
                }
                speedObject.SetActive(false);
            }

        }
        while (speedDisplayed < remainingSpeed
            && speedDisplayed < 50)
        {
            GameObject newLifeIcon = Instantiate(speedIcon, speedParent);
            //Debug.Log(character.type + " adding speed");
            speedDisplayed++;
        }
    }

    public void MoveUnit(int targetX, int targetY, bool instant = false,
        bool selectTargetTileAfter = false, int speedCost = 0)
    {
        Debug.Log("moving unit to " + targetX + "." + targetY);
        if (!SP_MapControl.instance.IsCoordinateValid(targetX, targetY))
            return;

        // Get tile
        SP_Tile targetTile = SP_MapControl.instance.tileMap[targetX, targetY];

        if (!SP_MapControl.instance.IsTileEmpty(targetTile))
            return;
        
        if(isMoveAnimating)
        {
            Debug.Log("old move not finisehd");
            return;
        }

        // --- CHECK PASSED - ALLOWED TO MOVE 

        isMoveAnimating = true;

        // Remove old tile reference
        if (parentTile != null)
        {
            parentTile.myUnit = null;
            parentTile = null;
        }

        transform.parent = targetTile.tileUnitParent;
        
        // set scale and rotation
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;

        // move
        if (instant)
        {
            transform.localPosition = Vector3.zero;
            ProcessMoveComplete(targetX, targetY, targetTile, speedCost, selectTargetTileAfter);
        }
        else
        {
            transform.DOLocalMove(Vector3.zero, moveDuration).SetEase(ease: moveEase).
                OnComplete(()=>
                {
                    ProcessMoveComplete(targetX, targetY, targetTile, speedCost, selectTargetTileAfter);
                });
        }
    }

    private void ProcessMoveComplete(int newX, int newY, SP_Tile targetTile,
        int movementSpent, bool selectParentTile = false)
    {
        // update speed
        myStats.unitCurrSpeed -= movementSpent;
        if (myStats.unitCurrSpeed <= 0)
            myStats.unitCurrSpeed = 0;
        SetSpeedUI();

        // set new coordinates
        x = newX;
        y = newY;

        // set new parent
        parentTile = targetTile;

        // update target tile info
        targetTile.myUnit = this;
        targetTile.DiscoverTile();

        isMoveAnimating = false;

        if (selectParentTile)
        {
            parentTile.SelectTile();
        }
    }

    public void MarkUnitAsSleeping(bool isSleeping)
    {
        sleepAnimation.SetActive(isSleeping);
    }

    public void ProcessNewTurnStart()
    {
        // SPEED
        myStats.unitCurrSpeed = myStats.unitMaxSpeed;
        SetSpeedUI();

        // SLEEPINESS
        if (isMyUnit)
        {
            if (SP_GameControl.instance.isAllyTurn)
            {
                isSleeping = false;
            }
            else
            {
                isSleeping = true;
            }
        }
        else // enemy turn
        {
            if (SP_GameControl.instance.isAllyTurn)
            {
                isSleeping = true;
            }
            else
            {
                isSleeping = false;
            }
        }
        MarkUnitAsSleeping(isSleeping);
    }

}
