using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class SP_GameControl : MonoBehaviour
{
    public static SP_GameControl instance;

    [Header("OTHER CONTROLLERS")]
    public SP_HUD_Control hudControl;
    public CustomCursor_Simanis customCursor;

    [Header("UNITS")]
    public Transform predefinedUnitParent;
    [HideInInspector] public List<SP_Unit> allUnits;
    [HideInInspector] public SP_Unit prevSelectedUnit = null;

    [Header("BUILDINGS")]
    [HideInInspector] public SP_Building prevSelectedBuilding = null;

    [Header("TURN MANAGEMENT")]
    public float enemyTurnDuration = 5f;
    [HideInInspector] public float enemyTurnStartTime;
    [HideInInspector] public float enemyTurnEndTime;
    public bool isAllyTurn = true;

    [Header("UI")]
    public AnnouncementUi_Simanis announcementControl;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SP_MapControl.instance.InitializeMap();
        announcementControl.gameObject.SetActive(true);
        announcementControl.HideAnnouncmentText(0);
        InitializePredefinedUnits();
        hudControl.SetTurnInfoText();
        MarkInactiveUnits();
    }

    private void Update()
    {
        if (isAllyTurn && Input.GetKeyDown(KeyCode.Space))
        {
            EndTurn();
        }

        if (Input.GetMouseButtonDown(1))
        {
            DeselectEverything();
        }

        if (!isAllyTurn)
        {
            if (Time.time >= enemyTurnEndTime)
            {
                EndTurn();
            }
        }
    }

    public void DeselectEverything()
    {
        if(prevSelectedUnit != null)
        {
            prevSelectedUnit.SelectUnit(false);
        }
        if(prevSelectedBuilding != null)
        {
            prevSelectedBuilding.SelectBuilding(false);
        }
        if (SP_RaycastControl.instance.previousRaycast != null)
        {
            SP_RaycastInteract raycastInteract = SP_RaycastControl.instance.previousRaycast;
            raycastInteract.ShowClickObj(false);
            raycastInteract.HighlightThis(false);
            SP_RaycastControl.instance.previousRaycast = null;
        }

    }

    private void InitializePredefinedUnits()
    {
        allUnits = new List<SP_Unit>();
        foreach (Transform child in predefinedUnitParent)
        {
            SP_Unit newUnit = child.GetComponent<SP_Unit>();
            allUnits.Add(newUnit);
        }
        foreach (SP_Unit foundUnit in allUnits)
        {
            foundUnit.MoveUnit(foundUnit.x, foundUnit.y, instant: true);

            if(!foundUnit.isAllyUnit)
            {
                SP_EnemyUnitAI enemyControl = foundUnit.gameObject.AddComponent<SP_EnemyUnitAI>();
                enemyControl.myBehaviour = UnitBehaviour.RandomMoveAttackWhenAdjacent;
                enemyControl.myUnit = foundUnit;
                foundUnit.aiControl = enemyControl;
            }
            foundUnit.InitializeUnit();
        }
        UpdateUnitStats();
    }

    public void EndTurn()
    {
        isAllyTurn = !isAllyTurn;
        hudControl.SetTurnInfoText();
        UpdateUnitStats();
        DeselectEverything();
        if (!isAllyTurn)
        {
            announcementControl.ShowAnnouncmentText(
                bigAnnounce: "Enemy Turn",
                smallAnnounce: "",
                appearDuration: 0.8f,
                disappearDelay: 1.8f,
                disappearDuration: 1f);
            SP_LevelAudioControl.instance.PlaySFX(SP_LevelAudioControl.instance.enemyTurnSFX);
            hudControl.ShowTurnDuration(true);
            enemyTurnStartTime = Time.time;
            enemyTurnEndTime = Time.time + enemyTurnDuration;


            DOVirtual.DelayedCall(0.6f, () =>
            {
                MoveNextEnemy();
            });
        }
        else
        {
            announcementControl.ShowAnnouncmentText(
                bigAnnounce: "Your Turn",
                smallAnnounce: "",
                appearDuration: 0.8f,
                disappearDelay: 1.8f,
                disappearDuration: 1f);
            SP_LevelAudioControl.instance.PlaySFX(SP_LevelAudioControl.instance.yourTurnSFX);
            hudControl.ShowTurnDuration(false);

        }
    }

    public void MoveNextEnemy()
    {
        if (isAllyTurn)
            return;
        
        foreach(SP_Unit unit in allUnits)
        {
            if (!unit.isAllyUnit && !unit.aiControl.hasCompletedTurn)
            {
                unit.aiControl.DoMove();
                return;
            }
        }

        enemyTurnEndTime -= 0.01f;
        MoveNextEnemy();
    }

    private void UpdateUnitStats()
    {
        foreach (SP_Unit unit in allUnits)
        {
            unit.ProcessNewTurnStart();

        }
    }

    private void MarkInactiveUnits()
    {
        foreach (SP_Unit unit in allUnits)
        {
            if (unit.isAllyUnit)
            {
                if (isAllyTurn)
                {
                    unit.MarkUnitAsSleeping(false);
                }
                else
                {
                    unit.MarkUnitAsSleeping(true);
                }
            }
            else // enemy turn
            {
                if (isAllyTurn)
                {
                    unit.MarkUnitAsSleeping(true);
                }
                else
                {
                    unit.MarkUnitAsSleeping(false);
                }
            }
        }
    }
}
