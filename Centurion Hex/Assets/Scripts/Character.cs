using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public enum CharacterType
    {
    ctScout,
    ctSurveyor,
    ctSenator,
    ctMerchant,
    ctBarista,
    ctDiplomat,
    ctMadman,
    ctPrincess,
    ctLegionary,
    ctRaptor,
    ctSniper,
    ctWitch,
        undefined
    }
    public enum CharacterState
    {
        csStack,
        csHand,
        csBoard,
        csDead
    }
    public uint id;
    public CharacterType type;
    public CharacterState state = CharacterState.csStack;
    public bool isWarUnit;
    public int x;
    public int y;
    public int Health;
    public int InitialHealth;
    public int Price;
    public int StepsPerTurn;
    public int AttackDamage;
    public int AttackRange;//only for snipers
    public string Name;
    public string Description;

    public Team Team;

    public bool CanBePlayedOffTurn;//if possible to play when not your turn
    public int StepsUsed;//steps used in current round

    public virtual int GetStepsPerTurn()
    {
        return StepsPerTurn;//override if some other logic
    }

    public virtual void onPlayed()
    {
        //action to do when placed on board
    }

    public virtual void DoTurnAction()
    {
        //does specific action on turn
    }

    public virtual void DoAttack()
    {
        //what to do when it's attacking
    }

    public void LoadFromNetwork(ByteArray data)
    {
        id = data.readUnsignedInt();
        type = (CharacterType)data.readByte();
        state = (CharacterState)data.readByte();
        isWarUnit = data.readBoolean();
        x = data.readByte();
        y = data.readByte();
        Health = data.readByte();
        InitialHealth = data.readByte();
        Price = data.readByte();
        StepsPerTurn = data.readByte();
        AttackDamage = data.readByte();
        AttackRange = data.readByte();
        Name = data.readUTF();
        Description = data.readUTF();
        CanBePlayedOffTurn = data.readBoolean();
    }
}
