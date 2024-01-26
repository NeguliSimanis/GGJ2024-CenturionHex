using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building
{
    public enum BuildingType
    {
        btSenate,
        btWar,
        btCivil,
    }

    public BuildingType Type;

    public int x;
    public int y;

    public bool requireNextToAlly;
    public Tile.TileType requiredTileType;//void if can build on any

    public int price;
    public string Name;
    public string Description;
    public int Health;
    public void onPlayed()
    {

    }

    public void onAttack()
    {
        Health--;//override this for senate to check if shrine is present
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
