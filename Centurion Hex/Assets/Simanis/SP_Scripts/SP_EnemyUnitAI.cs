using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum UnitBehaviour
{
    RandomMovePacifist,
    RandomMoveAttackWhenAdjacent
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
            case UnitBehaviour.RandomMoveAttackWhenAdjacent:
                TryToAttackAdjacentTile();
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
            myUnit.MoveUnit(targetTile.x, targetTile.y, speedCost: 1, isAI: true);
        }
        else
        {
            Debug.Log("no empty adjacent tiles " + myUnit.myStats.unitType);
            EndMove();
        }
    }

    private void TryToAttackAdjacentTile()
    {
        if (SP_MapControl.instance.RandomAdjacentAllyTile(myUnit.x, myUnit.y) != null)
        {
            myUnit.PlayAttackAnimation(SP_MapControl.instance.RandomAdjacentAllyTile(myUnit.x, myUnit.y));
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
