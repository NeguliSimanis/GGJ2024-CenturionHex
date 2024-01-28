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


    private void Start()
    {
        if (!shouldHighlightComponent)
        {
            Debug.Log(gameObject.name);
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
