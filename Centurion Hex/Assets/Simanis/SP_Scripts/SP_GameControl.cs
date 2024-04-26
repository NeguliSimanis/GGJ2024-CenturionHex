using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_GameControl : MonoBehaviour
{
    public static SP_GameControl instance;

    [Header("UNITS")]
    public Transform predefinedUnitParent;
    [HideInInspector] public SP_Unit prevSelectedUnit = null;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SP_MapControl.instance.InitializeMap();
        InitializePredefinedUnits();
    }

    private void InitializePredefinedUnits()
    {
        foreach (Transform child in predefinedUnitParent)
        {
            SP_Unit newUnit = child.GetComponent<SP_Unit>();
            newUnit.MoveUnit(newUnit.x, newUnit.y, instant: true);
        }
    }
}
