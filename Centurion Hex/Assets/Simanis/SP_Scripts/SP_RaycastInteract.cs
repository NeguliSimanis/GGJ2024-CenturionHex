using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_RaycastInteract : MonoBehaviour
{
    public GameObject highlightObj;
    public GameObject clickedObj;

    [Header("Tile interact")]
    public bool isTileInteract;
    public SP_Tile myTileControl;


    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            HighlightThis(false);
            ShowClickObj(false);
        }
    }

    public void HighlightThis(bool highlight)
    {

        highlightObj.SetActive(highlight);

        // show move cursor
        if (highlight && SP_GameControl.instance.prevSelectedUnit != null
            && SP_GameControl.instance.prevSelectedUnit.isAllyUnit)
        {
            SetContextualCursor(SP_GameControl.instance.prevSelectedUnit);
        }
        else
        {
            SP_GameControl.instance.customCursor.SetCursor(false, cursorAction: CursorAction.walk);
        }
    }

    private void SetContextualCursor(SP_Unit unitThatWillDoStuff)
    {
        if (!isTileInteract)
            return;

        // attack unit cursor
        if (myTileControl.myUnit != null)
        {
            if (!myTileControl.myUnit.isAllyUnit && !unitThatWillDoStuff.myStats.hasAttackedThisTurn)
            {
                SP_GameControl.instance.customCursor.SetCursor(true, cursorAction: CursorAction.attack);
                unitThatWillDoStuff.readyToAttack = true;
            }
            // already attacked this turn or is ally
            else
            {
                SP_GameControl.instance.customCursor.SetCursor(false, cursorAction: CursorAction.attack);
                unitThatWillDoStuff.readyToAttack = false;
            }
        }
        // attack building cursor
        else if (myTileControl.myBuilding != null)
        {
            if (!myTileControl.myBuilding.isAllyBuilding && !unitThatWillDoStuff.myStats.hasAttackedThisTurn)
            {
                SP_GameControl.instance.customCursor.SetCursor(true, cursorAction: CursorAction.attack);
                unitThatWillDoStuff.readyToAttack = true;
            }
            // already attacked this turn or is ally
            else
            {
                SP_GameControl.instance.customCursor.SetCursor(false, cursorAction: CursorAction.attack);
                unitThatWillDoStuff.readyToAttack = false;
            }
        }
        // walk cursor
        else
        {
            SP_GameControl.instance.customCursor.SetCursor(true, cursorAction: CursorAction.walk);
            unitThatWillDoStuff.readyToAttack = false;
        }
    }

    public void ShowClickObj(bool show)
    {
        clickedObj.SetActive(show);
        if (isTileInteract)
        {
            SP_InfoIcon.instance.ShowInfoIcon(myTileControl.infoIcon, myTileControl, show);
        }
    }

    public void ProcessClick()
    {
        if (isTileInteract)
            ProcessClick_TileInteract();
    }

    private void ProcessClick_TileInteract()
    {
        // DON'T SHOW CLICK OBJ IF YOU WERE ATTACKING WITH ALLY
        if (SP_GameControl.instance.prevSelectedUnit != null &&
            SP_GameControl.instance.prevSelectedUnit.readyToAttack)
        {

        }
        // Show click obj
        else
        {
            
            if (SP_RaycastControl.instance.previousClicked != null)
            {
                SP_RaycastControl.instance.previousClicked.ShowClickObj(false);
            }

            ShowClickObj(true);
            SP_RaycastControl.instance.previousClicked = this;
        }
        
        //Debug.Log("selecting tile from process click");
        myTileControl.SelectTile();
        
    }
}
