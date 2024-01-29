using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public GameObject Shop;
    public TextMeshProUGUI BuildingStackTitle;
    public CardVisual_Simanis BuildingsStack;
    public TextMeshProUGUI CharactersStackTitle;
    public CardVisual_Simanis CharactersStack;
    public CenturionGame game;
    public Button BuyBuilding;
    public Button BuyCharacter;
    public GameObject CardPrefabForHandCards;
    public GameObject HandCardHolder;

    // Start is called before the first frame update
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

    public void MaybeOpen()
    {
        Debug.Log("Checking shop " + game.mRoundState);
        if( game.mRoundState == CenturionGame.RoundState.rsManagement )
        {
            if ( game.RedMove && game.PlayingAsRed )
            {
                Debug.Log("Showing UI");
                ShowUI();
            }
            else if (!game.RedMove && !game.PlayingAsRed)
            {
                Debug.Log("Showing UI");
                ShowUI();
            }
            else
            {
                Debug.Log("Cant show shop - not my turn");
                HideUI();
            }
        }
        else
        {
            HideUI();
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
        card.transform.Translate(offset, 0, 0);
        offset += 120;
        return offset;
    }

    public void ShowUI()
    {
        foreach (Transform child in HandCardHolder.transform)
        {
            Destroy(child.gameObject);
        }

        FillPlayerHand();

        if ( game.GeneralMove )
        {
            //show war units
            CharactersStackTitle.text = "War units";
            CharactersStackTitle.color = Color.red;
            BuildingStackTitle.text = "War buildings";
            BuildingStackTitle.color = Color.red; 
            BuildingsStack.DisplayBuildCardVisuals(game.WarBuildings[0]);
            CharactersStack.DisplayCharacterCardVisuals(game.WarCharacters[0]);
        }
        else
        {
            //show civil units
            CharactersStackTitle.text = "Civil units";
            CharactersStackTitle.color = Color.blue;
            BuildingStackTitle.text = "Civil buildings";
            BuildingStackTitle.color = Color.blue;
            BuildingsStack.DisplayBuildCardVisuals(game.CivilBuildings[0]);
            CharactersStack.DisplayCharacterCardVisuals(game.CivilCharacters[0]);
        }
        Shop.SetActive(true);
    }

    public void HideUI()
    {
        Shop.SetActive(false);
    }
}
