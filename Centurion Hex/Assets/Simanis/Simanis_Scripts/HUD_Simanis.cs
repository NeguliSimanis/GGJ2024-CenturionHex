using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUD_Simanis : MonoBehaviour
{

    public CenturionGame centurionGame;

    public TextMeshProUGUI redTeamIdentifier; // team 0
    public string redTeamIdentifierText = "Ally Team";
    public string redTeamIdentifierTextEnemy = "Enemy Team";
    public TextMeshProUGUI blueTeamIdentifier; // team 1
    public string blueTeamIdentifierText = "Ally Team";
    public string blueTeamIdentifierTextEnemy = "Enemy Team";

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

    [Header("HOVER HIGHLIGHT")]
    public bool processRaycast = true;
    public RaycastInteract oldHighlight;
    public RaycastInteract.Type currSelection = RaycastInteract.Type.Null;

    public RaycastInteract targetHighlight;
    public bool lookingForExtraInteractionTarget = false;
    public RaycastInteract interactionTarget;

    [Header("end turn")]
    public EndTurnButton_Simanis endTurnButton;

    private void Start()
    {
        //UpdateTeamWealth();
        //UpdateTurnText();
    }

    public void UnitMoveComplete()
    {
        processRaycast = true;
    }

    public void UpdateCurHighlight(RaycastInteract raycastInteract)
    {
        if (!processRaycast)
            return;

        // looking for intereaction target once something is already selected
        if (lookingForExtraInteractionTarget && currSelection == RaycastInteract.Type.Character)
        {
            if (interactionTarget != null)
            {
                interactionTarget.SetHighlight(false);
                interactionTarget = null;
            }
            // character trying to interact with a tile
            if (raycastInteract.type == RaycastInteract.Type.Tile)
            {
               
                interactionTarget = raycastInteract;
                raycastInteract.SetHighlight(true);
            }
            return;
        }
        // highlight for first time
        if (oldHighlight != null && !lookingForExtraInteractionTarget)
        {
            if (raycastInteract == oldHighlight)
                return;
            oldHighlight.SetHighlight(false);
        }
        oldHighlight = raycastInteract;
        raycastInteract.SetHighlight(true);
        currSelection = raycastInteract.type;
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
            redTeamIdentifier.text = redTeamIdentifierText;
            blueTeamIdentifier.text = blueTeamIdentifierTextEnemy;
        }
        else // (centurionGame.PlayingAsRed)
        {
            redTeamIdentifier.text = redTeamIdentifierTextEnemy;
            blueTeamIdentifier.text = blueTeamIdentifierText;
        }
    }

    private void TryToMoveToTile()
    {
        Debug.Log("try to move to ");

        // char id  oldHighlight.type == RaycastInteract.Type.Character
        uint id = oldHighlight.characterVisualControl.character.id;

        //  target x y interactionTarget.type == RaycastInteract.Type.Tile
        int xPos = interactionTarget.tileVisualControl.xCoord;
        int yPos = interactionTarget.tileVisualControl.yCoord;

        Network.instance.MoveCharacter(characterId: id,
            x: xPos, y: yPos);
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (oldHighlight != null)
            {
                if (lookingForExtraInteractionTarget && interactionTarget)
                {
                    if (interactionTarget.type == RaycastInteract.Type.Tile
                        && oldHighlight.type == RaycastInteract.Type.Character)
                    {
                        lookingForExtraInteractionTarget = false;
                        processRaycast = false;
                        TryToMoveToTile();
                    }
                    return;
                }
                lookingForExtraInteractionTarget = true;
            }
        }
    }

    public void MoveCharacter()
    {
        Debug.Log("its alive");
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
            if (centurionGame.PlayingAsRed)
            {
                endTurnButton.gameObject.SetActive(true);
                endTurnButton.endButtonTeam0.SetActive(true);
                endTurnButton.endButtonTeam1.SetActive(false);
                teamTurnText.text = redTeamIdentifierText;
            }
            else
            {
                teamTurnText.text = redTeamIdentifierTextEnemy;

                endTurnButton.gameObject.SetActive(false);
            }
            //teamTurnText.text = team0String;
        }
        // Team 2
        else
        {
            // im playing as bl
            if (!centurionGame.PlayingAsRed)
            {
                endTurnButton.gameObject.SetActive(true);
                endTurnButton.endButtonTeam1.SetActive(true);
                endTurnButton.endButtonTeam0.SetActive(false);
                teamTurnText.text = blueTeamIdentifierText;
            }
            else
            {
                endTurnButton.gameObject.SetActive(false);
                teamTurnText.text = blueTeamIdentifierTextEnemy;
            }
        }
    }
}
