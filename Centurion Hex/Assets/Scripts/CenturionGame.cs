using Assets.Scripts.Buildings;
using Assets.Scripts.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenturionGame : MonoBehaviour
{
    public Board Board = new Board();

    public Team[] Teams = new Team[2];

    public bool StartWithRed;//start with random red/blue
    public bool RedMove;//current team
    public bool GeneralMove = true;//start with general move

    public GameObject tileObject;
    

    CenturionGame()
    {
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

    //called while there is no server
    void addInitialCharacters()
    {
        Character scout = new Scout();
        scout.x = 5; scout.y = 0;
        Teams[0].General.Characters.Add(scout);

        scout = new Scout();
        scout.x = 1; scout.y = 6;
        Teams[1].General.Characters.Add(scout);


        Character surveyor = new Surveyor();
        surveyor.x = 6; surveyor.y = 1;
        Teams[0].Governor.Characters.Add(surveyor);

        scout = new Surveyor();
        scout.x = 0; scout.y = 5;
        Teams[1].Governor.Characters.Add(surveyor);
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
