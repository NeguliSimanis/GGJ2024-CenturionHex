using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public enum TileType
    {
        ttVoid,//non present on board
        ttSenate,//senate sits on this
        ttBuildable,//can build on this
        ttEmpty,//empty white tile
        ttSlow,//removes 2 steps
        ttMoney,//gives 2 gold
        ttCenter,//gives bonus/victory point
    }

    public TileType tileType = TileType.ttVoid;
    public TileCover tileCover = new TileCover();
    public Character currentCharacter = null;
    public Building currentBuilding = null;

    bool CanWalkOnThis()
    {
        return tileType != TileType.ttVoid;
    }
    
    public bool IsEmpty()
    {
        bool isEmpty = true;
        if (currentBuilding != null)
        {
            isEmpty = false;
        }
        if (currentCharacter != null)
        {
            //Debug.Log("CONTAINS CHARACTER");
            isEmpty = false;
        }
        return isEmpty;
    }


    //// doesnt work correctly
    //public bool IsDiscovered()
    //{
    //    bool isDiscovered = true;
    //    if (tileCover.Type == TileCover.CoverType.ctUndefined)
    //        isDiscovered = false;

    //    return isDiscovered;
    //}

    public void LoadFromNetwork(ByteArray data)
    {
        tileType = (TileType)data.readByte();
        tileCover.LoadFromNetwork(data);
    }

    public void Reset()
    {
        currentCharacter = null;
        currentBuilding = null;
    }
}
