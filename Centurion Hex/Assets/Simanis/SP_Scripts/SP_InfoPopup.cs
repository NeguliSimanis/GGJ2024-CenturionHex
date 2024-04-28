using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SP_InfoPopup : MonoBehaviour
{
    public TextMeshProUGUI tileTitle;
    public TextMeshProUGUI tileContents;

    public void SetPopupData(SP_Tile tile)
    {
        if (!tile.isDiscovered)
        {
            SetUndiscoveredTileDescription();
            return;
        }

        string tileTitleText = "Empty Tile";
        string emptyTileText = "";
        string unitText = "";

        bool isTileEmpty = false;

        if (tile.myUnit == null && tile.myBuilding==null )
        {
            isTileEmpty = true;
        }
        else if (tile.myUnit != null)
        {
            isTileEmpty = false;
            

            if (tile.myUnit.isAllyUnit)
                unitText += "Ally";
            else
                unitText += "Enemy";

            switch (tile.myUnit.myStats.unitType)
            {
                case SP_UnitType.Scout:
                    unitText += " Scout";
                    unitText += " \n\n Weak but fast unit that dies if it attacks anything";
                    break;
                case SP_UnitType.Legionary:
                    unitText += " Legionary";
                    unitText += " \n\n Slow soldier that counterattacks when in melee range";
                    break;
                case SP_UnitType.Surveyor:
                    unitText += " Surveyor";
                    unitText += " \n\n Fast and fragile civilian unit\n\nCannot attack";
                    break;
                case SP_UnitType.Raptor:
                    unitText += " Raptor";
                    unitText += " \n\n Fast and deadly\n\nGains extra movement if starts turn on desert";
                    break;
                case SP_UnitType.Sniper:
                    unitText += " Sniper";
                    unitText += " \n\n Can attack from 2 tile distance\n\nGains 1 extra damage agains enemies on grasslands";
                    break;
            }
        }


        switch (tile.myEnvironment)
        {
            case SP_TileType.Desert:
                tileTitleText = "Desert Tile";
                emptyTileText = "Scorching sands ripple like golden waves under the sun";
                break;
            case SP_TileType.Grass:
                tileTitleText = "Grass Tile";
                emptyTileText = "Gentle sway of wildflowers beneath azure skies";
                break;
            case SP_TileType.Swamp:
                tileTitleText = "Swamp Tile";
                emptyTileText = "Murky waters and croaking frogs";// whisper secrets amidst twisted roots and tangled vines";
                break;
            case SP_TileType.Empty:
                tileTitleText = "Special Tile";
                break;
            case SP_TileType.Senate:
                tileTitleText = "Senate Tile";
                break;
            case SP_TileType.Forest:
                tileTitleText = "Forest Tile";
                emptyTileText = "A sprawling spruce forest";// spreads its verdant cloak";
                break;
        }

        tileTitle.text = tileTitleText;
        if (isTileEmpty)
        {
            tileContents.text = emptyTileText;
        }
        else
        {
            tileContents.text = unitText;
        }
    }

    private void SetUndiscoveredTileDescription()
    {
        string tileTitleText = "Undiscovered Tile";
        string [] emptyTileTexts = new string[] 
        {
            "What mysteries lie here?",
            "We do no know what is here",
            "A place yet to be explored"
        };

        int emptyRoll = Random.Range(0, emptyTileTexts.Length);


        tileTitle.text = tileTitleText;
        tileContents.text = emptyTileTexts[emptyRoll];
    }
}
