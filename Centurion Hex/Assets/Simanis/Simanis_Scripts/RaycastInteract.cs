using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastInteract : MonoBehaviour
{
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
            Debug.Log(gameObject.name);
           // if(hi)
            highlightObject.SetActive(false);
        }
        else
        {
            highlightComponent.enabled = false;
        }
    }

  //  public void 

    public void SetHighlight(bool set)
    {
        if (shouldHighlightComponent)
        {
            highlightComponent.enabled = set;
            return;
        }
        highlightObject.SetActive(set);
    }
}
