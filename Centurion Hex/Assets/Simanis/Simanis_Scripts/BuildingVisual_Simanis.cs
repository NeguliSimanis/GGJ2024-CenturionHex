using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BuildingVisualPrefab
{
    public Building.BuildingType type;
    public GameObject gameObject;
}

public class BuildingVisual_Simanis : MonoBehaviour
{
    public BuildingVisualPrefab[] buildingPrefabs;
    public Building building;

    public void SetBuildingVisuals(Building.BuildingType type)
    {
        foreach (BuildingVisualPrefab prefab in buildingPrefabs)
        {
            if (prefab.type != type)
                prefab.gameObject.SetActive(false);
            else if (prefab.type != Building.BuildingType.btSenate)
            {
                Debug.Log("building " + Building.BuildingType.btSenate);
                prefab.gameObject.SetActive(true);
            }
        }
    }
}
