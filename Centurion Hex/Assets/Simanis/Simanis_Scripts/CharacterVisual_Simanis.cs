using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


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

    public void WoundCharacter()
    {
        if (isDead)
            return;
        if (character.Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        isDead = true;
        tileSpawner.allCharacters.Remove(this);
        if (hudManager.oldHighlight.type == RaycastInteract.Type.Character
            && hudManager.oldHighlight.characterVisualControl == this)
        {
            hudManager.ClearHighlights();
        }
        Destroy(gameObject);
    }

    public void MoveCharacter(float speed)
    {
        Vector3 target = FindMoveTarget();
        Debug.Log("MOVE CALLED");
        transform.DOMove(target, speed)
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
    
}
