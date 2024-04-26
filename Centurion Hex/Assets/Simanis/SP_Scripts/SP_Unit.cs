using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SP_Unit : MonoBehaviour
{
    public bool isMyUnit = true;

    [Header("Coordinates")]
    public int x;
    public int y;
    public SP_Tile parentTile = null;

    [Header("Selction")]
    public GameObject selectAnimation;

    [Header("MOVE CONTROLS")]
    

    [Header("MOVE ANIMATION")]
    public float moveDuration = 1f;
    public Ease moveEase;
    private bool isMoveAnimating = false;

    private void Start()
    {
       // MoveUnit(x, y, instant: true);
    }

    public void SelectUnit(bool select)
    {
       // Debug.Log("SELECTED UNIT!");
        selectAnimation.SetActive(select);
        SP_GameControl.instance.prevSelectedUnit = this;
    }

    public void MoveUnit(int targetX, int targetY, bool instant = false,
        bool selectTargetTileAfter = false)
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
            ProcessMoveComplete(targetX, targetY, targetTile, selectTargetTileAfter);
        }
        else
        {
            transform.DOLocalMove(Vector3.zero, moveDuration).SetEase(ease: moveEase).
                OnComplete(()=>
                {
                    ProcessMoveComplete(targetX, targetY, targetTile, selectTargetTileAfter);
                });
        }
    }


    private void ProcessMoveComplete(int newX, int newY, SP_Tile targetTile,
        bool selectParentTile = false)
    {
        // set new coordinates
        x = newX;
        y = newY;

        // set new parent
        parentTile = targetTile;
        targetTile.myUnit = this;

        isMoveAnimating = false;

        if (selectParentTile)
        {
            parentTile.SelectTile();
        }
    }

}
