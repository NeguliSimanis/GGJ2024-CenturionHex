using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


[System.Serializable]
public class CharacterVisualPrefab
{
    public Character.CharacterType type;
    public GameObject gameObject;
}

public class CharacterVisual_Simanis : MonoBehaviour
{
    public bool isDead = false;
    public CharacterVisualPrefab[] charPrefabs;
    public Character character;
    public TileSpawner_Simanis tileSpawner;
    public HUD_Simanis hudManager;
    public bool isMyUnit;
    public GameObject activePrefab;

    public int xCoord;
    public int yCoord;

    [Header("STATS")]
    public Transform lifeParent;
    public Transform speedParent;
    public GameObject speedIcon;
    public GameObject lifeIcon;

    [Header("ANIMATIONS")]
    public float scaleSpeed = 1.0f; // Adjust the speed as needed
    public float maxScaleY = 1.08f;
    public float minScaleY = 1.0f;
    public Transform scaleAnimationTarget;

    private bool scalingUp = true;

    public void SetCharacterVisuals(Character.CharacterType type, TileSpawner_Simanis newTileSpawner)
    {
        tileSpawner = newTileSpawner;
        hudManager = tileSpawner.gameObject.GetComponent<HUD_Simanis>();
        foreach (CharacterVisualPrefab prefab in charPrefabs)
        {
            if (prefab.type != type)
                prefab.gameObject.SetActive(false);
            else
            {
                activePrefab = prefab.gameObject;
                prefab.gameObject.SetActive(true);
            }
        }
        if (IsMyUnit())
            FlipCharacter();
    }

    public bool IsMyUnit()
    {
        bool isMy = false;
        CenturionGame centurionGame = tileSpawner.centurionGame;

        if (character.Team.Type == Team.TeamType.ttBlue && !centurionGame.PlayingAsRed)
        {
            isMy = true;
            isMyUnit = true;
        }
        if (character.Team.Type == Team.TeamType.ttRed && centurionGame.PlayingAsRed)
        {
            isMy = true;
            isMyUnit = true;
        }
        return isMy;
    }

    public void FlipCharacter()
    {
        
        Vector3 currentScale = activePrefab.transform.localScale;

        // Flip the x scale by negating its value
        currentScale.x *= -1;

        // Apply the new scale to the object
        activePrefab.transform.localScale = currentScale;
    }

    public void SetLifeUI()
    {
        int remainingLife = character.Health;
        Debug.Log(character.type + " life remaining " + remainingLife
            + ". Normal life: " + character.InitialHealth);

        int lifeDisplayed = 0;
        foreach (Transform life in lifeParent)
        {
            if (life.gameObject.activeInHierarchy)
                lifeDisplayed++;
        }
        Debug.Log("life displayed " + lifeDisplayed);
        for (int i = 0; i < lifeDisplayed; i++)
        {
            if (lifeDisplayed > remainingLife)
            {
                GameObject lifeObject = lifeParent.GetChild(i).gameObject;
                if (lifeObject.activeInHierarchy)
                {
                    lifeDisplayed--;
                }
                lifeObject.SetActive(false);
            }
            
        }
        while (lifeDisplayed < remainingLife
            && lifeDisplayed < 50)
        {
            GameObject newLifeIcon = Instantiate(lifeIcon, lifeParent);
            lifeDisplayed++;
        }
    }

    public void SetSpeedUI()
    {
        //character.StepsPerTurn
        //character.StepsUsed

        int remainingSpeed = character.StepsPerTurn - character.StepsUsed;
        Debug.Log(character.type + " speed remaining " + remainingSpeed
            + ". Steps per turn: " + character.StepsPerTurn);

        int speedDisplayed = 0;
        foreach (Transform speed in speedParent)
        {
            if (speed.gameObject.activeInHierarchy)
                speedDisplayed++;
        }
        Debug.Log("speedDisplayed" + speedDisplayed + ". remaining speed " + remainingSpeed);
        for (int i = 0; i < speedParent.childCount; i++)
        {
            if (speedDisplayed > remainingSpeed)
            {
                GameObject speedObject = speedParent.GetChild(i).gameObject;
                if (speedObject.activeInHierarchy)
                {
                    speedDisplayed--;
                }
                speedObject.SetActive(false);
            }

        }
        while (speedDisplayed < remainingSpeed
            && speedDisplayed < 50)
        {
            GameObject newLifeIcon = Instantiate(speedIcon, speedParent);
            Debug.Log(character.type + " adding speed");
            speedDisplayed++;
        }
    }

    public void WoundCharacter()
    {
        if (isDead)
            return;
        CenturionGame centurionGame = tileSpawner.centurionGame;
        if (centurionGame.lastHurterB == null
            && centurionGame.lastHurterC == null)
        {
            Debug.Log("last hurter was a mine");
            TileVisual_Simanis explosionTile = FindTileCharIsOn();
            explosionTile.SpawnExplosion();
            
        }

        SetLifeUI();
        if (character.Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        isDead = true;
        tileSpawner.allCharacters.Remove(this);
        if (hudManager.oldHighlight)
        {
            if (hudManager.oldHighlight.type == RaycastInteract.Type.Character
                && hudManager.oldHighlight.characterVisualControl == this)
            {
                hudManager.ClearHighlights();
            }
        }
        Destroy(gameObject);
    }

    public TileVisual_Simanis FindTileCharIsOn()
    {
        TileVisual_Simanis tileStandingOn = tileSpawner.allTiles[0];
        foreach (TileVisual_Simanis tileVisual in tileSpawner.allTiles)
        {
            if (tileVisual.xCoord == character.x
                && tileVisual.yCoord == character.y)
            {
                tileStandingOn = tileVisual;
            }
        }
        return tileStandingOn;
    }

    public void MoveCharacter(float speed)
    {
        Vector3 moveTarget = Vector3.zero;
        hudManager.isListeningToMoveSuccess = false;
        foreach (TileVisual_Simanis tileVisual in tileSpawner.allTiles)
        {
            if (tileVisual.xCoord == character.x
                && tileVisual.yCoord == character.y)
            {
                transform.parent = tileVisual.unitTransformPos;
                xCoord = tileVisual.xCoord;
                yCoord = tileVisual.yCoord;
                Debug.Log("COD FIND " + tileVisual.xCoord + "." + tileVisual.yCoord);
            }
        }
        Debug.Log("MOVE CALLED");
        SetSpeedUI();
        transform.DOLocalMove(moveTarget, speed)
           .SetEase(Ease.InOutQuad).OnComplete(() => hudManager.ListenToRaycast());
    }

    public Vector3 FindMoveTarget()
    {
        Vector3 moveTarget = transform.position;

        foreach (TileVisual_Simanis tileVisual in tileSpawner.allTiles)
        {
            if (tileVisual.xCoord == character.x
                && tileVisual.yCoord == character.y)
            {
                moveTarget = tileVisual.unitTransformPos.position;
                Debug.Log("COD FIND " + tileVisual.xCoord + "." + tileVisual.yCoord);
            }
        }

        return moveTarget;
    }

  

    void Update()
    {
        float newYScale = scaleAnimationTarget.transform.localScale.y;

        if (scalingUp)
        {
            newYScale += Time.deltaTime * scaleSpeed;
            if (newYScale >= maxScaleY)
            {
                newYScale = maxScaleY;
                scalingUp = false;
            }
        }
        else
        {
            newYScale -= Time.deltaTime * scaleSpeed;
            if (newYScale <= minScaleY)
            {
                newYScale = minScaleY;
                scalingUp = true;
            }
        }

        // Apply the new scale
        scaleAnimationTarget.transform.localScale = 
            new Vector3(scaleAnimationTarget.transform.localScale.x, 
            newYScale,
            scaleAnimationTarget.transform.localScale.z);
    }

}
