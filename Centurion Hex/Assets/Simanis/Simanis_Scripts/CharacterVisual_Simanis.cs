using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    public void SetCharacterVisuals(Character.CharacterType type)
    {
        foreach (CharacterVisualPrefab prefab in charPrefabs)
        {
            if (prefab.type != type)
                prefab.gameObject.SetActive(false);
            else
                prefab.gameObject.SetActive(true);
        }
    }
    
}
