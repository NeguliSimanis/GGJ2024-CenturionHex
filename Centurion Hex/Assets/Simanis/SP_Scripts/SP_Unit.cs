using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
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

[System.Serializable]
public class SP_UnitStats
{
    public SP_UnitType unitType;
    public bool isCivilUnit;

    // movement
    public int unitMaxSpeed;
    public int unitCurrSpeed;

    // combat
    public int unitDamage;
    public int attackRange = 1;
    public bool hasAttackedThisTurn = false;

    // life
    public int unitMaxLife;
    public int unitCurrLife;

    public int unitCost = 2;

    public SP_UnitStats(SP_UnitType myType)
    {
        unitType = myType;

        switch(unitType)
        {
            case SP_UnitType.Scout:
                unitDamage = 1;
                unitMaxSpeed = 3;
                unitMaxLife = 2;
                isCivilUnit = false;
                break;
            case SP_UnitType.Surveyor:
                unitDamage = 0;
                unitMaxSpeed = 3;
                unitMaxLife = 1;
                isCivilUnit = true;
                break;
            case SP_UnitType.Legionary:
                unitDamage = 1;
                unitMaxSpeed = 1;
                unitMaxLife = 2;
                isCivilUnit = false;
                break;
            case SP_UnitType.Sniper:
                unitDamage = 1;
                unitMaxSpeed = 1;
                unitMaxLife = 1;
                attackRange = 2;
                isCivilUnit = false;
                break;
            case SP_UnitType.Raptor:
                unitDamage = 1;
                unitMaxSpeed = 3;
                unitMaxLife = 1;
                attackRange = 1;
                isCivilUnit = false;
                break;
        }

        unitCurrLife = unitMaxLife;
        unitCurrSpeed = unitMaxSpeed;
    }
}

[System.Serializable]
public class SP_UnitVisualPrefab
{
    public SP_UnitType type;
    public GameObject gameObject;
}

public class SP_Unit : MonoBehaviour
{
    public bool isAllyUnit = true;

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
    /// <summary>
    /// true when unit is selected and mouse hovering over valid target
    /// </summary>
    public bool readyToAttack = false;

    [Header("UNIT STATS HUD")]
    public Transform speedParent;
    public GameObject speedIcon;
    public Transform lifeParent;
    public GameObject lifeIcon;

    [Header("UNIT VISUALS")]
    public GameObject activePrefab;
    public SP_UnitVisualPrefab[] unitVisuals;

    [Header("MOVE ANIMATION")]
    public float moveDuration = 1f;
    public Ease moveEase;
    private bool isMoveAnimating = false;
    public float moveSFX_delay = 0.1f;

    [Header("ATTACK ANIMATION")]
    public float attackMoveToSpeed = 2f;
    public Ease attackToEase = Ease.InOutQuad;
    public float attackReturnSpeed = 1f;
    public Ease attackReturnEase = Ease.Linear;
    public float attackMoveDistance = 0.9f;

    [Header("OTHER ANIMATIONS")]
    public GameObject selectAnimation;

    // idle animation
    public float scaleSpeed = 1.0f; 
    public float maxScaleY = 1.08f;
    public float minScaleY = 1.0f;
    public Transform scaleAnimationTarget;
    private bool scalingUp = true;

    [Header("AI")]
    public SP_EnemyUnitAI aiControl;

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

        foreach (SP_UnitVisualPrefab prefab in unitVisuals)
        {
            if (prefab.type == myStats.unitType)
            {
                activePrefab = prefab.gameObject;
                prefab.gameObject.SetActive(true);
            }
            else
                prefab.gameObject.SetActive(false);
        }

        if (isAllyUnit)
            FlipCharacter();
        ColorUnit();
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
        bool selectTargetTileAfter = false, int speedCost = 0, bool isAI = false)
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
            DOVirtual.DelayedCall(moveSFX_delay, () =>
            {
                SP_LevelAudioControl.instance.PlayMoveSFX();
            });
            transform.DOLocalMove(Vector3.zero, moveDuration).SetEase(ease: moveEase).
                OnComplete(()=>
                {
                    ProcessMoveComplete(targetX, targetY, targetTile, speedCost, selectTargetTileAfter,isAI);
                });
        }
    }

    private void ProcessMoveComplete(int newX, int newY, SP_Tile targetTile,
        int movementSpent, bool selectParentTile = false, bool isAI = false)
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

        if (isAI)
        {
            DOVirtual.DelayedCall(0.1f, () =>
             {
                 aiControl.ProcessActionComplete();
             });
            
        }
    }

    public void MarkUnitAsSleeping(bool isSleeping)
    {
        sleepAnimation.SetActive(isSleeping);
    }

    public void ProcessNewTurnStart()
    {
        // AI
        if(!isAllyUnit)
        aiControl.hasCompletedTurn = false;

        // SPEED
        myStats.unitCurrSpeed = myStats.unitMaxSpeed;

        // SPEED - RAPTOR ABILITY
        if (myStats.unitType == SP_UnitType.Raptor)
        {
            Debug.Log("im raptor");
            if (parentTile.myEnvironment == SP_TileType.Desert)
            {
                myStats.unitCurrSpeed++;
                Debug.Log("IM ON DESERT");
            }
        }
        SetSpeedUI();

        // attack ability
        myStats.hasAttackedThisTurn = false;

        // SLEEPINESS
        if (isAllyUnit)
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

    public void FlipCharacter()
    {

        Vector3 currentScale = activePrefab.transform.localScale;

        // Flip the x scale by negating its value
        currentScale.x *= -1;

        // Apply the new scale to the object
        activePrefab.transform.localScale = currentScale;
    }

    public void ColorUnit()
    {
        CharacterColorChanger_Simanis colorChanger;
        colorChanger = activePrefab.GetComponent<CharacterColorChanger_Simanis>();

        
        int teamID = 0;

        if (!isAllyUnit)
            teamID = 1;
        //Debug.Log("im playing as red " + CenturionGame.Instance.PlayingAsRed + "");
        colorChanger.ChangeColor(teamID);
    }

    public void HurtUnit(int damageAmount, SP_UnitType unitDamageSource = SP_UnitType.Undefined,
        bool damagedByLandmine = false)
    {
        myStats.unitCurrLife -= damageAmount;
        SetLifeUI();
        if (myStats.unitCurrLife <= 0)
            Die();
    }

    public void PlayAttackAnimation(SP_Tile targetTile)
    {
        Vector3 enemyPos = targetTile.myUnit.activePrefab.transform.position;
        Vector3 myPos = activePrefab.transform.position;
        Vector3 targetPos = new Vector3(
            myPos.x + Mathf.Clamp(enemyPos.x - myPos.x, -1, 1) * attackMoveDistance,
            myPos.y,
            myPos.z + Mathf.Clamp(enemyPos.z - myPos.z, -1, 1) * attackMoveDistance);

        Debug.Log("my pos " + myPos + ". target pos " + targetPos + " . enemy pos: " + enemyPos);

        float attackMoveToDuration = Vector3.Distance(enemyPos, myPos) / attackMoveToSpeed;
        float attackReturnDuration = Vector3.Distance(enemyPos, myPos) / attackReturnSpeed;

        SP_LevelAudioControl.instance.PlayAttackSFX();

        activePrefab.transform.DOMove(targetPos, attackMoveToDuration).SetEase(attackToEase).
            OnComplete(() =>
            {
                // target reached
                activePrefab.transform.position = targetPos;
                ProcessAttackComplete(targetTile);
                // go back
                activePrefab.transform.DOMove(myPos, attackReturnDuration).SetEase(attackReturnEase).
                    OnComplete(() => 
                    {
                        ProcessAttackUnitAbilities(targetTile);
                        if (!isAllyUnit)
                            aiControl.ProcessActionComplete();
                    });
            }); 
    }

    private void ProcessAttackComplete(SP_Tile targetTile)
    {
        targetTile.myUnit.HurtUnit(myStats.unitDamage, myStats.unitType);
    }

    private void ProcessAttackUnitAbilities(SP_Tile targetTile)
    {
        // UNIT ABILITIES
        // scout - die
        if (myStats.unitType == SP_UnitType.Scout)
        {
            Die();
        }

        // legionary - counterattack
        if (targetTile.myUnit.myStats.unitType == SP_UnitType.Legionary)
        {
            int attackDistance = SP_MapControl.instance.DistanceBetweenTiles(x, y, targetTile.x, targetTile.y);
            if (attackDistance < 2)
            {
                HurtUnit(1, unitDamageSource: SP_UnitType.Legionary);
            }
        }

        // sniper - bonus on grass tiles
        if (myStats.unitType == SP_UnitType.Sniper)
        {
            if (targetTile.myEnvironment == SP_TileType.Grass)
                targetTile.myUnit.HurtUnit(1, SP_UnitType.Sniper);
        }
    }

    public void Die()
    {
        // deselect if was selected
        if (SP_GameControl.instance.prevSelectedUnit == this)
            SP_GameControl.instance.prevSelectedUnit = null;

        // remove from lists
        SP_GameControl.instance.allUnits.Remove(this);

        // ai
        if (!isAllyUnit)
            SP_GameControl.instance.MoveNextEnemy();

        // sfx
        if (isAllyUnit)
            SP_LevelAudioControl.instance.PlaySFX(SP_LevelAudioControl.instance.unitLostSFX);


        Destroy(gameObject);
    }

}
