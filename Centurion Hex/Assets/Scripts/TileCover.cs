using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCover
{
    public enum CoverType
    {
        ctUndefined = 0,//fog of war?
        ctGrass = 1,
        ctSwamp = 2,
        ctForest = 3,
        ctDesert = 4,
        ctTransparent = 5//for senate and adjacent
    }

    public enum BonusType
    {
        btNone,
        btStar,
        btSkull,
    }

    public CoverType Type = CoverType.ctUndefined;
    public BonusType Bonus = BonusType.btNone;


    /// <summary>
    /// single player only
    /// </summary>
    public void SP_SetRandomCoverType()
    {
        int randomRoll = UnityEngine.Random.Range(1, 5);

        Type = (CoverType)randomRoll;
    }

    /// <summary>
    /// single player only
    /// </summary>
    public void SP_SetRandomBonusType()
    {
        float bonusChance = 0.33f;
        float starChance = 0.7f;

        if (UnityEngine.Random.Range(0, 1f) > bonusChance)
            return;

        if (UnityEngine.Random.Range(0, 1f) < starChance)
        {
            Bonus = BonusType.btStar;
        }
        else
        {
            Bonus = BonusType.btSkull;
        }
        
    }

    public void LoadFromNetwork(ByteArray data)
    {
        Type = (CoverType)data.readByte();
        Bonus = (BonusType)data.readByte();
    }
}
