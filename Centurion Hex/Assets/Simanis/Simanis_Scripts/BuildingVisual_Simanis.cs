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
    private GameObject activePrefab;
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
            {
                prefab.gameObject.SetActive(false);
            }
            else// if (prefab.type != Building.BuildingType.btSenate)
            {
                //Debug.Log("building " + Building.BuildingType.btSenate);
                activePrefab = prefab.gameObject;
                prefab.gameObject.SetActive(true);
            }
        }
        ColorBuilding();
    }

    private void ColorBuilding()
    {
        BuildingColorChanger_Simanis colorChanger;
        colorChanger = activePrefab.GetComponent<BuildingColorChanger_Simanis>();

        int teamID = GetTeamColorID();
        Debug.Log("im playing as red " + CenturionGame.Instance.PlayingAsRed + " " + activePrefab.name);
        colorChanger.ColorBuilding(teamID);
    }

    /// <summary>
    /// 0 - red team
    /// 1 - not red team
    /// </summary>
    /// <returns></returns>
    private int GetTeamColorID()
    {
        int teamID = 1;
        // is red if is my unit and I'm playing as red
        if (IsMyBuilding() && CenturionGame.Instance.PlayingAsRed)
            teamID = 0;

        // is red if not my unit and not playing as red
        if (!IsMyBuilding() && !CenturionGame.Instance.PlayingAsRed)
            teamID = 0;
        return teamID;
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
