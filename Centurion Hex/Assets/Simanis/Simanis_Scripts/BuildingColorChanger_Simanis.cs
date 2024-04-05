using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingColorChanger_Simanis : MonoBehaviour
{
    public GameObject team0_colorObject;
    public GameObject team1_colorObject;


    public void ColorBuilding(int teamID)
    {
        if (teamID == 0)
        {
            team0_colorObject.SetActive(true);
            team1_colorObject.SetActive(false);
        }
        else
        {
            team1_colorObject.SetActive(true);
            team0_colorObject.SetActive(false);
        }
    }
}
