using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;


[System.Serializable]
public class BuildingVisualPrefab
{
    public Building.BuildingType type;
    public GameObject gameObject;
}

public class BuildingVisual_Simanis : MonoBehaviour
{
    public bool isDead = false;

    public BuildingVisualPrefab[] buildingPrefabs;
    public Building building;

    public int xCoord;
    public int yCoord;

    TileSpawner_Simanis tileSpawner;

    public void SetBuildingVisuals(Building.BuildingType type, TileSpawner_Simanis newTileSpawner)
    {
        tileSpawner = newTileSpawner;
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

    public bool IsMyBuilding()
    {
        bool isMy = false;
        CenturionGame centurionGame = tileSpawner.centurionGame;

        if (building.Team.Type == Team.TeamType.ttBlue && !centurionGame.PlayingAsRed)
        {
            isMy = true;
        }
        if (building.Team.Type == Team.TeamType.ttRed && centurionGame.PlayingAsRed)
        {
            isMy = true;
        }
        return isMy;
    }

    public void WoundBuilding()
    {
        if (isDead)
            return;
        if (building.Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        isDead = true;
        tileSpawner.allBuildings.Remove(this);
        /*
         * if (hudManager.oldHighlight)
        {
            if (hudManager.oldHighlight.type == RaycastInteract.Type.Character
                && hudManager.oldHighlight.characterVisualControl == this)
            {
                hudManager.ClearHighlights();
            }
        }
        */
        Destroy(gameObject);
    }
}
