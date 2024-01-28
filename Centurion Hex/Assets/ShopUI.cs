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

    // Start is called before the first frame update
    void Start()
    {
        Shop.SetActive(false);
        BuyBuilding.onClick.AddListener(() =>
        {
            Network.instance.BuyBuilding();
        });
        BuyCharacter.onClick.AddListener(() =>
        {
            Network.instance.BuyCharacter();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MaybeOpen()
    {
        if( game.mRoundState == CenturionGame.RoundState.rsManagement )
        {
            if ( game.RedMove == game.PlayingAsRed )
                        ShowUI();
        }
    }

    public void ShowUI()
    {
        Shop.SetActive(true);
        if( game.GeneralMove )
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
    }

    public void HideUI()
    {
        Shop.SetActive(false);
    }
}
