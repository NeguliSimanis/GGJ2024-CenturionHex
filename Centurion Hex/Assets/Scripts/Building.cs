using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TileCover;

public class Building
{
    public enum BuildingClass
    {
        bcSenate,
        bcWar,
        bcCivil,
    }

    public enum BuildingType
    {
        btSenate,
    }

    public enum BuildingState
    {
        bsStack,
        bsHand,
        bsBoard,
        bsDead
    }

    public Team Team;

    public uint id;

    public BuildingClass Class;
    public BuildingState State;
    public BuildingType Type;

    public int x;
    public int y;

    public bool requireNextToAlly;
    public CoverType requiredTileType;//void if can build on any
    public int victoryPoints;
    public int price;
    public string Name;
    public string Description;
    public int Health;
    public int InitialHealth;
    public virtual void onPlayed()
    {

    }

    public virtual void onAttack()
    {
        Health--;//override this for senate to check if shrine is present
    }

    public void LoadFromNetwork(ByteArray data)
    {
        id = data.readUnsignedInt();
        Class = (BuildingClass)data.readByte();
        State = (BuildingState)data.readByte();
        Type = (BuildingType)data.readByte();
        x = data.readByte();
        y = data.readByte();
        requireNextToAlly = data.readBoolean();
        requiredTileType = (CoverType)data.readByte();
        victoryPoints = data.readByte();
        price = data.readByte();
        Name = data.readUTF();
        Description = data.readUTF();
        Health = data.readByte();
        InitialHealth = data.readByte();
    }
}
