using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    [Header("TURN MANAGEMENT")]
    public float enemyTurnDuration = 5f;
    [HideInInspector] public float enemyTurnStartTime;
    [HideInInspector] public float enemyTurnEndTime;
    public bool isAllyTurn = true;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SP_MapControl.instance.InitializeMap();
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

        if (!isAllyTurn)
        {
            if (Time.time >= enemyTurnEndTime)
            {
                EndTurn();
            }
        }
    }

    private void InitializePredefinedUnits()
    {
        allUnits = new List<SP_Unit>();
        foreach (Transform child in predefinedUnitParent)
        {
            SP_Unit newUnit = child.GetComponent<SP_Unit>();
            allUnits.Add(newUnit);
            newUnit.InitializeUnit();
            newUnit.MoveUnit(newUnit.x, newUnit.y, instant: true);
        }
    }

    public void EndTurn()
    {
        isAllyTurn = !isAllyTurn;
        hudControl.SetTurnInfoText();
        UpdateUnitStats();

        if (!isAllyTurn)
        {
            hudControl.ShowTurnDuration(true);
            enemyTurnStartTime = Time.time;
            enemyTurnEndTime = Time.time + enemyTurnDuration;
        }
        else
        {
            hudControl.ShowTurnDuration(false);
        }
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
            if (unit.isMyUnit)
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
