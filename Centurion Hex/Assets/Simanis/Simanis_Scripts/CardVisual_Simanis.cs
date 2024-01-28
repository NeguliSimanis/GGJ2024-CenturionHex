using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Building;
using static Character;
using UnityEngine.TextCore.Text;

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

    public void DisplayCharacterCardVisuals(Character character)
    {
        cardTypeIcon.sprite = character.isWarUnit ? warUnitIcon : civilUnitIcon;

        foreach (CardImage cardImage in cardImages)
        {
            if (cardImage.isCharacter && cardImage.charType == character.type)
            {
                cardMainImage.sprite = cardImage.sprite;
            }
        }
        buildingStats.SetActive(false);
        unitStats.SetActive(true);

        cardTitle.text = character.Name;
        cardAbility.text = character.Description;
        cardCost.text = character.Price.ToString();
        unitDamage.text = character.AttackDamage.ToString();
        unitSpeed.text = character.StepsPerTurn.ToString();

        //hacky
        unitRange.GetComponentInChildren<TextMeshProUGUI>().text = character.AttackRange.ToString();
        life2.SetActive(character.InitialHealth > 1);
    }

    public void DisplayBuildCardVisuals(Building building)
    {
        cardTypeIcon.sprite = building.Class == BuildingClass.bcWar ? warBuildIcon : civilBuildIcon;

        foreach (CardImage cardImage in cardImages)
        {
            if (!cardImage.isCharacter && cardImage.buildingType == building.Type)
            {
                cardMainImage.sprite = cardImage.sprite;
            }
        }
        cardTitle.text = building.Name;
        cardAbility.text = building.Description;
        cardCost.text = building.price.ToString();

        buildingStats.SetActive(true);
        unitStats.SetActive(false);

        buildPlaceNextToAlly.SetActive( building.requireNextToAlly );
        buildPlaceAny.SetActive(building.requiredTileType == TileCover.CoverType.ctUndefined );
        buildPlaceWood.SetActive(building.requiredTileType == TileCover.CoverType.ctForest);
        buildPlaceGrass.SetActive(building.requiredTileType == TileCover.CoverType.ctGrass);
        vicPointAmount.text = building.victoryPoints.ToString();

        life2.SetActive(building.InitialHealth > 1);
    }
}
