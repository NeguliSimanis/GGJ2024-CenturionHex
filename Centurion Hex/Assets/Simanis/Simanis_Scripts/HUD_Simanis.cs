using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUD_Simanis : MonoBehaviour
{

    public CenturionGame centurionGame;
    public static HUD_Simanis instance;
    [HideInInspector]
    public AnnouncementUi_Simanis announcement_UI;

    public TextMeshProUGUI allyTeamIdentifier; 
    public TextMeshProUGUI enemyTeamIdentifier;

    public TextMeshProUGUI generalOrGovernorText;
    public Color generalTurnColor;
    public Color govTurnColor;
    public TextMeshProUGUI teamTurnText;

    public TextMeshProUGUI goldAllyTeam;
    public TextMeshProUGUI goldEnemyTeam;

    public TextMeshProUGUI vicPointsAlly;
    public TextMeshProUGUI vicPointsEnemy;

    public TextMeshProUGUI WinnerText;
    public GameObject WinnerUI;

    // GAME CURSOR
    ///// <summary>
    ///// During enemy turn only default cursor available
    ///// </summary>
    //[HideInInspector]
    //public bool showOnlyDefaultCursor;
    private CustomCursor_Simanis customCursor;

    [Header("HOVER HIGHLIGHT")]
    public bool processRaycast = true;
    public RaycastInteract oldHighlight;
    public RaycastInteract.Type currSelection = RaycastInteract.Type.Null;

    public RaycastInteract targetHighlight;
    public bool lookingForExtraInteractionTarget = false;
    public RaycastInteract interactionTarget;

    [Header("end turn")]
    public EndTurnButton_Simanis endTurnButton;

    [Header("MOVE INPUT PROCESSING")]
    public float listenForMoveSuccessDuration = 0.03f;
    public bool isListeningToMoveSuccess = false;
    private float moveStartTime;

    [Header("PLACE CARDS PROCESSING")]
    public bool cardPlacementInputAllowed = false;
    public GameObject cardPrefabBeingPlayed;
    public Character characterBeingPlaced;
    public Building buildingBeingPlaced;
    public bool tryingToPlaceBuilding = false;
    public RaycastInteract cardPlacementTarget;
    public bool canPlaceCardThere = false;
    public List<TileVisual_Simanis> allowedCardPlacementTiles = new List<TileVisual_Simanis>();

    [Header("CARD HAND/DECK HIGHLIGHTS")]
    ///// <summary>
    ///// the card that has been last clicked
    ///// </summary>
    public CardVisual_Simanis highlightedCardVisual;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //UpdateTeamWealth();
        //UpdateTurnText();
        customCursor = GetComponent<CustomCursor_Simanis>();
        announcement_UI = AnnouncementUi_Simanis.instance;
        HideGameFinished();
        SetTeamIdentifierTexts();
    }

    private void SetTeamIdentifierTexts()
    {
        enemyTeamIdentifier.text = "Enemy";
        allyTeamIdentifier.text = "Ally";
    }

    public void ListenToRaycast()
    {
        processRaycast = true;
    }

    public bool IsMyTurn()
    {
        bool isMy = false;
        if (centurionGame.RedMove && centurionGame.PlayingAsRed)
        {
            isMy = true;
        }
        if (!centurionGame.RedMove && !centurionGame.PlayingAsRed)
        {
            isMy = true;
        }
        return isMy;
    }

    public bool IsManagementPhase()
    {
        bool isManagePhase = false;
        if (centurionGame.mRoundState == CenturionGame.RoundState.rsManagement)
            isManagePhase = true;
        return isManagePhase;
   }

    public void RemoveCustomCursor()
    {
        if (customCursor == null)
            customCursor = gameObject.GetComponent<CustomCursor_Simanis>();

        customCursor.SetCursor(false, cursorAction: CursorAction.undefined);
    }

    public void ShowAllowedBuildPlacementTiles(GameObject card, Building buildBeingPlaced)
    {
        allowedCardPlacementTiles.Clear();
        Debug.Log("show allowed building placement tiles");
        buildingBeingPlaced = buildBeingPlaced;
        cardPrefabBeingPlayed = card;

        // can build next to own buildings
        if (buildBeingPlaced.requireNextToAlly)
        {
            // get a list of own buildings
            List<BuildingVisual_Simanis> allBuildingVisuals = TileSpawner_Simanis.instance.allBuildings;
            List<BuildingVisual_Simanis> myBuildingVisuals = new List<BuildingVisual_Simanis>();
            foreach (BuildingVisual_Simanis buildingVisual in allBuildingVisuals)
            {
                Building curBuilding = buildingVisual.building;

                if (centurionGame.PlayingAsRed && curBuilding.Team == centurionGame.Teams[0])
                    myBuildingVisuals.Add(buildingVisual);
                if (!centurionGame.PlayingAsRed && curBuilding.Team == centurionGame.Teams[1])
                    myBuildingVisuals.Add(buildingVisual);
            }

            
            // get a list of buildable tiles adjacent to own buildings
            foreach (BuildingVisual_Simanis buildingVisual in myBuildingVisuals)
            {
                Tile[] allowedTiles = CenturionGame.Instance.GetTilesAdjacentToBuilding(
                buildingVisual.building.id,
                onlyBuildableTiles: true);

                // highlight adjacent empty tiles - next to own buildings
                foreach (Tile allowedTile in allowedTiles)
                {
                    if (allowedTile != null )
                    {
                        TileVisual_Simanis newAllowedTile = TileSpawner_Simanis.instance.GetTileVisual(allowedTile);
                        allowedCardPlacementTiles.Add(newAllowedTile);
                        newAllowedTile.HighlightTile(true);
                    }
                }
            }
        }
        // can be built anywhere
        else if (buildBeingPlaced.requiredTileType == TileCover.CoverType.ctUndefined)
        {
            // can be built anywhere - get a list of buildable tiles 
            Tile[,] allTiles = CenturionGame.Instance.Board.Tiles;

            // can be built anywhere - highlight empty tiles
            foreach (Tile currTile in allTiles)
            {
                if (currTile != null &&
                currTile.currentBuilding == null &&
                currTile.currentCharacter == null &&
                currTile.tileCover.Type != TileCover.CoverType.ctUndefined &&
                currTile.tileType == Tile.TileType.ttBuildable)
                {
                    TileVisual_Simanis newAllowedTile = TileSpawner_Simanis.instance.GetTileVisual(currTile);
                    allowedCardPlacementTiles.Add(newAllowedTile);
                    newAllowedTile.HighlightTile(true);
                }
            }

        }
        // can be built FOREST
        else if (buildBeingPlaced.requiredTileType == TileCover.CoverType.ctForest)
        {
            // FOREST - get a list of buildable tiles 
            Tile[,] allTiles = CenturionGame.Instance.Board.Tiles;

            // FOREST - highlight empty tiles
            foreach (Tile currTile in allTiles)
            {
                if (currTile != null &&
                currTile.currentBuilding == null &&
                currTile.currentCharacter == null &&
                currTile.tileType == Tile.TileType.ttBuildable &&
                currTile.tileCover.Type == TileCover.CoverType.ctForest)
                {
                    TileVisual_Simanis newAllowedTile = TileSpawner_Simanis.instance.GetTileVisual(currTile);
                    allowedCardPlacementTiles.Add(newAllowedTile);
                    newAllowedTile.HighlightTile(true);
                }
            }
        }
        // can be built GRASS
        else if (buildBeingPlaced.requiredTileType == TileCover.CoverType.ctGrass)
        {
            // FOREST - get a list of buildable tiles 
            Tile[,] allTiles = CenturionGame.Instance.Board.Tiles;

            // FOREST - highlight empty tiles
            foreach (Tile currTile in allTiles)
            {
                if (currTile != null &&
                currTile.currentBuilding == null &&
                currTile.currentCharacter == null &&
                currTile.tileType == Tile.TileType.ttBuildable &&
                currTile.tileCover.Type == TileCover.CoverType.ctGrass)
                {
                    TileVisual_Simanis newAllowedTile = TileSpawner_Simanis.instance.GetTileVisual(currTile);
                    allowedCardPlacementTiles.Add(newAllowedTile);
                    newAllowedTile.HighlightTile(true);
                }
            }

        }
        // unknown error
        else
        {
            Debug.LogError("Don't know where building can be placed");
        }
        if (allowedCardPlacementTiles.Count > 0)
        {
            tryingToPlaceBuilding = true;
            cardPlacementInputAllowed = true;
        }
    }

    public void ShowAllowedCharPlacementTiles(GameObject card, Character charBeingPlaced)
    {
        characterBeingPlaced = charBeingPlaced;
        cardPrefabBeingPlayed = card;
        Debug.Log("Showing tiles where character can be placed on board");

        // find MY senate
        BuildingVisual_Simanis mySenate = null;
        foreach (BuildingVisual_Simanis buildingVisual in TileSpawner_Simanis.instance.allBuildings)
        {
            Building senate;
            if (buildingVisual.building.Type == Building.BuildingType.btSenate)
            {
                senate = buildingVisual.building;

                if (centurionGame.PlayingAsRed && senate.Team == centurionGame.Teams[0])
                    mySenate = buildingVisual;
                if (!centurionGame.PlayingAsRed && senate.Team == centurionGame.Teams[1])
                    mySenate = buildingVisual;
            }
        }

        if (mySenate == null)
        {
            Debug.LogError("couldnt find your senat");
            return;
        }

        // find adjacent empty tiles
        Tile[] allowedTiles = CenturionGame.Instance.GetTilesAdjacentToBuilding(
            mySenate.building.id);

        // highlight adjacent empty tiles
        foreach(Tile allowedTile in allowedTiles)
        {
            if (allowedTile != null)
            {
                TileVisual_Simanis newAllowedTile = TileSpawner_Simanis.instance.GetTileVisual(allowedTile);
                allowedCardPlacementTiles.Add(newAllowedTile);
                newAllowedTile.HighlightTile(true);
            }
        }

        cardPlacementInputAllowed = true;
    }

    public void UpdateManagementPhaseHighlights(RaycastInteract raycastInteract)
    {
        if (!cardPlacementInputAllowed)
            return;

        // Placing cards - check if placing on tile
        if (raycastInteract.type == RaycastInteract.Type.Tile)
        {
            cardPlacementTarget = raycastInteract;   
        }
        else
        {
            cardPlacementTarget = null;
        }
    }

    public void UpdateMovementPhaseHighlight(RaycastInteract raycastInteract)
    {
        // only highlight stuff if its ur turn
        if (!IsMyTurn())
        {
            ClearHighlights();
            RemoveCustomCursor();
            return;
        }

        if (IsManagementPhase())
            return;

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
                // check if character has movement remaining 
                bool hasMovementRemaining = false;
                if (oldHighlight != null)
                {
                   // Debug.Log("OLD HIGHLIGHT EXISTS: " + oldHighlight);
                    if (oldHighlight.characterVisualControl.character.RemainingStepsThisTurn() > 0)
                        hasMovementRemaining = true;
                }
                // 
                if (hasMovementRemaining)
                {
                    interactionTarget = raycastInteract;
                    raycastInteract.SetHighlight(true);
                    customCursor.SetCursor(true, CursorAction.walk);
                }
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
        raycastInteract.SetHighlight(true, highlightType: RaycastInteract.HighlightType.MoveUnit);
        currSelection = raycastInteract.type;
    }

    public void UpdateTeamWealth()
    {
        if (centurionGame.PlayingAsRed)
        {
            goldAllyTeam.text = centurionGame.Teams[0].Gold.ToString();
            goldEnemyTeam.text = centurionGame.Teams[1].Gold.ToString();
        }
        else
        {
            goldAllyTeam.text = centurionGame.Teams[1].Gold.ToString();
            goldEnemyTeam.text = centurionGame.Teams[0].Gold.ToString();
        }
    }
    public void UpdateTeamVictoryPoints()
    {
        if (centurionGame.PlayingAsRed)
        {
            vicPointsAlly.text = centurionGame.Teams[0].VictoryPoints.ToString();
            vicPointsEnemy.text = centurionGame.Teams[1].VictoryPoints.ToString();
        }
        else
        {
            vicPointsAlly.text = centurionGame.Teams[1].VictoryPoints.ToString();
            vicPointsEnemy.text = centurionGame.Teams[0].VictoryPoints.ToString();
        }
    }

    public void HideGameFinished()
    {
        WinnerUI.SetActive(false);
    }

    public void UpdateOnlineOffline()
    {
        Debug.Log("Some one went online or offline - update UI accordingly");
    }

    public void ShowGameFinished()
    {
        WinnerText.text = "Winning team is " + ( centurionGame.WinnerTeam == Team.TeamType.ttRed ? "Red" : "Blue" ) + " team";
        WinnerUI.SetActive(true);
    }

    public void UpdateTurnIDs()
    {
        if (centurionGame.PlayingAsRed)
        {
            //redTeamIdentifier.text = redTeamIdentifierText;
            //blueTeamIdentifier.text = blueTeamIdentifierTextEnemy;
        }
        else // (centurionGame.PlayingAsRed)
        {
            //redTeamIdentifier.text = redTeamIdentifierTextEnemy;
            //blueTeamIdentifier.text = blueTeamIdentifierText;
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

        if (CenturionGame.Instance.UseNetwork)
            Network.instance.HurtTile(id, xPos, yPos);
        else
            FakeNetwork_Simanis.Instance.SP_HurtTile(id, xPos, yPos);
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

        if (CenturionGame.Instance.UseNetwork)
        {
            Network.instance.MoveCharacter(
                characterId: id,
                x: xPos, 
                y: yPos);
        }
        else
        {
            FakeNetwork_Simanis.Instance.SP_MoveCharacter(
                characterId: id,
                x: xPos,
                y: yPos);
        }
    }

    public void TryToPlaceUnitOnTile()
    {
        
        Debug.Log("try to place char on tile");
        Network.instance.PlaceCharacter(characterBeingPlaced.id, 
            x: cardPlacementTarget.tileVisualControl.xCoord,
            y: cardPlacementTarget.tileVisualControl.yCoord);
        //ClearHighlights();
    }

    public void TryToPlaceBuildingOnTile()
    {
        //
        Debug.Log("try to place build on tile");
        Network.instance.PlaceBuilding(buildingBeingPlaced.id,
            x: cardPlacementTarget.tileVisualControl.xCoord,
            y: cardPlacementTarget.tileVisualControl.yCoord);
       // ClearHighlights();
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
        foreach (TileVisual_Simanis tileVisual in TileSpawner_Simanis.instance.allTiles)
        {
            tileVisual.HighlightTile(false);
        }
        allowedCardPlacementTiles.Clear();
        cardPlacementInputAllowed = false;
        characterBeingPlaced = null;
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
            // PLACING CARDS ON BOARD
            if(cardPlacementInputAllowed && cardPlacementTarget)
            {
                foreach(TileVisual_Simanis tileVisual in allowedCardPlacementTiles)
                {
                    if (tileVisual == cardPlacementTarget.tileVisualControl)
                    {
                        Debug.Log("target for card placement found");
                        if (!tryingToPlaceBuilding)
                            TryToPlaceUnitOnTile();
                        else
                            TryToPlaceBuildingOnTile();
                    }
                }
            }

            // UNIT MOVING/ATTACK LOGIC
            if (oldHighlight != null)
            {
                if (lookingForExtraInteractionTarget && interactionTarget)
                {
                    // TRY MOVE
                    if (interactionTarget.type == RaycastInteract.Type.Tile
                        && oldHighlight.type == RaycastInteract.Type.Character)
                    {
                        processRaycast = false;
                        TryToMoveToTile();
                    }

                    // TRY ATTACK CHAR
                    if (interactionTarget.type == RaycastInteract.Type.Character
                        && oldHighlight.type == RaycastInteract.Type.Character)
                    {
                        processRaycast = false;
                        TryToAttackTile(attackChar: true);
                    }

                    // TRY ATTACK BUILD
                    if (interactionTarget.type == RaycastInteract.Type.Building
                        && oldHighlight.type == RaycastInteract.Type.Character)
                    {
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

    /// <summary>
    /// Called by CenturionGame.cs event in editor
    /// </summary>
    public void HandleRoundStateChange()
    {
        ClearHighlights();
        UpdateTurnText();
        RemoveCustomCursor();
        ColorGreyInactiveUnits();
        ResetUnitSpeedDisplay();
    }

    private void ResetUnitSpeedDisplay()
    {
        Debug.Log("unit speed reset");
        TileSpawner_Simanis.instance.UpdateAllCharacterSpeedUI();
    }

    /// <summary>
    /// Colors war units grey during governor turn and vice versa
    /// </summary>
    public void ColorGreyInactiveUnits()
    {
        TileSpawner_Simanis.instance.MarkInactiveUnits();
    }

    public void UpdateTurnText()
    {
        //Debug.Log("Updating Turn phase text");
        UpdateTurnIDs();

        /*
         * So the turn can be:
         *  - enemy or ally:
         *      > teamTurnText - dunno where this is
         *  - governor or general:
         *      > generalOrGovernorText - big text in corner of screen
         *  - movement or management
         *      > teamTurnText - suffix to that
         */
        string bigAnnounceString = "turn";
        string smallAnnounceString = " war/civil x phase";

        // general
        if (centurionGame.GeneralMove)
        {
            generalOrGovernorText.text = "General Turn";
            smallAnnounceString = "War ";
           // generalOrGovernorText.color = generalTurnColor;
        }
        // governor
        else
        {
            generalOrGovernorText.text = "Governor Turn";
            smallAnnounceString = "Civil ";
            //generalOrGovernorText.color = govTurnColor;
        }

        // TEAM 1
        if (centurionGame.RedMove)
        {
            if (centurionGame.PlayingAsRed)
            {
                endTurnButton.gameObject.SetActive(true);
                endTurnButton.endButtonTeam0.SetActive(true);
                endTurnButton.endButtonTeam1.SetActive(false);
                teamTurnText.text = "Ally Team";
                bigAnnounceString = "Ally Turn";
            }
            else
            {
                teamTurnText.text = "Enemy Team";
                bigAnnounceString = "Enemy Turn";
                endTurnButton.gameObject.SetActive(false);
            }
        }
        // Team 2
        else
        {
            // im playing as blue and is blue turn
            if (!centurionGame.PlayingAsRed)
            {
                endTurnButton.gameObject.SetActive(true);
                endTurnButton.endButtonTeam1.SetActive(true);
                endTurnButton.endButtonTeam0.SetActive(false);
                teamTurnText.text = "Ally Team";
                bigAnnounceString = "Ally Turn";
            } 
            // playing red and is blue turn
            else
            {
                endTurnButton.gameObject.SetActive(false);
                teamTurnText.text = "Enemy Team";
                bigAnnounceString = "Enemy Turn";
            }
        }
        switch (centurionGame.mRoundState)
        {
            case CenturionGame.RoundState.rsManagement:
                teamTurnText.text += " - Management phase";
                smallAnnounceString += "management phase";
                break;
            default:
                teamTurnText.text += " - Movement phase";
                smallAnnounceString += "movement phase";
                break;
        }
        if (announcement_UI != null)
            announcement_UI.ShowAnnouncmentText(
                bigAnnounce: bigAnnounceString,
                smallAnnounce: smallAnnounceString,
                appearDuration: 0.8f,
                disappearDelay: 1.8f,
                disappearDuration: 1f);
    }
}
