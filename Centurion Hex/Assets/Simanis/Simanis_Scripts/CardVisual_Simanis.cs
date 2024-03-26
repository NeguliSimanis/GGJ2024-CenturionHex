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
using DG.Tweening;

[System.Serializable]
public class CardImage
{
    public bool isCharacter;
    public Sprite sprite;
    public Character.CharacterType charType;
    public Building.BuildingType buildingType;
}

public class CardVisual_Simanis : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// is this a card visual in card stack (true) or player hand (false)
    /// </summary>
    public bool isCardInShop = true;
    private bool isCharacter = true; // false if is building
    
    private Character cardCharacter;
    private Building cardBuilding;

    public bool isCardInHand = false;

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
    public bool isImageClicked;

    [Header("Raising card on hover")]
    public RectTransform cardVisualContainer;
    public float cardRaiseDuration = 4f; // card is raised on mouse hover. This determines how quickly
   
    private Tween cardRaiseTween;
    private float cardRaiseDistance;
    private bool isCardRaised = false;
    private bool isRaisingCard = false;
    private bool isLoweringCard = false;
    private Vector3 cardVisualDefaultPos;


    private void Start()
    {
        handCardInteract.onClick.AddListener(TryPlayCard);
        cardRaiseDistance = cardVisualContainer.rect.height;

        
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    if (cardRaiseTween != null && cardRaiseTween.IsActive())
        //        cardRaiseTween.Kill();
        //}
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // remove highlight from currently highlighted card
        if (HUD_Simanis.instance.highlightedCardVisual != null)
            HUD_Simanis.instance.highlightedCardVisual.HighlightSelectedCard(false);

        HighlightSelectedCard(true);
     
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isCardInHand)
            LiftCardVisual(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(isCardInHand)
            LiftCardVisual(false);
    }

    public void AddCardToPlayerHand()
    {
        Debug.Log("Card Added to hand " + cardTitle.text);
        handCardInteract.enabled = true;
        isCardInShop = false;
        
        DOVirtual.DelayedCall(0.02f, () =>
        {
            isCardInHand = true;
            cardVisualDefaultPos = cardVisualContainer.localPosition;
        });
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

    public void LiftCardVisual(bool lift = true)
    {
        // don't raise card if raised
        if (isCardRaised && lift)
            return;
        // don't raise if raising
        if (isRaisingCard && lift)
            return;
        // don't lower if lowered
        if (cardVisualDefaultPos.y == cardVisualContainer.position.y && !lift)
            return;
        // don't lower if lowring
        if (isLoweringCard && !lift)
            return;
        // kill old tween
        if (cardRaiseTween != null && cardRaiseTween.IsActive())
            cardRaiseTween.Kill();

        Vector3 targetPos = cardVisualDefaultPos;

        // raise card
        if (lift)
        {
            isRaisingCard = true;
            isLoweringCard = false;
            targetPos.y += cardRaiseDistance;
            cardRaiseTween = cardVisualContainer.
                DOLocalMove(targetPos, cardRaiseDuration).
                SetEase(Ease.InOutQuad).OnComplete(()=>
                {
                    isCardRaised = true;
                    isRaisingCard = false;
                });
        }
        // lower card
        else
        {
            isRaisingCard = false;
            isLoweringCard = true;
            isCardRaised = false;
            cardRaiseTween = cardVisualContainer.
                DOLocalMove(targetPos, cardRaiseDuration).
                SetEase(Ease.InOutQuad).OnComplete(() =>
                {
                    isLoweringCard = false;
                }); 
        }
    }

    public void HighlightSelectedCard(bool highlight = true)
    {
        HUD_Simanis.instance.highlightedCardVisual = this;
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
                //Debug.Log("pic found");
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
                //Debug.Log("pic found");
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
