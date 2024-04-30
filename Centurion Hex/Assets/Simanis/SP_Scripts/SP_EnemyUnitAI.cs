using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum UnitBehaviour
{
    RandomMovePacifist,
    RandomMoveAttackWhenInRange
}

public class SP_EnemyUnitAI : MonoBehaviour
{
    public SP_Unit myUnit;
    public UnitBehaviour myBehaviour;
    public bool hasCompletedTurn = false;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            DoMove();
        }
    }

    public void DoMove()
    {
        switch (myBehaviour)
        {
            case UnitBehaviour.RandomMovePacifist:
                MoveToRandomTile();
                break;
            case UnitBehaviour.RandomMoveAttackWhenInRange:
                TryToAttackNearbyEnemy();
                break;
        }
    }

    private void MoveToRandomTile()
    {
        if (myUnit.myStats.unitCurrSpeed <= 0)
        {
            EndMove();
            return;
        }
        if (SP_MapControl.instance.HasEmptyAdjacentTiles(myUnit.x, myUnit.y))
        {
            SP_Tile targetTile = SP_MapControl.instance.GetRandomAdjacentTile(myUnit.x, myUnit.y);
            int additionalSpeedCost = 0;
            if (targetTile.isSlowTile && !targetTile.isDiscovered)
                additionalSpeedCost++;
            myUnit.MoveUnit(targetTile.x, targetTile.y, speedCost: 1 + additionalSpeedCost, isAI: true);
        }
        else
        {
            Debug.Log("no empty adjacent tiles " + myUnit.myStats.unitType);
            EndMove();
        }
    }

    private void TryToAttackNearbyEnemy()
    {
        // meleee attack
        if (SP_MapControl.instance.RandomAdjacentAllyTile(myUnit.x, myUnit.y) != null
            && !myUnit.myStats.hasAttackedThisTurn)
        {
            myUnit.PlayAttackAnimation(SP_MapControl.instance.RandomAdjacentAllyTile(myUnit.x, myUnit.y));
        }
        // no one in range - try ranged attack
        else if (myUnit.myStats.attackRange > 1 && !myUnit.myStats.hasAttackedThisTurn)
        {
            bool rangeTargetFound = false;
            foreach (SP_Unit unit in SP_GameControl.instance.allUnits)
            {
                if (unit.isAllyUnit && !unit.myStats.isDead)
                {
                    if (SP_MapControl.instance.DistanceBetweenTiles(unit.x, unit.y, myUnit.x, myUnit.y)
                        <= myUnit.myStats.attackRange)
                    {
                        myUnit.PlayAttackAnimation(unit.parentTile);
                        rangeTargetFound = true;
                        break;
                    }
                }
            }
            if (!rangeTargetFound)
                MoveToRandomTile();
        }
        else
        {
            MoveToRandomTile();
        }
    }

    private void EndMove()
    {
        hasCompletedTurn = true;
        SP_GameControl.instance.MoveNextEnemy();
    }

    public void ProcessActionComplete()
    {
        if(myUnit.myStats.isDead)
        {
            EndMove();
            return;
        }

        if (SP_GameControl.instance.isAllyTurn)
        {
            EndMove();
            return;
        }

        if (myUnit.myStats.unitCurrSpeed > 0)
        {
            DoMove();
        }
        else if (!myUnit.myStats.hasAttackedThisTurn)
        {
            DoMove();
        }
        else
        {
            EndMove();
        }
    }
}
