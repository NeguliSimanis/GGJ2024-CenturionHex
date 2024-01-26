using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    public enum TeamType
    {
        ttRed,
        ttBlue,
    }

    public TeamType Type;
    
    public Player Governor;
    public Player General;

    public Building Senate = new Building();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void InitPlayers()
    {
        Governor = new Player();
        Governor.Type = Type == TeamType.ttRed ? Player.PlayerType.ptRedGovernor : Player.PlayerType.ptBlueGovernor;
        General = new Player();
        General.Type = Type == TeamType.ttRed ? Player.PlayerType.ptRedGeneral : Player.PlayerType.ptBlueGeneral;
    }
}
