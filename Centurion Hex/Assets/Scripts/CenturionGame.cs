using Assets.Scripts.Buildings;
using Assets.Scripts.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static CenturionGame;
using static Character;
using static Building;

public class CenturionGame : MonoBehaviour
{
    public enum RoundState
    {
        rsNone,
        rsGeneratingWealth,
        rsMovingCharacters,
        rsManagement,
    };

    [HideInInspector] public RoundState mRoundState;
    [HideInInspector] public int placementPriceMultiplier = 1;
    [HideInInspector] public int free_unit_cards = 0;
    public static CenturionGame Instance { get; private set; }
    [HideInInspector] public Team.TeamType WinnerTeam;

    [Header("COLORS")]
    public Color team0Color = Color.red;
    public Color team1Color = Color.blue;

    [Header("EVENTS")]
    public UnityEvent onGameReload;
    public UnityEvent onRoundStateChange;
    public UnityEvent onWealthFromBuilding;
    public UnityEvent onWealthFromCharacter;
    public UnityEvent onCharacterMoved;
    public UnityEvent onTileCovered;
    public UnityEvent onWealthFromTile;
    public UnityEvent onPointsFromTile;
    public UnityEvent onPointsFromBuilding;
    public UnityEvent onCharacterHurt;
    public UnityEvent onBuildingHurt;
    public UnityEvent onCharacterBought;
    public UnityEvent onBuildingBought;
    public UnityEvent onUpdateGold;
    public UnityEvent onUpdatePoints;
    public UnityEvent onPlaceCharacter;
    public UnityEvent onPlaceBuilding;
    public UnityEvent onGameFinished;
    public UnityEvent onOnlineOffline;
    public UnityEvent onStackUpdateBuilding;
    public UnityEvent onStackUpdateCharacter;
    public UnityEvent onUnitGotAdditionalSteps;
    public UnityEvent onDoMadman;
    public UnityEvent onSwapSides;

    [HideInInspector] public Board Board = new Board();

    //all characters in deck
    [HideInInspector] public List<Character> Characters = new List<Character>();

    //shortcuts
    [HideInInspector] public List<Character> WarCharacters = new List<Character>();
    [HideInInspector] public Character lastAddedWarCharacter;
    [HideInInspector] public List<Character> CivilCharacters = new List<Character>();
    [HideInInspector] public Character lastAddedCivilCharacter;
    [HideInInspector] public List<Character> BoardCharacters = new List<Character>();
    [HideInInspector] public Character lastSwappedChar;

    //all buildings in deck
    [HideInInspector] public List<Building> Buildings = new List<Building>();

    //shortcuts
    [HideInInspector] public List<Building> WarBuildings = new List<Building>();
    [HideInInspector] public List<Building> CivilBuildings = new List<Building>();
    [HideInInspector] public Building lastAddedCivilBuilding;
    [HideInInspector] public List<Building> BoardBuildings = new List<Building>();
    [HideInInspector] public Building lastAddedWarBuilding;
    [HideInInspector] public uint lastAdditionalSteps = 0;

    private void Update()
    {
#if UNITY_EDITOR
        // Code here will only execute when running in the Unity Editor
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DebugEndTurn();
        }
        

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Network.instance.DebugRestart();
        }
        return;
#endif
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndTurn();
        }
    }

    public void DebugEndTurn()
    {
        if (UseNetwork)
            Network.instance.DebugEndMove();
        else
        {
            FakeNetwork_Simanis.Instance.SP_EndTurnPhase();
            Debug.Log("TODO: SINGLE PLAYER END TURN");
        }
    }

    public void EndTurn()
    {
        if (UseNetwork)
            Network.instance.EndMove();
        else
        {
            FakeNetwork_Simanis.Instance.SP_EndTurnPhase();
            Debug.Log("TODO SINGLE PLAYER END TURN");
        }
    }

    public Building GetBoardBuilding(uint id)
    {
        for (int k = 0; k < BoardBuildings.Count; k++)
        {
            if (BoardBuildings[k].id == id)
                return BoardBuildings[k];
        }
        return null;
    }

    public Building GetBuilding(uint id)
    {
        for (int k = 0; k < Buildings.Count; k++)
        {
            if (Buildings[k].id == id)
                return Buildings[k];
        }
        return null;
    }

    public Tile[] GetTilesAdjacentToBuilding(uint buildingID,
        bool tilesWithoutUnits = true,
        bool onlyDiscoveredTiles = true,
        bool onlyBuildableTiles = false)
    {
        if (!tilesWithoutUnits)
        {
            Debug.LogError("functionality not implemenmted");
            return null;
        }

        if (!onlyDiscoveredTiles)
        {
            Debug.LogError("functionality not implemenmted");
            return null;
        }

        Building building = null;

        for (int k = 0; k < Buildings.Count; k++)
        {
            if (Buildings[k].id == buildingID)
                building = Buildings[k];
        }

        if (building == null)
        {
            Debug.LogError("building not found");
            return null;
        }
        Tile[] adjacentTiles = Board.GetAdjacentTiles(building.x, building.y);
        Tile[] adjacentEmptyTiles = new Tile[] { null, null, null, null, null, null};

        for (int i = 0; i < adjacentTiles.Length; i++) 
        {
            if (adjacentTiles[i] != null)
            {
                if (adjacentTiles[i].currentBuilding == null &&
                    adjacentTiles[i].currentCharacter == null &&
                    adjacentTiles[i].tileCover.Type != TileCover.CoverType.ctUndefined)
                {
                    if (onlyBuildableTiles)
                    {
                        if (adjacentTiles[i].tileType == Tile.TileType.ttBuildable)
                            adjacentEmptyTiles[i] = adjacentTiles[i];
                    }
                    else
                        adjacentEmptyTiles[i] = adjacentTiles[i];
                }
            }
        }

        return adjacentEmptyTiles;
    }

    public Character GetBoardCharacter(uint id )
    {
        for (int k = 0; k < BoardCharacters.Count; k++)
        {
            if (BoardCharacters[k].id == id)
                return BoardCharacters[k];
        }
        return null;
    }

    public Character GetCharacter(uint id)
    {
        for (int k = 0; k < Characters.Count; k++)
        {
            if (Characters[k].id == id)
                return Characters[k];
        }
        return null;
    }

    [HideInInspector] public Team[] Teams = new Team[2];

    [HideInInspector] public bool StartWithRed;//start with random red/blue
    [HideInInspector] public bool RedMove;//current team
    [HideInInspector] public bool GeneralMove = true;//start with general move

    [HideInInspector] public bool PlayingAsRed = true;
    [HideInInspector] public bool PlayingAsGeneral = true;
    [HideInInspector] public bool PlayingAsGovernor = true;

    public bool UseNetwork = false;
    public Building lastSourceBuilding;
    public Character lastSourceCharacter;
    [HideInInspector] public int lastWealthAmount;
    [HideInInspector] public int lastPointAmount;

    private Tile lastWealthTile;
    private Tile lastPointTile;
    public Character lastCharacterMoved;
    public Tile lastTileCovered;
    public Character lastHurterC;
    public Building lastHurterB;
    public Character lastHurtCharacter;
    public Building lastHurtBuilding;
    public Building lastBoughtBuilding;
    public Character lastBoughtCharacter;
    public Character lastPlacedCharacter;
    public Building lastPlacedBuilding;

    CenturionGame()
    {
        if(Instance == null )
            Instance = this;
        for( int i = 0; i < Teams.Length; i++ )
        {
            Teams[i] = new Team();
            Teams[i].Type = Team.TeamType.ttRed + i;
            Teams[i].InitPlayers();
        }

        RedMove = StartWithRed = true;
    }

    public void Awake()
    {
        if (!UseNetwork)//add some stuff on board if not using network
        {
            addInitialCharacters();
        }
    }

    private void Start()
    {
        if (!UseNetwork)//add some stuff on board if not using network
        {
            onGameReload.Invoke();
        }
#if UNITY_EDITOR
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 45;
#endif
    }

    public void ResetGame()
    {
        Characters.Clear();
        WarCharacters.Clear();
        CivilCharacters.Clear();
        BoardCharacters.Clear();
        Buildings.Clear();
        WarBuildings.Clear(); 
        CivilBuildings.Clear(); 
        BoardBuildings.Clear();

        Board.Reset();

        Teams[0].General.ResetGame();
        Teams[0].Governor.ResetGame();
        Teams[1].General.ResetGame();
        Teams[1].Governor.ResetGame();
    }

    public void LoadFromNetwork(ByteArray data)
    {
        ResetGame();

        StartWithRed = data.readBoolean();
        RedMove = data.readBoolean();
        GeneralMove = data.readBoolean();
        mRoundState = (RoundState)data.readByte();

        Board.LoadFromNetwork(data);
        //load characters
        int numChars = data.readByte();
        for (int k = 0; k < numChars; k++)
        {
            Character ch = new Character();
            ch.LoadFromNetwork(data);
            addCharacter(ch, (Team.TeamType)data.readByte() );
        }

        int numBuildings = data.readByte();
        for( int k = 0; k < numBuildings; k++)
        {
            Building building = new Building();
            building.LoadFromNetwork(data);
            addBuilding(building, (Team.TeamType)data.readByte());
        }

        Teams[0].LoadFromNetwork(data);
        Teams[1].LoadFromNetwork(data);
        
        PlayingAsRed = (Teams[0].Governor.NetworkPlayerID == SystemInfo.deviceUniqueIdentifier) || (Teams[0].General.NetworkPlayerID == SystemInfo.deviceUniqueIdentifier);
        PlayingAsGeneral = (Teams[0].General.NetworkPlayerID == SystemInfo.deviceUniqueIdentifier) || (Teams[1].General.NetworkPlayerID == SystemInfo.deviceUniqueIdentifier);
        PlayingAsGovernor = (Teams[0].Governor.NetworkPlayerID == SystemInfo.deviceUniqueIdentifier) || (Teams[1].Governor.NetworkPlayerID == SystemInfo.deviceUniqueIdentifier);

        onGameReload.Invoke();
    }

    //called while there is no server
    void addInitialCharacters()
    {
        addCharacter(Character.CharacterType.ctScout, 5, 0, Team.TeamType.ttRed, Character.CharacterState.csBoard);
        addCharacter(Character.CharacterType.ctScout, 1, 6, Team.TeamType.ttBlue, Character.CharacterState.csBoard);

        addCharacter(Character.CharacterType.ctSurveyor, 6, 1, Team.TeamType.ttRed, Character.CharacterState.csBoard);
        addCharacter(Character.CharacterType.ctSurveyor, 0, 5, Team.TeamType.ttBlue, Character.CharacterState.csBoard);

        addBuilding(Building.BuildingType.btSenate, 6, 0, Team.TeamType.ttRed, Building.BuildingState.bsBoard);
        addBuilding(Building.BuildingType.btSenate, 0, 6, Team.TeamType.ttBlue, Building.BuildingState.bsBoard);
    }

    private void addCharacter(Character unit, Team.TeamType team)
    {
        Characters.Add(unit);
        

        switch (team)
        {
            case Team.TeamType.ttRed:
                unit.Team = Teams[0];
                break;
            case Team.TeamType.ttBlue:
                unit.Team = Teams[1];
                break;
            default:
                unit.Team = null;
                break;
        }


        switch (unit.state)
        {
            case CharacterState.csStack:
                if (unit.isWarUnit)
                {
                    WarCharacters.Add(unit);
                    lastAddedWarCharacter = unit;
                }
                else
                {
                    CivilCharacters.Add(unit);
                    lastAddedCivilCharacter = unit;
                }
                break;
            case CharacterState.csHand:
                if (unit.isWarUnit)
                {
                    unit.Team.General.StandByCharacters.Add(unit);
                }
                else
                {
                    unit.Team.Governor.StandByCharacters.Add(unit);
                }
                break;
            case CharacterState.csBoard:
                BoardCharacters.Add(unit);
                Board.GetTile(unit.x, unit.y).currentCharacter = unit;
                break;
            case CharacterState.csDead:
                break;
        }
    }

    //for internal use
    private void addCharacter(Character.CharacterType charType, byte x, byte y, Team.TeamType team, Character.CharacterState state)
    {
        Character unit = CharacterFactory.CreateCharacter(charType);
        unit.x = x;
        unit.y = y;
        unit.state = state;

        addCharacter(unit, team);
    }

    private void addBuilding(Building unit, Team.TeamType team)
    {
        switch (team)
        {
            case Team.TeamType.ttRed:
                unit.Team = Teams[0];
                break;
            case Team.TeamType.ttBlue:
                unit.Team = Teams[1];
                break;
            default:
                unit.Team = null;
                break;
        }
        Buildings.Add(unit);
        switch (unit.State)
        {
            case BuildingState.bsStack:

                switch (unit.Class)
                {
                    case BuildingClass.bcSenate:
                        unit.Team.Senate = unit;
                        break;
                    case BuildingClass.bcCivil:
                        CivilBuildings.Add(unit);
                        lastAddedCivilBuilding = unit;
                        break;
                    case BuildingClass.bcWar:
                        WarBuildings.Add(unit);
                        lastAddedWarBuilding = unit;
                        break;
                }
                break;
            case BuildingState.bsHand:
                if(unit.Class == BuildingClass.bcWar )
                {
                    unit.Team.General.StandByBuildings.Add(unit);
                }
                else
                {
                    unit.Team.Governor.StandByBuildings.Add(unit);
                }
                break;
            case BuildingState.bsBoard:
                BoardBuildings.Add(unit);
                Board.GetTile(unit.x, unit.y).currentBuilding = unit;
                if (unit.Class == BuildingClass.bcSenate)
                    unit.Team.Senate = unit;
                break;
            case BuildingState.bsDead:
                break;
        }
    }
    private void addBuilding(BuildingType buildType, int x, int y, Team.TeamType team, BuildingState state)
    {
        Building unit = BuildingFactory.CreateBuilding(buildType);
        unit.x = x;
        unit.y = y;
        unit.State = state;

        addBuilding(unit, team);
    }

    void onEndTurnChooseNextPlayer()
    {
        //can be optimized - lets do it later ( never )
        if( StartWithRed )
        {
            if( RedMove )
            {
                //move to blue
                RedMove = false;
            }
            else
            {
                //move to red and switch general/governor
                GeneralMove = !GeneralMove;
                RedMove = true;
            }
        }
        else
        {
            if (!RedMove)
            {
                //move to red
                RedMove = true;
            }
            else
            {
                //move to blue and switch general/governor
                GeneralMove = !GeneralMove;
                RedMove = false;
            }
        }
    }

    public void OnRoundUpdate(bool _RedMove, bool _GeneralMove, RoundState _roundState, int _placementPriceMultiplier, int _free_unit_cards)
    {
        Debug.Log("round state updated");
        RedMove = _RedMove;
        GeneralMove = _GeneralMove;
        mRoundState = _roundState;
        placementPriceMultiplier = _placementPriceMultiplier;
        free_unit_cards = _free_unit_cards;
        if (mRoundState == RoundState.rsMovingCharacters)
        {
            //reset moving character move values
            for(int k =0; k < BoardCharacters.Count; k++)
            {
                BoardCharacters[k].StepsUsed = 0;
                BoardCharacters[k].AdditionalSteps = 0;
            }
        }
        onRoundStateChange.Invoke();
    }

    public void OnWealthFromBuilding(Team.TeamType tt, int wealth, uint sourceBuilding)
    {
        Teams[tt == Team.TeamType.ttRed ? 0 : 1].Gold += wealth;
        lastSourceBuilding = GetBoardBuilding(sourceBuilding);
        lastWealthAmount = wealth;
        onWealthFromBuilding.Invoke();
    }

    public void OnWealthFromCharacter(Team.TeamType tt, int wealth, uint sourceBuilding)
    {
        Teams[tt == Team.TeamType.ttRed ? 0 : 1].Gold += wealth;
        lastSourceCharacter = GetBoardCharacter(sourceBuilding);
        lastWealthAmount = wealth;
        onWealthFromCharacter.Invoke();
    }

    public void OnUnitGotAdditionalSteps(uint sourceChar, int numAdditionalSteps)
    {
        lastSourceCharacter = GetBoardCharacter(sourceChar);
        lastSourceCharacter.AdditionalSteps = numAdditionalSteps;
        onUnitGotAdditionalSteps.Invoke();
    }

    public void OnCharacterMoved(uint characterId, int x, int y, int stepsUsed)
    {
        Debug.Log("character moved " + characterId.ToString() + " to " + x.ToString() + "x" + y.ToString() );
        for(int k = 0; k < BoardCharacters.Count; k++ )
        {
            if(BoardCharacters[k].id == characterId )
            {
                Board.GetTile(BoardCharacters[k].x, BoardCharacters[k].y).currentCharacter = null;
                Board.GetTile(x, y).currentCharacter = BoardCharacters[k];
                BoardCharacters[k].x = x;
                BoardCharacters[k].y = y;
                BoardCharacters[k].StepsUsed = stepsUsed;
                lastCharacterMoved = BoardCharacters[k];
                break;
            }
        }
        onCharacterMoved.Invoke();
    }

    public void OnTileCovered(int x, int y, TileCover.CoverType ct, TileCover.BonusType bt )
    {
        Board.GetTile(x, y).tileCover.Type = ct;
        Board.GetTile(x, y).tileCover.Bonus = bt;
        lastTileCovered = Board.GetTile(x, y);
        onTileCovered.Invoke();
    }

    public void OnWealthFromCover(Team.TeamType tt, int wealth, int x, int y)
    {
        Teams[tt == Team.TeamType.ttRed ? 0 : 1].Gold += wealth;
        lastWealthAmount = wealth;
        lastWealthTile = Board.GetTile(x, y);
        onWealthFromTile.Invoke();
    }

    public void OnPointFromCover(Team.TeamType tt, int points, int x, int y)
    {
        Teams[tt == Team.TeamType.ttRed ? 0 : 1].VictoryPoints += points;
        lastPointAmount = points;
        lastPointTile = Board.GetTile(x, y);
        onPointsFromTile.Invoke();
    }

    public void OnPointFromBuilding(Team.TeamType tt, int points, uint bid)
    {
        Teams[tt == Team.TeamType.ttRed ? 0 : 1].VictoryPoints += points;
        lastSourceBuilding = GetBoardBuilding(bid);
        lastPointAmount = points;
        onPointsFromBuilding.Invoke();
    }

    public void OnCharacterHurt(uint charid, int health, uint reason)
    {
        Character ch = GetBoardCharacter(charid);
        ch.Health = health;
        if( ch.Health == 0 )
        {
            BoardCharacters.Remove(ch);
            Board.GetTile(ch.x, ch.y).currentCharacter = null;
            if(ch.isWarUnit)
            {
                ch.Team.General.DeadCharacters.Add(ch);
                ch.Team.General.Characters.Remove(ch);
            }
            else
            {
                ch.Team.Governor.DeadCharacters.Add(ch);
                ch.Team.Governor.Characters.Remove(ch);
            }
        }
        lastHurterC = reason != 0 ? ( GetCharacter(reason) ) : null;
        lastHurterB = reason != 0 ? (GetBuilding(reason)) : null;
        lastHurtCharacter = ch;
        onCharacterHurt.Invoke();
    }
    public void OnBuildingHurt(uint bid, int health, uint reason)
    {
        Building ch = GetBoardBuilding(bid);
        ch.Health = health;
        if (ch.Health == 0)
        {
            BoardBuildings.Remove(ch);
            Board.GetTile(ch.x, ch.y).currentBuilding = null;
            if (ch.Class == Building.BuildingClass.bcWar)
            {
                ch.Team.General.DeadBuildings.Add(ch);
                ch.Team.General.Buildings.Remove(ch);
            }
            else
            {
                ch.Team.Governor.DeadBuildings.Add(ch);
                ch.Team.Governor.Buildings.Remove(ch);
            }
        }
        lastHurterC = reason != 0 ? (GetCharacter(reason)) : null;
        lastHurterB = reason != 0 ? (GetBuilding(reason)) : null;
        lastHurtBuilding = ch;
        onBuildingHurt.Invoke();
    }

    public void OnBuildingBought(uint bid, Team.TeamType tt )
    {
        Building ch = GetBuilding(bid);
        ch.State = BuildingState.bsHand;
        ch.Team = tt == Team.TeamType.ttRed ? Teams[0] : Teams[1];
        if(ch.Class == Building.BuildingClass.bcWar)
        {
            ch.Team.General.StandByBuildings.Add(ch);
        }
        else
        {
            ch.Team.Governor.StandByBuildings.Add(ch);
        }
        lastBoughtBuilding = ch;
        onBuildingBought.Invoke();
    }
    public void OnCharacterBought(uint bid, Team.TeamType tt)
    {
        Character ch = GetCharacter(bid);
        ch.state = CharacterState.csHand;
        ch.Team = tt == Team.TeamType.ttRed ? Teams[0] : Teams[1];
        if (ch.isWarUnit)
        {
            ch.Team.General.StandByCharacters.Add(ch);
        }
        else
        {
            ch.Team.Governor.StandByCharacters.Add(ch);
        }
        lastBoughtCharacter = ch;
        onCharacterBought.Invoke();
    }

    public void OnUpdateGold(Team.TeamType tt, int Gold)
    {
        Teams[tt == Team.TeamType.ttRed ? 0 : 1].Gold = Gold;
        onUpdateGold.Invoke();
    }
    public void OnUpdatePoints(Team.TeamType tt, int Points)
    {
        Teams[tt == Team.TeamType.ttRed ? 0 : 1].VictoryPoints = Points;
        onUpdatePoints.Invoke();
    }

    public void OnPlaceCharacter(uint cid, Team.TeamType tt, int x, int y)
    {
        Character c = GetCharacter(cid);
        c.state = CharacterState.csBoard;
        BoardCharacters.Add(c);
        c.x = x;
        c.y = y;
        Board.GetTile(x, y).currentCharacter = c;
        if(c.isWarUnit)
        {
            Teams[tt == Team.TeamType.ttRed ? 0 : 1].General.StandByCharacters.Remove(c);
            Teams[tt == Team.TeamType.ttRed ? 0 : 1].General.Characters.Add(c);
        }
        else
        {
            Teams[tt == Team.TeamType.ttRed ? 0 : 1].Governor.StandByCharacters.Remove(c);
            Teams[tt == Team.TeamType.ttRed ? 0 : 1].Governor.Characters.Add(c);
        }

        lastPlacedCharacter = c;

        onPlaceCharacter.Invoke();
    }

    public void OnPlaceBuilding(uint cid, Team.TeamType tt, int x, int y)
    {
        Building c = GetBuilding(cid);
        c.State = BuildingState.bsBoard;
        BoardBuildings.Add(c);
        c.x = x;
        c.y = y;
        Board.GetTile(x, y).currentBuilding = c;
        if (c.Class == BuildingClass.bcWar )
        {
            Teams[tt == Team.TeamType.ttRed ? 0 : 1].General.StandByBuildings.Remove(c);
            Teams[tt == Team.TeamType.ttRed ? 0 : 1].General.Buildings.Add(c);
        }
        else
        {
            Teams[tt == Team.TeamType.ttRed ? 0 : 1].Governor.StandByBuildings.Remove(c);
            Teams[tt == Team.TeamType.ttRed ? 0 : 1].Governor.Buildings.Add(c);
        }

        lastPlacedBuilding = c;

        onPlaceBuilding.Invoke();
    }

    public void OnGameFinished(Team.TeamType tt)
    {
        WinnerTeam = tt;
        onGameFinished.Invoke();
    }

    public void OnOnlineOffline(bool IsOnline, Team.TeamType tt, string Name )
    {
        Teams[tt == Team.TeamType.ttRed ? 0 : 1].General.IsOnline = IsOnline;
        Teams[tt == Team.TeamType.ttRed ? 0 : 1].Governor.IsOnline = IsOnline;
        if( IsOnline)
        {
            Teams[tt == Team.TeamType.ttRed ? 0 : 1].General.NetworkPlayerName = Name;
            Teams[tt == Team.TeamType.ttRed ? 0 : 1].Governor.NetworkPlayerName = Name;
        }
        onOnlineOffline.Invoke();
    }

    public void OnStackUpdateCharacter(ByteArray incomingData)
    {
        Character ch = new Character();
        ch.LoadFromNetwork(incomingData);
        addCharacter(ch, (Team.TeamType)incomingData.readByte());
        onStackUpdateCharacter.Invoke();
        Debug.Log("event called. Character" + ch.Name);
    }

    public void OnStackUpdateBuilding(ByteArray incomingData)
    {
        Building ch = new Building();
        ch.LoadFromNetwork(incomingData);
        addBuilding(ch, (Team.TeamType)incomingData.readByte());
        onStackUpdateBuilding.Invoke();
    }

    public void OnDoMadman()
    {
        var tempHand = Teams[0].Governor.StandByCharacters;
        Teams[0].Governor.StandByCharacters = Teams[1].Governor.StandByCharacters;
        Teams[1].Governor.StandByCharacters = tempHand;

        tempHand = Teams[0].General.StandByCharacters;
        Teams[0].General.StandByCharacters = Teams[1].General.StandByCharacters;
        Teams[1].General.StandByCharacters = tempHand;

        for (int k = 0; k < Teams[0].Governor.StandByCharacters.Count; k++)
        {
            Teams[0].Governor.StandByCharacters[k].Team = Teams[0];
        }
        for (int k = 0; k < Teams[1].Governor.StandByCharacters.Count; k++)
        {
            Teams[1].Governor.StandByCharacters[k].Team = Teams[1];
        }
        for (int k = 0; k < Teams[0].General.StandByCharacters.Count; k++)
        {
            Teams[0].General.StandByCharacters[k].Team = Teams[0];
        }
        for (int k = 0; k < Teams[1].General.StandByCharacters.Count; k++)
        {
            Teams[1].General.StandByCharacters[k].Team = Teams[1];
        }
        onDoMadman.Invoke();
    }

    public void OnSwapSides(uint v)
    {
        Character character = GetBoardCharacter(v);
        Team oldTeam = character.Team;
        Team newTeam = oldTeam.Type == Team.TeamType.ttRed ? Teams[1] : Teams[0];

        if(character.isWarUnit)
        {
            oldTeam.General.Characters.Remove(character);
            newTeam.General.Characters.Add(character);
        }
        else
        {
            oldTeam.Governor.Characters.Remove(character);
            newTeam.Governor.Characters.Add(character);
        }
        character.Team = newTeam;
        lastSwappedChar = character;
        onSwapSides.Invoke();
    }
}
