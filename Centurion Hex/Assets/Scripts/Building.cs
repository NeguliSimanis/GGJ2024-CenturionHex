using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public BuildingClass Class;
    public BuildingState State;
    public BuildingType Type;

    public int x;
    public int y;

    public bool requireNextToAlly;
    public Tile.TileType requiredTileType;//void if can build on any

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
}
