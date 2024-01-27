using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team
{
    public enum TeamType
    {
        ttNone,
        ttRed,
        ttBlue,
    }

    public TeamType Type;
    
    public Player Governor;
    public Player General;

    public Building Senate;
    public int Gold;
    public int VictoryPoints;

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
        Governor.Team = this;
        Governor.Type = Type == TeamType.ttRed ? Player.PlayerType.ptRedGovernor : Player.PlayerType.ptBlueGovernor;
        General = new Player();
        General.Type = Type == TeamType.ttRed ? Player.PlayerType.ptRedGeneral : Player.PlayerType.ptBlueGeneral;
        General.Team = this;
    }

    public void LoadFromNetwork(ByteArray data)
    {
        Type = (TeamType)data.readByte();
        Gold = data.readByte();
        VictoryPoints = data.readByte();
        Governor.LoadFromNetwork(data);
        General.LoadFromNetwork(data);
    }
}

