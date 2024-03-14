using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Building;
using static Character;
using UnityEngine.TextCore.Text;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class CardImage
{
    public bool isCharacter;
    public Sprite sprite;
    public Character.CharacterType charType;
    public Building.BuildingType buildingType;
}

public class CardVisual_Simanis : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// is this a card visual in card stack (true) or player hand (false)
    /// </summary>
    public bool isCardInShop = true;
    private bool isCharacter = true; // false if is building
    
    private Character cardCharacter;
    private Building cardBuilding;

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

    [Header("PLAYER HAND")]
    public Button handCardInteract;

    [Header("CARD HIGHLIGHT")]
    public GameObject cardHighlight;
    public UnityEvent onCardHighlightEvent;
    public bool isImageClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject.GetComponent<Image>() != null)
        {

            onCardHighlightEvent.Invoke();
            HighlightSelectedCard(true);
        }
        else
        {
            HighlightSelectedCard(false);
        }
    }

    private void Start()
    {
        handCardInteract.onClick.AddListener(TryPlayCard);
    }

    public void AddCardToPlayerHand()
    {
        Debug.Log("Card Added to hand " + cardTitle.text);
        handCardInteract.enabled = true;
        isCardInShop = false;
    }

    public void TryPlayCard()
    {
        Debug.Log("trying to play card " + cardTitle.text);
        HighlightSelectedCard();
        if (!CanAffordCard())
            return;

        
        HUD_Simanis hud = HUD_Simanis.instance;
        if (isCharacter)
            hud.ShowAllowedCharPlacementTiles(this.gameObject, cardCharacter);
        else
        {
            hud.ShowAllowedBuildPlacementTiles(this.gameObject, cardBuilding);
            if (cardBuilding == null)
                Debug.Log("issue");
        }
    }

    public void HighlightSelectedCard(bool highlight = true)
    {

        cardHighlight.SetActive(highlight);
        Debug.Log("highlight selected carc");
    }

    private bool CanAffordCard()
    {
        bool afford = true;

        int myGold = CenturionGame.Instance.Teams[0].Gold;
        if (!CenturionGame.Instance.PlayingAsRed)
            myGold = CenturionGame.Instance.Teams[1].Gold;

        if (isCharacter)
        {
            if (cardCharacter.Price > myGold)
            {
                afford = false;
                Debug.Log("Can't afford card");
            }
        }
        else 
        {
            if (cardBuilding.price > myGold)
            {
                afford = false;
                Debug.Log("Can't afford card");
            }
        }
        return afford;
    }

    public void DisplayCharacterCardVisuals(Character character)
    {
        cardTypeIcon.sprite = character.isWarUnit ? warUnitIcon : civilUnitIcon;
        isCharacter = true;
        cardCharacter = character;
        foreach (CardImage cardImage in cardImages)
        {
            if (cardImage.isCharacter && cardImage.charType == character.type)
            {
                Debug.Log("pic found");
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

        isCharacter = false;
        cardBuilding = building;

        foreach (CardImage cardImage in cardImages)
        {
            if (!cardImage.isCharacter && cardImage.buildingType == building.Type)
            {
                Debug.Log("pic found");
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
