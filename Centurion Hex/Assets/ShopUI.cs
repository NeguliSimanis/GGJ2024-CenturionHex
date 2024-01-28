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
            if ( game.RedMove == game.PlayingAsRed )
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

    public void ShowUI()
    {
        foreach (Transform child in HandCardHolder.transform)
        {
            Destroy(child.gameObject);
        }

        //fill cards from hand
        if( game.RedMove == game.PlayingAsRed )
        {
            //my turn
            int offset = 0;
            Player p = game.RedMove ? (game.GeneralMove ? game.Teams[0].General : game.Teams[0].Governor) : (game.GeneralMove ? game.Teams[1].General : game.Teams[1].Governor);
            for( int k = 0; k < p.StandByCharacters.Count; k++ )
            {
                GameObject card = Instantiate(CardPrefabForHandCards);
                card.GetComponent<CardVisual_Simanis>().DisplayCharacterCardVisuals(p.StandByCharacters[k]);
                card.transform.SetParent(HandCardHolder.transform);
                card.transform.Translate(offset, 0, 0);
                offset += 120;
            }
            for (int k = 0; k < p.StandByBuildings.Count; k++)
            {
                GameObject card = Instantiate(CardPrefabForHandCards);
                card.GetComponent<CardVisual_Simanis>().DisplayBuildCardVisuals(p.StandByBuildings[k]);
                card.transform.SetParent(HandCardHolder.transform);
                card.transform.Translate(offset, 0, 0);
                offset += 120;
            }
        }

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
