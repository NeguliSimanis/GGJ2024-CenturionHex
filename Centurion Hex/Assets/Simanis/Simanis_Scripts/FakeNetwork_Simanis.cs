using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FakeNetwork_Simanis : MonoBehaviour
{
    public static FakeNetwork_Simanis Instance { get; private set; }

    [Header("turn managment")]
    public float turnDuration = 30f;
    [HideInInspector] public bool isAllyTurn = true;
    [HideInInspector] public bool isEnemyTurn = false;
    [HideInInspector] public CenturionGame.RoundState roundState;
    [HideInInspector] public bool isWarTurn = true;
    [HideInInspector] public bool isCivilTurn = false;
    [HideInInspector] public float turnStartTime;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (CenturionGame.Instance.UseNetwork)
            return;
        roundState = CenturionGame.RoundState.rsMovingCharacters;
        SP_StartTurnPhase();
    }

    private void FixedUpdate()
    {
        if (CenturionGame.Instance.UseNetwork)
            return;

        if (Time.time > turnStartTime + turnDuration)
        {
            SP_EndTurnPhase();
        }

    }

    public void SP_EndTurnPhase()
    {
        switch(roundState)
        {
            // WAS GENERATING WEALTH
            case CenturionGame.RoundState.rsGeneratingWealth:
                SP_StartTurnPhase();
                return;
                break;
           // WAS MOVING CHARACTERS
            case CenturionGame.RoundState.rsMovingCharacters:
                //Debug.Log("was movement phase now managmenet");
                roundState = CenturionGame.RoundState.rsManagement;
                SP_StartTurnPhase();
                return;
                break;
            // WAS MANAGEMENT
            case CenturionGame.RoundState.rsManagement:
                Debug.Log("was movement phase now moving");

                if (isWarTurn && isEnemyTurn || isCivilTurn && isEnemyTurn)
                {
                    isWarTurn = !isWarTurn;
                    isCivilTurn = !isCivilTurn;
                }
                isAllyTurn = !isAllyTurn;
                isEnemyTurn = !isEnemyTurn;

                roundState = CenturionGame.RoundState.rsMovingCharacters;
                SP_StartTurnPhase();
                return;
                break;
            case CenturionGame.RoundState.rsNone:
                SP_StartTurnPhase();
                return;
                break;
        }
    }

    private void SP_StartTurnPhase()
    {
        CenturionGame.Instance.OnRoundUpdate(
            _RedMove: isAllyTurn,
            _GeneralMove: isWarTurn,
            _roundState: roundState,
            _placementPriceMultiplier: 1,
            _free_unit_cards: 0
            );
        turnStartTime = Time.time;
    }

    public int CalculateStepsUsed(int targetX, int targetY)
    {
        int stepsUsed = 1;
        Tile targetTile = CenturionGame.Instance.Board.GetTile(targetX, targetY);
        if (targetTile.tileType == Tile.TileType.ttSlow)
            stepsUsed = 2;
        return stepsUsed;
    }

    public void SP_MoveCharacter(uint characterId, int x, int y)
    {
        Character character = CenturionGame.Instance.GetBoardCharacter(characterId);
        Debug.Log("is war turn " + isWarTurn +". is war unit " + character.isWarUnit);
        

        // allow moving war chars only in war turn
        if (character.isWarUnit && !isWarTurn)
        {
            Debug.Log("cannot move its not war turn");
            return;
        }

        if (!character.isWarUnit && !isCivilTurn)
        {
            Debug.Log("cannot move its not civil turn");
            return;
        }

        int distanceBetweenTiles = CenturionGame.Instance.Board.DistanceBetweenTiles(
           character.x, character.y,
           x, y);
        if (distanceBetweenTiles > character.RemainingStepsThisTurn())
            return;

        // calculate remaining speed
        int speedUsed = character.StepsUsed + CalculateStepsUsed(x, y);

        // MOVE CHARACTER 
        CenturionGame.Instance.OnCharacterMoved(
            characterId: characterId,
            x,
            y,
            stepsUsed: speedUsed);

        // DISCOVER TILE
        Tile targetTile = CenturionGame.Instance.Board.GetTile(x, y);
        if (!TileSpawner_Simanis.instance.GetTileVisual(targetTile).isDiscovered)
        {
            Debug.Log("discovering tile");
            CenturionGame.Instance.OnTileCovered(
                x,
                y, 
                targetTile.tileCover.Type,
                targetTile.tileCover.Bonus);
        };
    }

    public void SP_HurtTile(uint characterId, int x, int y)
    {
        // get attacking char info
        Character attackingCharacter = CenturionGame.Instance.GetBoardCharacter(characterId);
        int attackRange = attackingCharacter.AttackRange;
        Debug.Log(attackingCharacter.type + " is attacking!");

        // get if target tile has any valid targets
        Tile targetTile = CenturionGame.Instance.Board.GetTile(x, y);
        if (targetTile.IsEmpty())
        {
            Debug.Log("cannot attack, target tile is empty");
            return;
        }

        // origin tile info
        Tile originTile = CenturionGame.Instance.Board.GetTile(attackingCharacter.x, attackingCharacter.y);

        // 
        int distanceBetweenTiles = CenturionGame.Instance.Board.DistanceBetweenTiles(
            attackingCharacter.x, attackingCharacter.y,
            x, y);

        Debug.Log("my range is " + attackRange + " My damage is " + attackingCharacter.AttackDamage);
        if (distanceBetweenTiles > attackRange + 1)
        {
            Debug.Log("cannot attack, too far");
        }

        else
        {
            // hurt character
            if (targetTile.currentCharacter != null)
            {
                Character targetChar = targetTile.currentCharacter;
                int targetHealth = targetChar.Health - attackingCharacter.AttackDamage;


                Debug.Log("attempting to hurt " + targetChar.type + " Target remaining hp: " + targetHealth);
                CenturionGame.Instance.OnCharacterHurt(charid: targetChar.id,
                    health: targetHealth,
                    reason: attackingCharacter.id);
            }
            // hurt building
            else
            {
                Building targetBuilding = targetTile.currentBuilding;
                int targetHealth = targetBuilding.Health - attackingCharacter.AttackDamage;

                CenturionGame.Instance.OnBuildingHurt(targetBuilding.id,
                    health: targetHealth,
                    reason: attackingCharacter.id);
            }


        }


        // 
        //TileSpawner_Simanis.instance.

        //outgoingData.writeByte((byte)Messages.op_hurt_tile);
        //outgoingData.writeUnsignedInt(characterId);
        //outgoingData.writeByte((byte)x);
        //outgoingData.writeByte((byte)y);
        //Send("hurt_tile");
    }

}
