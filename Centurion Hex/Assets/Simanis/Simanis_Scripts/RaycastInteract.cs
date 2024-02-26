using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastInteract : MonoBehaviour
{
    public enum HighlightType
    {
        MoveUnit,
        Undefined
    }

    public enum Type
    {
        Character,
        Building,
        Tile,
        Null
    }
    public RaycastInteract.Type type;

    public bool shouldHighlightComponent = false;
    public GameObject highlightObject;
    public Outline highlightComponent;

    [Header("char control")]
   // public bool isCharacter = false;
    public CharacterVisual_Simanis characterVisualControl;

    [Header("tile control")]
    //public bool isTile = false;
    public TileVisual_Simanis tileVisualControl;

    [Header("building control")]
    //public bool isTile = false;
    public BuildingVisual_Simanis buildingVisualControl;


    private void Start()
    {
        if (!shouldHighlightComponent)
        {
           // Debug.Log(gameObject.name);
           // if(hi)
            highlightObject.SetActive(false);
        }
        else
        {
            highlightComponent.enabled = false;
        }
    }

  //  public void 

    public bool BelongsToThisPlayer()
    {
        bool belongsToThisPlayer = false;

        if (type == RaycastInteract.Type.Building)
        {
            if (buildingVisualControl.IsMyBuilding())
                belongsToThisPlayer = true;
        }
        else if (type == RaycastInteract.Type.Character)
        {
            if (characterVisualControl.isMyUnit)
                belongsToThisPlayer = true;
        }

        return belongsToThisPlayer;
    }


    public void SetHighlight(
        bool set, 
        RaycastInteract.HighlightType highlightType = RaycastInteract.HighlightType.Undefined)
    {
        // exception - don't highlight enemy unit
        if (highlightType == RaycastInteract.HighlightType.MoveUnit && set)
        {
            if (type == RaycastInteract.Type.Character && !BelongsToThisPlayer())
            {
                Debug.Log("Won't highlight unit, belongs to enemy ");
                return;
            }
        }

        // highlight
        if (shouldHighlightComponent)
        {
            highlightComponent.enabled = set;
            return;
        }
        highlightObject.SetActive(set);
    }
}
