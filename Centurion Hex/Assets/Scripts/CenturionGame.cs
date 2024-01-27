using Assets.Scripts.Buildings;
using Assets.Scripts.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CenturionGame : MonoBehaviour
{
    public static CenturionGame Instance { get; private set; }
    public UnityEvent onGameReload;

    public Board Board = new Board();

    //all characters in deck
    public List<Character> Characters = new List<Character>();

    //shortcuts
    public List<Character> WarCharacters = new List<Character>();
    public List<Character> CivilCharacters = new List<Character>();
    public List<Character> BoardCharacters = new List<Character>();

    public Team[] Teams = new Team[2];

    public bool StartWithRed;//start with random red/blue
    public bool RedMove;//current team
    public bool GeneralMove = true;//start with general move

    CenturionGame()
    {
        Instance = this;
        for( int i = 0; i < Teams.Length; i++ )
        {
            Teams[i] = new Team();
            Teams[i].Type = Team.TeamType.ttRed + i;
            Teams[i].InitPlayers();

            //place initial structure
            Teams[i].Senate = new Senate();
            Teams[i].Senate.x = i == 0 ? 6 : 0;
            Teams[i].Senate.y = i == 0 ? 0 : 6;
        }

        StartWithRed = true;// Random.Range(0,2) == 0;
        RedMove = StartWithRed;
        addInitialCharacters();
    }

    public void LoadFromNetwork(ByteArray data)
    {
        StartWithRed = data.readBoolean();
        RedMove = data.readBoolean();
        GeneralMove = data.readBoolean();
        Board.LoadFromNetwork(data);
        onGameReload.Invoke();
    }

    //called while there is no server
    void addInitialCharacters()
    {
        addCharacter(Character.CharacterType.ctScout, 5, 0, Team.TeamType.ttRed, Character.CharacterState.csBoard);
        addCharacter(Character.CharacterType.ctScout, 1, 6, Team.TeamType.ttBlue, Character.CharacterState.csBoard);

        addCharacter(Character.CharacterType.ctSurveyor, 6, 1, Team.TeamType.ttRed, Character.CharacterState.csBoard);
        addCharacter(Character.CharacterType.ctSurveyor, 0, 5, Team.TeamType.ttBlue, Character.CharacterState.csBoard);
    }

    private void addCharacter(Character.CharacterType charType, int x, int y, Team.TeamType team, Character.CharacterState state)
    {
        Character unit = CharacterFactory.CreateCharacter(charType);
        unit.x = x;
        unit.y = y;
        unit.state = state;

        switch(team)
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
        Characters.Add(unit);
        switch( unit.state)
        {
            case Character.CharacterState.csStack:
                if( unit.isWarUnit )
                {
                    WarCharacters.Add(unit);
                }
                else
                {
                    CivilCharacters.Add(unit);
                }
                break;
            case Character.CharacterState.csHand:
                break;
            case Character.CharacterState.csBoard:
                BoardCharacters.Add(unit);
                Board.GetTile(x, y).currentCharacter = unit;
                break;
            case Character.CharacterState.csDead:
                break;
        }
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


    public void SpawnTile()
    {

    }
}
