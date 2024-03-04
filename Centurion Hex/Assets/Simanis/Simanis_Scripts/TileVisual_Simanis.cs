using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public enum TileVisualType
{
    GrassRegular,
    GrassBuild,
    ForestRegular,
    ForestBuild,
    Senate
}

[System.Serializable]
public class TilePrefab_Simanis
{
    public GameObject gameObject;
    public Tile.TileType type;
    public bool isCover = false;
}

public class TileVisual_Simanis : MonoBehaviour
{
    public TileSpawner_Simanis tileSpawner;

    public bool allowFlip = false;
    public GameObject tileCenter;

    public TilePrefab_Simanis[] tilePrefabs;
    public Tile tile;
    public GameObject debugTextHolder;
    public TextMeshProUGUI debugText;
    public int xCoord;
    public int yCoord;

    public Transform unitTransformPos;

    [Header("Explosion")]
    public GameObject explosionPrefab;
    public Transform explosionLocation;

    [Header("VICTORY POINT ANIM")]
    public Animator victoryAnimator;
    public GameObject victoryAnimObject;
    public Image animationImage;
    public Sprite victorySprite;
    public Sprite coinSprite;
    public float vicAnimationDuration = 1f;

    [Header("TILE HIGHLIHGTS")]
    public GameObject tileHighlight;

    public void SetTileVisuals(TileSpawner_Simanis spawner)
    {
        tileSpawner = spawner;
        Tile.TileType type = tile.tileType;
        if (tile.tileCover.Type == TileCover.CoverType.ctTransparent)
        {
            DiscoverTile();
            return;
        }
        if (tile.tileCover.Type != TileCover.CoverType.ctUndefined)
        {
            DiscoverTile();
            return;
        }
        foreach (TilePrefab_Simanis prefab in tilePrefabs)
        {
            if (prefab.isCover)
                prefab.gameObject.SetActive(true);
            else
                prefab.gameObject.SetActive(false);
        }
    }

    public void SetTileCoords(int xC, int yC, bool debug = false)
    {
        xCoord = xC;
        yCoord = yC;

        if (debug)
        {
            debugTextHolder.SetActive(true);
            debugText.text = xCoord + "." + yCoord;
        }
    }

    public void SpawnExplosion()
    {
        Debug.Log("spawning explosion");
        GameObject newExplosion = Instantiate(explosionPrefab, explosionLocation.transform);
        newExplosion.transform.localPosition = Vector3.zero;
        newExplosion.transform.localRotation = Quaternion.identity;
        newExplosion.transform.localScale = Vector3.one;
        newExplosion.SetActive(true);
        //explosionPrefab.transform.localRotation = Quaternion.identity;

    }

    public void SpawnGoldGain()
    {
        Debug.Log("spawning gold gain");
        if (tileSpawner.centurionGame.RedMove)
        {
            victoryAnimObject.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            victoryAnimObject.transform.localScale = new Vector3(1, 1, 1);
        }
        animationImage.sprite = coinSprite;
        
        victoryAnimator.SetTrigger("Find");

    }


    public void SpawnVictoryPointGain()
    {
        // flip if u are team red
        if (tileSpawner.centurionGame.RedMove)
        {
            victoryAnimObject.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            victoryAnimObject.transform.localScale = new Vector3(1, 1, 1);
        }
        animationImage.sprite = victorySprite;

        Debug.Log("spawning victory point gain");
        victoryAnimator.SetTrigger("Find");
        DOVirtual.DelayedCall(vicAnimationDuration, () =>
        {
            victoryAnimator.SetTrigger("Hide");
        });

    }

    public void DiscoverTile()
    {
        Tile.TileType type = tile.tileType;
        foreach (TilePrefab_Simanis prefab in tilePrefabs)
        {
            if (prefab.type != type)
                prefab.gameObject.SetActive(false);
            else
                prefab.gameObject.SetActive(true);
        }
    }

    public void FlipTile()
    {
        if (!allowFlip)
            return;
        Quaternion currentRotation = tileCenter.transform.rotation;

        // Set the rotation on the y-axis to 180 degrees
        currentRotation.eulerAngles = new Vector3(0, 180, 0);

        // Apply the new rotation to the object
        tileCenter.transform.rotation = currentRotation;
    }

    public void ShowMessage(string message)
    {
        debugTextHolder.SetActive(true);
        debugText.text = message;
    }

    public void HighlightTile(bool highlight)
    {
        tileHighlight.SetActive(highlight);
    }
}
