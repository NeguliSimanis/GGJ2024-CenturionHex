using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SP_GameOverPopup : MonoBehaviour
{
    public static SP_GameOverPopup instance;
    public GameObject popup;

    private void Awake()
    {
        instance = this;
    }

    public void InitializePopup(bool isVictory)
    {
        ShowPopup(true);
    }

    public void ShowPopup(bool show)
    {
        popup.SetActive(show);
    }
}
