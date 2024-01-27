using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public enum PlayerType
    {
        ptRedGovernor,
        ptRedGeneral,
        ptBlueGovernor,
        ptBlueGeneral,
    }

    public PlayerType Type;
    public string NetworkPlayerID;
    public Team Team;

    public int Gold = 2;//initial gold amount

    public List<Character> Characters = new List<Character>();//on board
    public List<Building> Buildings = new List<Building>();//on board

    public List<Character> StandByCharacters = new List<Character>();
    public List<Building> StandByBuildings = new List<Building>();
    public List<Character> DeadCharacters = new List<Character>();
    public List<Building> DeadBuildings = new List<Building>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadFromNetwork(ByteArray data)
    {
        Type = (PlayerType)data.readByte();
        NetworkPlayerID = data.readUTF();
    }
}
