using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

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

    [Header("Tile covers")]
    public GameObject tileCover;




    public void DiscoverTile()
    {

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
                SP_GameControl.instance.prevSelectedUnit.MoveUnit(x, y, selectTargetTileAfter: true);
            }
        }
    }
}
