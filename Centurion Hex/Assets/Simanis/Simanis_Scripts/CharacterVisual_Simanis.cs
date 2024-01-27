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

    public CharacterVisualPrefab[] charPrefabs;
    public Character character;
    public TileSpawner_Simanis tileSpawner;
    public HUD_Simanis hudManager;

    public void SetCharacterVisuals(Character.CharacterType type, TileSpawner_Simanis newTileSpawner)
    {
        tileSpawner = newTileSpawner;
        hudManager = tileSpawner.gameObject.GetComponent<HUD_Simanis>();
        foreach (CharacterVisualPrefab prefab in charPrefabs)
        {
            if (prefab.type != type)
                prefab.gameObject.SetActive(false);
            else
                prefab.gameObject.SetActive(true);
        }
    }

    public void MoveCharacter(float speed)
    {
        Vector3 target = FindMoveTarget();
        transform.DOMove(target, speed)
           .SetEase(Ease.InOutQuad).OnComplete(() => hudManager.UnitMoveComplete());
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
                Debug.Log("COD FIND");
            }
        }

        return moveTarget;
    }
    
}
