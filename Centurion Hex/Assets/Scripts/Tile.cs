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
    public Character currentCharacter;

    bool CanWalkOnThis()
    {
        return tileType != TileType.ttVoid;
    }
    
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
        tileType = (TileType)data.readByte();
        tileCover.LoadFromNetwork(data);
    }
}
