using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public int x;
    public int y;
    public int Health;
    public int Price;
    public int StepsPerTurn;
    public int AttackDamage;
    public int AttackRange;//only for snipers
    public string Name;
    public string Description;

    public Team Team;

    public bool CanBePlayedOffTurn;//if possible to play when not your turn

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
}
