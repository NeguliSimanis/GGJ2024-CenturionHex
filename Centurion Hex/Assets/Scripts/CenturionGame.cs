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

    public RoundState mRoundState;

    public static CenturionGame Instance { get; private set; }
    public UnityEvent onGameReload;
    public UnityEvent onRoundStateChange;
    public UnityEvent onWealthFromBuilding;
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

    public Board Board = new Board();

    //all characters in deck
    public List<Character> Characters = new List<Character>();

    //shortcuts
    public List<Character> WarCharacters = new List<Character>();
    public List<Character> CivilCharacters = new List<Character>();
    public List<Character> BoardCharacters = new List<Character>();

    //all biuldings in deck
    public List<Building> Buildings = new List<Building>();

    //shortcuts
    public List<Building> WarBuildings = new List<Building>();
    public List<Building> CivilBuildings = new List<Building>();
    public List<Building> BoardBuildings = new List<Building>();

    public Building GetBoardBuilding(uint id )
    {
        for( int k = 0; k < BoardBuildings.Count; k++ )
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

    public Team[] Teams = new Team[2];

    public bool StartWithRed;//start with random red/blue
    public bool RedMove;//current team
    public bool GeneralMove = true;//start with general move

    public bool PlayingAsRed = true;
    public bool PlayingAsGeneral = true;
    public bool PlayingAsGovernor = true;

    public bool UseNetwork = false;
    public Building lastSourceBuilding;
    public int lastWealthAmount;
    public int lastPointAmount;

    private Tile lastWealthTile;
    private Tile lastPointTile;
    public Character lastCharacterMoved;
    public Tile lastTileCovered;
    public Character lastHurtCharacter;
    public Building lastHurtBuilding;
    public Building lastBoughtBuilding;
    public Character lastBoughtCharacter;

    CenturionGame()
    {
        Instance = this;
        for( int i = 0; i < Teams.Length; i++ )
        {
            Teams[i] = new Team();
            Teams[i].Type = Team.TeamType.ttRed + i;
            Teams[i].InitPlayers();
        }

        RedMove = StartWithRed = true;
    }

    public void Start()
    {
        if (!UseNetwork)//add some stuff on board if not using network
        {
            addInitialCharacters();
            onGameReload.Invoke();
        }
#if UNITY_EDITOR
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 45;
#endif
    }

    public void LoadFromNetwork(ByteArray data)
    {
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
                }
                else
                {
                    CivilCharacters.Add(unit);
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
                        break;
                    case BuildingClass.bcWar:
                        WarBuildings.Add(unit);
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

    public void OnRoundUpdate(bool _RedMove, bool _GeneralMove, RoundState _roundState)
    {
        Debug.Log("round state updated");
        RedMove = _RedMove;
        GeneralMove = _GeneralMove;
        mRoundState = _roundState;
        if(mRoundState == RoundState.rsMovingCharacters)
        {
            //reset moving character move values
            for(int k =0; k < BoardCharacters.Count; k++)
            {
                BoardCharacters[k].StepsUsed = 0;
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

    public void OnCharacterHurt(uint charid, int health)
    {
        Character ch = GetBoardCharacter(charid);
        ch.Health = health;
        if( ch.Health == 0 )
        {
            BoardCharacters.Remove(ch);
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
        lastHurtCharacter = ch;
        onCharacterHurt.Invoke();
    }
    public void OnBuildingHurt(uint bid, int health)
    {
        Building ch = GetBoardBuilding(bid);
        ch.Health = health;
        if (ch.Health == 0)
        {
            BoardBuildings.Remove(ch);
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
}
