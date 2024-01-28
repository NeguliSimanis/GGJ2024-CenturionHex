using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public string movePhaseSuffix = "- Movement phase";
    public string managementPhaseSuffix = "- Management phase";
    public Color generalTurnColor;
    public Color govTurnColor;
    public TextMeshProUGUI teamTurnText;

    public TextMeshProUGUI goldTeam0;
    public TextMeshProUGUI goldTeam1;

    public TextMeshProUGUI vicPointsTeam0;
    public TextMeshProUGUI vicPointsTeam1;

    [Header("HOVER HIGHLIGHT")]
    private CustomCursor_Simanis customCursor;
    public bool processRaycast = true;
    public RaycastInteract oldHighlight;
    public RaycastInteract.Type currSelection = RaycastInteract.Type.Null;

    public RaycastInteract targetHighlight;
    public bool lookingForExtraInteractionTarget = false;
    public RaycastInteract interactionTarget;

    [Header("end turn")]
    public EndTurnButton_Simanis endTurnButton;

    public float listenForMoveSuccessDuration = 0.03f;
    public bool isListeningToMoveSuccess = false;
    private float moveStartTime;

    private void Start()
    {
        //UpdateTeamWealth();
        //UpdateTurnText();
        customCursor = GetComponent<CustomCursor_Simanis>();
    }

    public void ListenToRaycast()
    {
        processRaycast = true;
    }

    public void UpdateCurHighlight(RaycastInteract raycastInteract)
    {
        // only highlight stuff if its ur turn
        if (centurionGame.RedMove && !centurionGame.PlayingAsRed)
        {
            ClearHighlights();
            return;
        }
        if (!centurionGame.RedMove && centurionGame.PlayingAsRed)
        {
            ClearHighlights();
            return;
        }

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
                customCursor.SetCursor(true, CursorAction.walk);
            }
            // ATTACKING
            // check if u have any attack
            if (oldHighlight.characterVisualControl.character.AttackDamage < 1)
            {
                customCursor.SetCursor(true, CursorAction.error);
                return;
            }
            // try to interact with another character
            if (raycastInteract.type == RaycastInteract.Type.Character)
            {
                // check if is enemy
                if (!raycastInteract.characterVisualControl.IsMyUnit())
                {
                    interactionTarget = raycastInteract;
                    raycastInteract.SetHighlight(true);
                    customCursor.SetCursor(true, CursorAction.attack);
                }
            }
            // try to interact with another building
            if (raycastInteract.type == RaycastInteract.Type.Building)
            {
                // check if is enemy
                if (!raycastInteract.buildingVisualControl.IsMyBuilding())
                {
                    Debug.Log("todo attack building");
                    interactionTarget = raycastInteract;
                    raycastInteract.SetHighlight(true);
                    customCursor.SetCursor(true, CursorAction.attack);
                }
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
        customCursor.SetCursor(false, CursorAction.walk);
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

    private void TryToAttackTile(bool attackChar)
    {
        Debug.Log("try to attack tile");

        // char id  oldHighlight.type == RaycastInteract.Type.Character
        uint id = 0;
        if (oldHighlight)
            id = oldHighlight.characterVisualControl.character.id;
        else
            ClearHighlights();

        //  target x y interactionTarget.type == RaycastInteract.Type.Tile
        int xPos;
        int yPos;
        if (attackChar)
        {
            xPos = interactionTarget.characterVisualControl.xCoord;
            yPos = interactionTarget.characterVisualControl.yCoord;
        }
        else
        {
            xPos = interactionTarget.buildingVisualControl.xCoord;
            yPos = interactionTarget.buildingVisualControl.yCoord;
        }

        Network.instance.HurtTile(id, xPos, yPos);
    }

    private void TryToMoveToTile()
    {
        Debug.Log("try to move to ");

        // char id  oldHighlight.type == RaycastInteract.Type.Character
        uint id = 0;
        if (oldHighlight)
            id = oldHighlight.characterVisualControl.character.id;
        else
            ClearHighlights();

        //  target x y interactionTarget.type == RaycastInteract.Type.Tile
        int xPos = interactionTarget.tileVisualControl.xCoord;
        int yPos = interactionTarget.tileVisualControl.yCoord;

        Network.instance.MoveCharacter(characterId: id,
            x: xPos, y: yPos);
    }

    public void ClearHighlights()
    {
        lookingForExtraInteractionTarget = false;
        currSelection = RaycastInteract.Type.Null;
        if (interactionTarget)
        {
            interactionTarget.SetHighlight(false);
            interactionTarget = null;
        }
        if (oldHighlight)
        {
            oldHighlight.SetHighlight(false);
            oldHighlight = null;
        }
        ListenToRaycast();
    }

    public void ReloadScene()
    {
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Load the active scene again
        SceneManager.LoadScene(activeSceneIndex);
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ClearHighlights();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            // Call the function to reload the active scene
            ReloadScene();
        }

        if (isListeningToMoveSuccess)
        {
            if (Time.time > listenForMoveSuccessDuration + moveStartTime)
            {
                isListeningToMoveSuccess = false;
                ListenToRaycast();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (oldHighlight != null)
            {
                if (lookingForExtraInteractionTarget && interactionTarget)
                {
                    // TRY MOVE
                    if (interactionTarget.type == RaycastInteract.Type.Tile
                        && oldHighlight.type == RaycastInteract.Type.Character)
                    {
                        lookingForExtraInteractionTarget = false;
                        processRaycast = false;
                        TryToMoveToTile();
                    }

                    // TRY ATTACK CHAR
                    if (interactionTarget.type == RaycastInteract.Type.Character
                        && oldHighlight.type == RaycastInteract.Type.Character)
                    {
                        lookingForExtraInteractionTarget = false;
                        processRaycast = false;
                        TryToAttackTile(attackChar: true);
                    }

                    // TRY ATTACK BUILD
                    if (interactionTarget.type == RaycastInteract.Type.Building
                        && oldHighlight.type == RaycastInteract.Type.Character)
                    {
                        lookingForExtraInteractionTarget = false;
                        processRaycast = false;
                        TryToAttackTile(attackChar: false);
                    }
                    return;
                }
                lookingForExtraInteractionTarget = true;
                moveStartTime = Time.time;
                isListeningToMoveSuccess = true;
            }
        }
    }

    public void MoveCharacter()
    {
        Debug.Log("its alive");
    }

    public void UpdateTurnText()
    {
        Debug.Log("Updating Turn phase text");
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
            // im playing as blue
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
        switch (centurionGame.mRoundState)
        {
            case CenturionGame.RoundState.rsManagement:
                teamTurnText.text += managementPhaseSuffix;
                break;
            default:
                teamTurnText.text += movePhaseSuffix;
                break;
        }
    }
}
