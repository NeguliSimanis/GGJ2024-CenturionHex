using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHandVisuals_Simanis : MonoBehaviour
{
    public enum Type
    {
        player,
        enemy,
        deck
    }

    public CardHandVisuals_Simanis.Type type;
    public Transform cardParent;
    public GameObject cardPrefab;

    private void Start()
    {
        
    }

    public void LoadPlayerCards()
    {

    }
}
