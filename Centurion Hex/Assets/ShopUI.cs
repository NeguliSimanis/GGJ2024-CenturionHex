using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ShopUI : MonoBehaviour
{
    private bool isShopOpen = false;
    public static ShopUI instance;

    public Button openShopButton;

    public GameObject Shop;
    public TextMeshProUGUI shopTitle;
    public TextMeshProUGUI BuildingStackTitle;
    public CardVisual_Simanis BuildingsStack;
    public TextMeshProUGUI CharactersStackTitle;
    public CardVisual_Simanis CharactersStack;
    public CenturionGame game;
    public Button BuyBuilding;
    public Button BuyCharacter;
    public GameObject CardPrefabForHandCards;
    public GameObject HandCardHolder;

    [Header("Animations and timings")]
    public float shopAppearDelay = 2.5f;
    public float shopAppearDuration = 0.5f;
    private Vector3 shopDefaultPos;
    private float shopMoveDistance;
    public Image shopBlurBG;

    private void Awake()
    {
        instance = this;
        shopMoveDistance = Screen.height * 1.1f;
        shopDefaultPos = Shop.transform.position;
        openShopButton.onClick.AddListener(() =>ShowUI(showWithDelay: false, animate: true ));
    }

    void Start()
    {
        BuyBuilding.onClick.AddListener(() =>
        {
            Debug.Log("Trying to buy building");
            Network.instance.BuyBuilding();
        });
        BuyCharacter.onClick.AddListener(() =>
        {
            Debug.Log("Trying to buy character");
            Network.instance.BuyCharacter();
        });
        HideUI();
        FillPlayerHand();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MaybeOpen(bool animate)
    {
        Debug.Log("Checking shop " + game.mRoundState);
        if( game.mRoundState == CenturionGame.RoundState.rsManagement )
        {
            if ( game.RedMove && game.PlayingAsRed )
            {
                Debug.Log("Showing UI");
                ShowUI(showWithDelay: true, animate);
            }
            else if (!game.RedMove && !game.PlayingAsRed)
            {
                Debug.Log("Showing UI");
                ShowUI(showWithDelay: true, animate);
            }
            else
            {
                Debug.Log("Cant show shop - not my turn");
                if (animate)
                    HideWithAnimationUI();
                else
                    HideUI();
            }
        }
        else
        {
            if (animate && isShopOpen)
                HideWithAnimationUI();
            else
            {
                HideUI();
            }
            Debug.Log("Shop cant open - no management phase");
        }
    }

    public void FillPlayerHand()
    {
        //fill cards from hand
        if (game.PlayingAsRed)
        {
            //my turn
            int offset = 0;
            Player p = game.RedMove ? 
                (game.GeneralMove ? game.Teams[0].General : game.Teams[0].Governor) : 
                (game.GeneralMove ? game.Teams[1].General : game.Teams[1].Governor);

            if (game.GeneralMove)
            {
                p = game.Teams[0].General;
                Debug.Log("displaying red team (0) general cards");
            }
            else
            {
                p = game.Teams[0].Governor;
                Debug.Log("displaying red team (0) governor cards");
            }
            for (int k = 0; k < p.StandByCharacters.Count; k++)
            {
                offset = AddCardToPlayerHand(p, k, isBuilding: false, offset);
            }
            for (int k = 0; k < p.StandByBuildings.Count; k++)
            {
                offset = AddCardToPlayerHand(p, k, isBuilding: true, offset);
            }
        }
        else
        {
            //my turn
            int offset = 0;

            // cant understand this so I just change variable later
            Player p = !game.RedMove ? 
                // STATEMENT 1
                (game.GeneralMove ?
                game.Teams[1].General : game.Teams[1].Governor) :
                // STATEMENT 2
                (game.GeneralMove ?
                game.Teams[0].General : game.Teams[0].Governor);

            if (game.GeneralMove)
            {
                Debug.Log("displaying blue team (1) general cards");
                p = game.Teams[1].General;
            }
            else
            {
                Debug.Log("displaying blue team (1) governor cards");
                p = game.Teams[1].Governor;
            }

            for (int k = 0; k < p.StandByCharacters.Count; k++)
            {
                offset = AddCardToPlayerHand(p, k, isBuilding: false, offset);
            }
            for (int k = 0; k < p.StandByBuildings.Count; k++)
            {
                offset = AddCardToPlayerHand(p, k, isBuilding: true, offset);
            }
        }
    }

    public int AddCardToPlayerHand(Player p, int cardID, bool isBuilding, int offset)
    {
        GameObject card = Instantiate(CardPrefabForHandCards);
        CardVisual_Simanis cardVisual = card.GetComponent<CardVisual_Simanis>();
        if (isBuilding)
            cardVisual.DisplayBuildCardVisuals(p.StandByBuildings[cardID]);
        else
            cardVisual.DisplayCharacterCardVisuals(p.StandByCharacters[cardID]);
        cardVisual.AddCardToPlayerHand();
        card.transform.SetParent(HandCardHolder.transform);
        card.transform.localScale = Vector3.one;
        card.transform.Translate(offset, 0, 0);
        offset += 120;
        return offset;
    }
    
    public void ClearPlayerHand()
    {
        foreach (Transform child in HandCardHolder.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void ShowUI(bool showWithDelay, bool animate = false)
    {
        isShopOpen = true;
        ClearPlayerHand();

        FillPlayerHand();

        if ( game.GeneralMove )
        {
            //show war units
            shopTitle.text = "WAR DECK";
            CharactersStackTitle.text = "Units";
            BuildingStackTitle.text = "Buildings";
            BuildingsStack.DisplayBuildCardVisuals(game.lastAddedWarBuilding) ;// WarBuildings[0]);
            CharactersStack.DisplayCharacterCardVisuals(game.lastAddedWarCharacter);// WarCharacters[0]);
        }
        else
        {
            //show civil units
            shopTitle.text = "CIVIL DECK";
            CharactersStackTitle.text = "Units";
            BuildingStackTitle.text = "Buildings";
            BuildingsStack.DisplayBuildCardVisuals(game.lastAddedCivilBuilding);//CivilBuildings[0]);
            CharactersStack.DisplayCharacterCardVisuals(game.lastAddedCivilCharacter);//CivilCharacters[0]);
        }

        // show draw button if management phase
        if (game.mRoundState == CenturionGame.RoundState.rsManagement)
        {
            BuyBuilding.gameObject.SetActive(true);
            BuyCharacter.gameObject.SetActive(true);
        }
        // hide draw buttons if it's not management phase
        else
        {
            BuyBuilding.gameObject.SetActive(false);
            BuyCharacter.gameObject.SetActive(false);
        }

        AnimateShopUI(animate, showWithDelay);
        if (!animate)
            Shop.SetActive(true);
        
    }

    private void AnimateShopUI(bool animate, bool showWithDelay)
    {
        if (!animate)
        {
            Shop.transform.position = shopDefaultPos;
            shopBlurBG.color = new Color(0, 0, 0, 0.5f);
        }
        else
        {
            // initialize shop
            Shop.SetActive(true);
            Vector3 targetPos = shopDefaultPos;
            Vector3 startPos = targetPos;
            startPos.y += shopMoveDistance;
            Shop.transform.position = startPos;

            // show animation with delay
            if (showWithDelay)
            {
                DOVirtual.DelayedCall(shopAppearDelay, () =>
                {
                // shop move animation
                Shop.transform.DOMove(targetPos, shopAppearDuration).
                        SetEase(Ease.InOutQuad);

                // shop fade in bg blur
                shopBlurBG.color = new Color(0, 0, 0, 0);
                    shopBlurBG.DOFade(0.5f, shopAppearDuration);
                });
            }
            // show animation without delay
            else
            {
                // shop move animation
                Shop.transform.DOMove(targetPos, shopAppearDuration).
                        SetEase(Ease.InOutQuad);

                // shop fade in bg blur
                shopBlurBG.color = new Color(0, 0, 0, 0);
                shopBlurBG.DOFade(0.5f, shopAppearDuration);
            }
        }
        // enable shop bg
        shopBlurBG.gameObject.SetActive(true);
        shopBlurBG.raycastTarget = true;
    }

    public void HideWithAnimationUI()
    {
     //   Debug.Log("HIDING WITH ANIMATION " + Time.time);
        // disable bg blur
        shopBlurBG.raycastTarget = false;

        // fade out bg blur
        shopBlurBG.DOFade(0, shopAppearDuration);

        // move shop ui
        Vector3 targetPos = shopDefaultPos;
        targetPos.y += shopMoveDistance;
        Shop.transform.DOMove(targetPos, shopAppearDuration).
            SetEase(Ease.InOutQuad).
            OnComplete(()=> HideUI());
    }

    public void HideUI()
    {
       // Debug.Log("HIDING UI " + Time.time);
        Shop.SetActive(false);
        shopBlurBG.gameObject.SetActive(false);
        isShopOpen = false;
    }
}
