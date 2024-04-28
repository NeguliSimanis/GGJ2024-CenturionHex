using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SP_InfoIcon : MonoBehaviour
{
    public static SP_InfoIcon instance;

    // info icon
    public GameObject infoIcon;

    // info panel
    public GameObject infoPanel;
    public float infoPanelOffset_x = 5f;
    public float infoPanelOffset_y = 5f;
    private SP_InfoPopup infoPopupControl;

    private SP_Tile selectedTile;

    private void Awake()
    {
        instance = this;
        infoIcon.GetComponent<Button>().onClick.AddListener(() => ShowInfoPanel());
        infoPopupControl = infoPanel.GetComponent<SP_InfoPopup>();
    }


    public void ShowInfoIcon(GameObject iconPosition, SP_Tile tile, bool show = true)
    {
        if (show)
        {
            infoIcon.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, iconPosition.transform.TransformPoint(Vector3.zero));
            selectedTile = tile;
        }
        else
        {
            HideInfoPanel();
        }
        infoIcon.gameObject.SetActive(show);

    }

    public void HideInfoPanel()
    {
        infoPanel.SetActive(false);
    }

    public void ShowInfoPanel()
    {
        infoPanel.SetActive(!infoPanel.activeInHierarchy);
        
        if (infoPanel.activeInHierarchy)
        {
            infoPopupControl.SetPopupData(selectedTile);
            Vector3 infoPanelPos = infoIcon.transform.position;
            infoPanelPos.x += infoPanelOffset_x;
            infoPanelOffset_y += infoPanelOffset_y;
            infoPanel.transform.position = infoPanelPos;
        }
    }
}
