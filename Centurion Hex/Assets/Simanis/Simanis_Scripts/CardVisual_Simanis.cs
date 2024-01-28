using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class CardImage
{
    public bool isCharacter;
    public Sprite sprite;
    public Character.CharacterType charType;
    public Building.BuildingType buildingType;
}

public class CardVisual_Simanis : MonoBehaviour
{
    public CardImage[] cardImages;
    public Image cardMainImage;

    public TextMeshProUGUI cardTitle;
    public TextMeshProUGUI cardAbility;
    public TextMeshProUGUI cardCost;

    public GameObject life1;
    public GameObject life2;

    [Header("CARD TYPE ICONS")]
    public Image cardTypeIcon;
    public Sprite civilUnitIcon;
    public Sprite civilBuildIcon;
    public Sprite warUnitIcon;
    public Sprite warBuildIcon;

    [Header("BUILDING STATS")]
    public GameObject buildingStats;
    public TextMeshProUGUI vicPointAmount;
    public GameObject buildPlaceNextToAlly;
    public GameObject buildPlaceAny;
    public GameObject buildPlaceWood;
    public GameObject buildPlaceGrass;

    [Header("UNIT STATS")]
    public GameObject unitStats;
    public GameObject unitRange;
    public TextMeshProUGUI unitDamage;
    public TextMeshProUGUI unitSpeed;

    public void DisplayCharacterCardVisuals(Character.CharacterType type)
    {
        foreach(CardImage cardImage in cardImages)
        {
            if (cardImage.isCharacter && cardImage.charType == type)
            {
                cardMainImage.sprite = cardImage.sprite;
            }
        }
    }

    public void DisplayBuildCardVisuals(Building.BuildingType type)
    {
        foreach (CardImage cardImage in cardImages)
        {
            if (!cardImage.isCharacter && cardImage.buildingType == type)
            {
                cardMainImage.sprite = cardImage.sprite;
            }
        }
    }

}
