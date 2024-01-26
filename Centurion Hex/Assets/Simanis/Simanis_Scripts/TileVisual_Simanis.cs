using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum TileVisualType
{
    GrassRegular,
    GrassBuild,
    ForestRegular,
    ForestBuild
}

[System.Serializable]
public class TilePrefab_Simanis
{
    public GameObject gameObject;
    public TileVisualType type;
}

public class TileVisual_Simanis : MonoBehaviour
{
    public TilePrefab_Simanis[] tilePrefabs;
    public Tile tile;
    public GameObject debugTextHolder;
    public TextMeshProUGUI debugText;


    public void SetTileVisuals(TileVisualType type)
    {
        foreach (TilePrefab_Simanis prefab in tilePrefabs)
        {
            if (prefab.type != type)
                prefab.gameObject.SetActive(false);
            else
                prefab.gameObject.SetActive(true);
        }
    }

    public void ShowMessage(string message)
    {
        debugTextHolder.SetActive(true);
        debugText.text = message;
    }
}
