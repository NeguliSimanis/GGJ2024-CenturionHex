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
            case CenturionGame.RoundState.rsGeneratingWealth:
                SP_StartTurnPhase();
                return;
                break;
            case CenturionGame.RoundState.rsMovingCharacters:
                Debug.Log("was movement phase now managmenet");
                roundState = CenturionGame.RoundState.rsManagement;
                SP_StartTurnPhase();
                return;
                break;
            case CenturionGame.RoundState.rsManagement:
                Debug.Log("was movement phase now moving");
                isAllyTurn = !isAllyTurn;
                isEnemyTurn = !isEnemyTurn;
                isWarTurn = !isWarTurn;
                isCivilTurn = !isCivilTurn;
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
        Debug.Log("TODO: check if can move so far");

        


        // MOVE CHARACTER 
        CenturionGame.Instance.OnCharacterMoved(
            characterId: characterId,
            x,
            y,
            stepsUsed: CalculateStepsUsed(x,y));

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

        //outgoingData.writeByte((byte)Messages.op_move_character);
        //outgoingData.writeUnsignedInt(characterId);
        //outgoingData.writeByte((byte)x);
        //outgoingData.writeByte((byte)y);
        //Send("move_character");
    }

}
