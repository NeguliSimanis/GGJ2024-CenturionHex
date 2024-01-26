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

    public bool CanBePlayedOffTurn;//if possible to play when not your turn

    public int GetStepsPerTurn()
    {
        return StepsPerTurn;//override if some other logic
    }

    public void onPlayed()
    {
        //action to do when placed on board
    }

    public void DoTurnAction()
    {
        //does specific action on turn
    }

    public void DoAttack()
    {
        //what to do when it's attacking
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
