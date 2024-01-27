using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUD_Simanis : MonoBehaviour
{
    public CenturionGame centurionGame;

    public TextMeshProUGUI redTeamIdentifier; // team 0
    public TextMeshProUGUI blueTeamIdentifier; // team 1

    public TextMeshProUGUI generalOrGovernorText;
    public string team0String = "-Team 1-";
    public string team1String = "-Team 2-";
    public string govString = "Governor Turn";
    public string generalString = "General Turn";
    public Color generalTurnColor;
    public Color govTurnColor;
    public TextMeshProUGUI teamTurnText;

    public TextMeshProUGUI goldTeam0;
    public TextMeshProUGUI goldTeam1;

    public TextMeshProUGUI vicPointsTeam0;
    public TextMeshProUGUI vicPointsTeam1;

    private void Start()
    {
        //UpdateTeamWealth();
        //UpdateTurnText();
    }

    public void UpdateTeamWealth()
    {
        goldTeam0.text = centurionGame.Teams[0].Gold.ToString();
        goldTeam1.text = centurionGame.Teams[1].Gold.ToString();
    }

    public void UpdateTurnIDs()
    {
        if (centurionGame.PlayingAsRed)
        {
            redTeamIdentifier.text = "Team 1 (you)";
            blueTeamIdentifier.text = "Team 2";
        }
        else // (centurionGame.PlayingAsRed)
        {
            redTeamIdentifier.text = "Team 1";
            blueTeamIdentifier.text = "Team 2 (you)";
        }
    }

    public void UpdateTurnText()
    {
        UpdateTurnIDs();
        // general
        if (centurionGame.GeneralMove)
        {
            generalOrGovernorText.text = generalString;
            generalOrGovernorText.color = generalTurnColor;
        }
        // governor
        else
        {
            generalOrGovernorText.text = govString;
            generalOrGovernorText.color = govTurnColor;
        }
        // team 1
        if (centurionGame.RedMove)
        {
            teamTurnText.text = team0String;
        }
        // Team 2
        else
        {
            teamTurnText.text = team1String;
        }
    }
}
