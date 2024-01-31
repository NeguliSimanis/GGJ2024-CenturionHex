using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCover
{
    public enum CoverType
    {
        ctUndefined,//fog of war?
        ctGrass,
        ctSwamp,
        ctForest,
        ctDesert,
        ctTransparent//for senate and adjacent
    }

    public enum BonusType
    {
        btNone,
        btStar,
        btSkull,
    }

    public CoverType Type = CoverType.ctUndefined;
    public BonusType Bonus = BonusType.btNone;

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
        Type = (CoverType)data.readByte();
        Bonus = (BonusType)data.readByte();
    }
}
