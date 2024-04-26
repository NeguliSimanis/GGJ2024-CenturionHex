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

    public void HighlightThis(bool highlight)
    {
        highlightObj.SetActive(highlight);
    }

    public void ShowClickObj(bool show)
    {
        clickedObj.SetActive(show);
    }

    public void ProcessClick()
    {
        if (SP_RaycastControl.instance.previousClicked != null)
        {
            SP_RaycastControl.instance.previousClicked.ShowClickObj(false);
        }

        SP_RaycastControl.instance.previousClicked = this;
        ShowClickObj(true);

        if (isTileInteract)
        {
            Debug.Log("selecting tile from process click");
            myTileControl.SelectTile();
        }
    }
}
