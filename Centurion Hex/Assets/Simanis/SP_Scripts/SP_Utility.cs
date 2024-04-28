using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_Utility : MonoBehaviour
{
    public static bool IsEvenNumber(int num)
    {
        return num % 2 == 0;
    }

    public static bool PointerOverUI()
    {
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }
}
