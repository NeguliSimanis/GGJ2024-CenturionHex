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
    public string characterName;
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
    public ParticleSystem bloodAnimation;
    public GameObject zzzAnimation;
    public float scaleSpeed = 1.0f; // Adjust the speed as needed
    public float maxScaleY = 1.08f;
    public float minScaleY = 1.0f;
    public Transform scaleAnimationTarget;
    private bool scalingUp = true;

    /*
     * character fades to transparent and sprite goes up
     */
    [Header("death animation")]
    public float deathAnimationDuration = 0.9f;
    public float deathAnimationHeight = 2f; 

    private void Start()
    {
        zzzAnimation.SetActive(false);
    }

    private void FixedUpdate()
    {
        return;
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayBloodAnimation();
        }
    }

    public void SetCharacterVisuals(Character.CharacterType type, TileSpawner_Simanis newTileSpawner)
    {
        tileSpawner = newTileSpawner;
        hudManager = tileSpawner.gameObject.GetComponent<HUD_Simanis>();
        foreach (CharacterVisualPrefab prefab in charPrefabs)
        {
            if (prefab.type == type || prefab.characterName == character.Name)
            {
                activePrefab = prefab.gameObject;
                prefab.gameObject.SetActive(true);
            }
            else
                prefab.gameObject.SetActive(false);
        }
        if (IsMyUnit())
            FlipCharacter();
        ColorUnit();
    }

    public void ColorUnit()
    {
        CharacterColorChanger_Simanis colorChanger;
        colorChanger = activePrefab.GetComponent<CharacterColorChanger_Simanis>();

        int teamID = GetTeamColorID();
        //Debug.Log("im playing as red " + CenturionGame.Instance.PlayingAsRed + "");
        colorChanger.ChangeColor(teamID);
    }

    public void MarkUnitAsInactive(bool markAsInactive)
    {
        if (markAsInactive)
        {
            //Debug.Log("marking as SLEEPY " + character.Name + ". This is my character: " + IsMyUnit());
            zzzAnimation.SetActive(true);
           // speedParent.gameObject.SetActive(false);
        }
        else
        {
            //Debug.Log("marking as not sleepy, ACTIVE " + character.Name + ". This is my character: " + IsMyUnit());
            zzzAnimation.SetActive(false);
            //speedParent.gameObject.SetActive(true);
        }
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
        if (IsMyUnit() && CenturionGame.Instance.PlayingAsRed)
            teamID = 0;

        // is red if not my unit and not playing as red
        if (!IsMyUnit() && !CenturionGame.Instance.PlayingAsRed)
            teamID = 0;
        return teamID;
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

    public bool IsWarUnit()
    {
        bool isWar = character.isWarUnit;
        return isWar;
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
        //Debug.Log(character.type + " life remaining " + remainingLife
        //    + ". Normal life: " + character.InitialHealth);

        int lifeDisplayed = 0;
        foreach (Transform life in lifeParent)
        {
            if (life.gameObject.activeInHierarchy)
                lifeDisplayed++;
        }
        //Debug.Log("life displayed " + lifeDisplayed);
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

    public void SetSpeedUI(bool isNewRoundPhase = false)
    {
        //character.StepsPerTurn
        //character.StepsUsed

        
        int remainingSpeed = character.RemainingStepsThisTurn();
        if (isNewRoundPhase)
            remainingSpeed += character.StepsUsed;
        //Debug.Log(character.type + " speed remaining " + remainingSpeed
        //    + ". Steps per turn: " + character.StepsPerTurn);

        int speedDisplayed = 0;
        foreach (Transform speed in speedParent)
        {
            if (speed.gameObject.activeInHierarchy)
                speedDisplayed++;
        }
        //Debug.Log("speedDisplayed" + speedDisplayed + ". remaining speed " + remainingSpeed);
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
            //Debug.Log(character.type + " adding speed");
            speedDisplayed++;
        }
    }

    private void PlayBloodAnimation()
    {
        bloodAnimation.gameObject.SetActive(true);
        bloodAnimation.Play();
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
        else if (character.Health > 0)
        {
            
        }
        

        SetLifeUI();
        if (character.Health <= 0)
        {
            Die();
        }
        else if (centurionGame.lastHurterB != null
            || centurionGame.lastHurterC != null)
        {
            Debug.Log("got hurt!");
            PlayBloodAnimation();
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
        PlayDeathAnimation();
    }

    private void PlayDeathAnimation()
    {
        // fade to transparent
        CharacterColorChanger_Simanis colorChanger;
        colorChanger = activePrefab.GetComponent<CharacterColorChanger_Simanis>();
        colorChanger.FadeToTransparent(deathAnimationDuration);

        // Move sprite up
        Vector3 targetPosition = new Vector3(activePrefab.transform.position.x,
          activePrefab.transform.position.y + deathAnimationHeight,
          activePrefab.transform.position.z);
        activePrefab.transform.DOMove(targetPosition, deathAnimationDuration * 1.01f).OnComplete(() =>
        {
            Destroy(gameObject);
        });
        
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
                //Debug.Log("COD FIND " + tileVisual.xCoord + "." + tileVisual.yCoord);
            }
        }
        //Debug.Log("MOVE CALLED");
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
